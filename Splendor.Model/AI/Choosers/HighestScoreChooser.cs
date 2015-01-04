namespace Splendor.Model.AI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	/// <summary>
	/// Picks the highest point value card it can afford and works towards buying it.
	/// </summary>
	public class TargetCardChooser : IChooser
	{
		private readonly Random rand = new Random();
		private readonly int playerIndex;

		private Card targetCard;

		public TargetCardChooser(int playerIndex)
		{
			this.playerIndex = playerIndex;
		}

		public IAction Choose(IGame state)
		{
			var actions = state.AvailableActions.ToArray();
			if (actions.Length == 0)
			{
				return null;
			}
			if (actions.Length == 1)
			{
				return actions[0];
			}
			int index = -1;
			if (state.CurrentPhase == Phase.Choose)
			{
				IPlayer player = state.GetPlayer(this.playerIndex);
				// if we have a target, make sure its still there
				if (this.targetCard.id != Rules.SentinelCard.id && !(state.Market.Contains(this.targetCard) || player.Hand.Contains(this.targetCard)))
				{
					this.targetCard = Rules.SentinelCard;
				}
				// if not, pick the biggest point value card we can buy given the game constraints
				// TODO: generalize past 2 players
				int maxTokenCost = 4;// +(3 - player.Hand.Count());
				if (this.targetCard.id == Rules.SentinelCard.id)
				{
					this.targetCard = state.Market.OrderByDescending(c => c.value).Where(c => DiscountedCost(c, player) <= Rules.MaxTokensHeld && HighestCost(c, player) < maxTokenCost).FirstOrDefault();
					Debug.WriteLine("Choose target: {0}", targetCard);
				}
				// search the options for states that get us closer to buying our target card
				if (this.targetCard.id != Rules.SentinelCard.id)
				{
					int preDistance = CostUntil(this.targetCard, player);
					int maxDiff = int.MinValue;
					for (int i = 0; i < actions.Length; i++)
					{
						IAction action = actions[i];
						IGame clone = state.Clone();
						clone.Step(action);
						while (clone.CurrentPhase != Phase.GameOver && clone.CurrentPhase != Phase.EndTurn)
						{
							clone.Step(clone.AvailableActions.FirstOrDefault());
						}
						IPlayer clonePlayer = clone.GetPlayer(this.playerIndex);
						int diff = preDistance - CostUntil(this.targetCard, clonePlayer);
						if (clonePlayer.Tableau.Contains(this.targetCard))
						{
							index = i;
							break;
						}
						if (diff > maxDiff)
						{
							maxDiff = diff;
							index = i;
						}
					}
				}
			}
			if (index == -1)
			{
				index = this.rand.Next(actions.Length);
			}
			Debug.WriteLine("Choose {0}", actions[index]);
			return actions[index];
		}

		private int CostUntil(Card card, IPlayer player)
		{
			int totalCost = 0;
			totalCost += Math.Max(0, card.costWhite - player.Gems(Color.White) - player.Tokens(Color.White));
			totalCost += Math.Max(0, card.costBlue - player.Gems(Color.Blue) - player.Tokens(Color.Blue));
			totalCost += Math.Max(0, card.costGreen - player.Gems(Color.Green) - player.Tokens(Color.Green));
			totalCost += Math.Max(0, card.costRed - player.Gems(Color.Red) - player.Tokens(Color.Red));
			totalCost += Math.Max(0, card.costBlack - player.Gems(Color.Black) - player.Tokens(Color.Black));
			totalCost = Math.Max(0, totalCost - player.Tokens(Color.Gold));
			return totalCost;
		}

		private int HighestCost(Card card, IPlayer player)
		{
			return Math.Min(Math.Max(0, card.costWhite - player.Gems(Color.White)),
				Math.Min(Math.Max(0, card.costBlue - player.Gems(Color.Blue)),
				Math.Min(Math.Max(0, card.costGreen - player.Gems(Color.Green)),
				Math.Min(Math.Max(0, card.costRed - player.Gems(Color.Red)),
				Math.Max(0, card.costBlack - player.Gems(Color.Black))))));
		}

		private int DiscountedCost(Card card, IPlayer player)
		{
			int totalCost = 0;
			totalCost += card.costWhite - player.Gems(Color.White);
			totalCost += card.costBlue - player.Gems(Color.Blue);
			totalCost += card.costGreen - player.Gems(Color.Green);
			totalCost += card.costRed - player.Gems(Color.Red);
			totalCost += card.costBlack - player.Gems(Color.Black);
			return totalCost;
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}
	}
}
