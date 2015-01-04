namespace Splendor.Model.AI
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	public class ExpectedValueChooser : GreedyChooser
	{
		// Expected value of a position can be estimated via:
		// current score (known)
		// for nobles: value of noble * distance to noble * prob of getting that noble
		// for cards in market: value of card * prob of getting that card * distance to card
		// for cards in hand: value of card * distance to card
		// for cards in neither: value of card * distance to card * prob of card coming out

		public ExpectedValueChooser(int playerIndex)
			: base(playerIndex)
		{
		}

		protected async override Task<double> EvaluateStateAsync(IGame state)
		{
			return await Task.Run(() =>
				{
					double value = 0;
					IPlayer currentPlayer = state.GetPlayer(this.PlayerIndex);

					//double[] gemValues = CalculateGemValues(state);

					value += currentPlayer.Score;
					// build a table of gem prevalence
					double[] tokenValues = new double[5];
					// tokens can only buy market cards, so don't count nobles
					for (int m = 0; m < state.Market.Length; m++)
					{
						Card card = state.Market[m];
						for (int c = 0; c < 5; c++)
						{
							tokenValues[c] += Math.Max(0, card.Cost((Color)c) - currentPlayer.Gems((Color)c));
						}
					}
					double[] gemValues = new double[5];
					Buffer.BlockCopy(tokenValues, 0, gemValues, 0, 5);
					for (int n = 0; n < state.Nobles.Length; n++)
					{
						Noble noble = state.Nobles[n];
						for (int c = 0; c < 5; c++)
						{
							gemValues[c] += Math.Max(0, noble.requires[c] - currentPlayer.Gems((Color)c));
						}
					}
					double totalTokens = tokenValues.Sum();
					double totalGems = gemValues.Sum();
					for (int i = 0; i < tokenValues.Length; i++)
					{
						tokenValues[i] /= totalTokens;
						gemValues[i] /= totalGems;
					}
					// for each card in tableau, value is the gem value it gives + weighted value of cards it contributes towards buying
					foreach (Card card in currentPlayer.Tableau)
					{
						value += tokenValues[card.gives];
						for (int m = 0; m < state.Market.Length; m++)
						{
							Card marketCard = state.Market[m];
							if (marketCard.id == Rules.SentinelCard.id || marketCard.Cost((Color)card.gives) == 0) continue;
							double cardValue = marketCard.value + gemValues[marketCard.gives];
							double cardCost = TotalCost(marketCard);
							value += (cardValue / cardCost);
						}
					}
					// for tokens, give their gem value once (for gold, give highest gem value)
					for (int c = 0; c < 5; c++)
					{
						value += tokenValues[c] * currentPlayer.Tokens((Color)c);
					}
					value += tokenValues.Max() * currentPlayer.Tokens(Color.Gold);


					//for (int n = 0; n < state.Nobles.Length; n++)
					//{
					//	Noble noble = state.Nobles[n];
					//	value += noble.value * NobleRatio(state, currentPlayer, noble);
					//}
					// for each market card, get some value for each percentage of matching gems we have
					//for (int m = 0; m < state.Market.Length; m++)
					//{
					//	Card card = state.Market[m];
					//	if (card.id == Rules.SentinelCard.id) continue;
					//	value += (card.value + gemValues[card.gives]) * CardRatio(state, currentPlayer, card);
					//	// todo: secondary value of card as a gem in terms of expectations of its future value?
					//}
					//foreach (Card card in currentPlayer.Hand)
					//{
					//	if (card.id == Rules.SentinelCard.id) continue;
					//	int totalCost = card.costBlack + card.costBlue + card.costGreen + card.costRed + card.costWhite;
					//	value += card.value * (1.0 - (Distance(card, currentPlayer) / totalCost));
					//}

					return value;
				});
		}

		// card value: value + (# cards in market it contributes to * card value * contribution) + (nobles * noble value * contribution)

		private static int TotalCost(Card card)
		{
			return card.costBlack + card.costBlue + card.costGreen + card.costRed + card.costWhite;
		}

		private double[] CalculateGemValues(IGame state)
		{
			double[] values = new double[5];

			for (int m = 0; m < state.Market.Length; m++)
			{
				Card card = state.Market[m];
				int cost = TotalCost(card);
				for (int color = 0; color < 5; color++)
				{
					if (card.Cost((Color)color) > 0)
					{
						values[color] += card.value * (1.0 / cost);
					}
				}
			}
			//for (int n = 0; n < state.Nobles.Length; n++)
			//{
			//	Noble noble = state.Nobles[n];
			//	int cost = noble.requires[0] + noble.requires[1] + noble.requires[2] + noble.requires[3] + noble.requires[4];
			//	for (int color = 0; color < 5; color++)
			//	{
			//		if (noble.requires[color] > 0)
			//		{
			//			values[color] += noble.value * (1.0 / cost);
			//		}
			//	}
			//}
			return values;
		}

		private double CardRatio(IGame state, IPlayer player, Card card)
		{
			int ourDistance = Distance(card, player);
			int opponentDistance = this.ForBestOpponent(state, p => Distance(card, p));
			if (ourDistance == 0 && opponentDistance == 0)
			{
				return 0.5;
			}
			return 1.0 - ((double)ourDistance / (ourDistance + opponentDistance));
		}

		private int Distance(Card card, IPlayer player)
		{
			int distance = 0;
			distance += Math.Max(0, card.costBlack - player.BuyingPower[(int)Color.Black]);
			distance += Math.Max(0, card.costGreen - player.BuyingPower[(int)Color.Green]);
			distance += Math.Max(0, card.costBlue - player.BuyingPower[(int)Color.Blue]);
			distance += Math.Max(0, card.costRed - player.BuyingPower[(int)Color.Red]);
			distance += Math.Max(0, card.costWhite - player.BuyingPower[(int)Color.White]);
			distance = Math.Max(0, distance - player.Tokens(Color.Gold));
			return distance;
		}

		private double NobleRatio(IGame state, IPlayer player, Noble noble)
		{
			// ratio of our distance to best opponent's distance
			int ourDistance = Distance(noble, player);
			int opponentDistance = this.ForBestOpponent(state, p => Distance(noble, p));
			if (ourDistance == 0 && opponentDistance == 0)
			{
				return 0.5;
			}
			return 1.0 - ((double)ourDistance / (ourDistance + opponentDistance));
		}

		private int ForBestOpponent(IGame state, Func<IPlayer, int> x)
		{
			int best = int.MinValue;
			for (int i = 0; i < state.Players.Length; i++)
			{
				if (i == this.PlayerIndex) continue;
				int value = x(state.Players[i]);
				if (value > best)
				{
					best = value;
				}
			}
			return best;
		}

		private static int Distance(Noble noble, IPlayer player)
		{
			int distance = 0;
			for (int c = 0; c < 5; c++)
			{
				distance += Math.Max(0, noble.requires[c] - player.Gems((Color)c));
			}
			return distance;
		}
	}
}
