namespace Splendor.Model.AI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	public class IanMStrategy : IChooser
	{
		private readonly Random rand = new Random();

		struct Diff
		{
			public int Score;
			public int Tokens;
			public int Gems;
			public int Index;

			public double ScoreEfficiency
			{
				get
				{
					if (this.Score == 0)
					{
						return double.MinValue;
					}
					return this.Tokens == 0 ? this.Score : (double)this.Score / this.Tokens;
				}
			}

			public double GemEfficiency
			{
				get
				{
					if (this.Gems == 0)
					{
						return double.MinValue;
					}
					return this.Tokens == 0 ? this.Gems : (double)this.Gems / this.Tokens;
				}
			}
		}

		public IanMStrategy(int playerIndex)
		{

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
				// can we build a card that will provide a noble?
				// can we build a card for 3 tokens or less?
				int initialTokens = state.CurrentPlayer.TokenCount;
				int initialScore = state.CurrentPlayer.Score;
				int initialGems = state.CurrentPlayer.GemCount;
				Diff[] diffs = new Diff[actions.Length];
				for (int i = 0; i < actions.Length; i++)
				{
					IGame clone = state.Clone();
					clone.Step(actions[i]);
					while (clone.CurrentPhase != Phase.EndTurn && clone.CurrentPhase != Phase.GameOver)
					{
						clone.Step(clone.AvailableActions.FirstOrDefault());
					}
					int scoreDiff = clone.CurrentPlayer.Score - initialScore;
					int tokenDiff = clone.CurrentPlayer.TokenCount - initialTokens;
					int gemDiff = clone.CurrentPlayer.GemCount - initialGems;
					diffs[i] = new Diff() { Gems = gemDiff, Score = scoreDiff, Tokens = tokenDiff, Index = i };
				}
				diffs = diffs.OrderByDescending(d => d.ScoreEfficiency).ThenByDescending(d => d.GemEfficiency).ThenByDescending(d => d.Tokens).ToArray();
				var diff = diffs.First();
				index = diff.Index;
			}
			if (index == -1)
			{
				index = this.rand.Next(actions.Length);
			}
			Debug.WriteLine("Choose {0}", actions[index]);
			return actions[index];
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}
	}

	public class HighestScoreChooser : IChooser
	{
		private readonly Random rand = new Random();
		private readonly int playerIndex;

		private Card targetCard;

		public HighestScoreChooser(int playerIndex)
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

	public class MaxScoreChooser : IChooser
	{
		private readonly Random rand = new Random();
		private readonly int playerIndex;

		public MaxScoreChooser(int playerIndex)
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
			int preScore = state.GetPlayer(this.playerIndex).Score;
			int maxDiff = 0;
			int index = -1;
			for (int i = 0; i < actions.Length; i++)
			{
				IGame clone = state.Clone();
				clone.Step(actions[i]);
				while (clone.CurrentPhase != Phase.EndTurn && clone.CurrentPhase != Phase.GameOver)
				{
					clone.Step(clone.AvailableActions.FirstOrDefault());
				}
				int diff = clone.GetPlayer(this.playerIndex).Score - preScore;
				if (diff > maxDiff)
				{
					maxDiff = diff;
					index = i;
				}
			}
			if (index == -1)
			{
				index = this.rand.Next(actions.Length);
			}
			return actions[index];
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}
	}

	public class ColorPreferenceChooser : IChooser
	{
		private const double TokenValue = 1.0;
		private const double GemValue = 10.0;

		private readonly int playerIndex;

		private Color favoriteColor;

		public ColorPreferenceChooser(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.favoriteColor = Color.Gold;
		}

		public IAction Choose(IGame state)
		{
			if (this.favoriteColor == Color.Gold)
			{
				this.favoriteColor = DetermineBestColor(state);
			}

			var actions = state.AvailableActions.ToArray();
			if (actions.Length == 0)
			{
				return null;
			}
			if (actions.Length == 1)
			{
				return actions[0];
			}

			double best = double.MinValue;
			int bestIndex = int.MinValue;
			int preCountTokens = state.GetPlayer(this.playerIndex).Tokens(this.favoriteColor);
			int preCountGems = state.GetPlayer(this.playerIndex).Gems(this.favoriteColor);
			for (int i = 0; i < actions.Length; i++)
			{
				var action = actions[i];
				var clone = state.Clone();
				clone.Step(action);
				int postCountTokens = clone.GetPlayer(this.playerIndex).Tokens(this.favoriteColor);
				int postCountGems = clone.GetPlayer(this.playerIndex).Gems(this.favoriteColor);
				double v = (postCountTokens - preCountTokens) * TokenValue + (postCountGems - preCountGems) * GemValue;
				if (v > best)
				{
					best = v;
					bestIndex = i;
				}
			}
			return actions[bestIndex];
		}

		private Color DetermineBestColor(IGame state)
		{
			double[] colorValues = new double[5];
			foreach (Card card in state.Market)
			{
				if (card.value > 0)
				{
					colorValues[(int)Color.White] += (double)card.value / (card.costWhite);
					colorValues[(int)Color.Blue] += (double)card.value / (card.costBlue);
					colorValues[(int)Color.Green] += (double)card.value / (card.costGreen);
					colorValues[(int)Color.Red] += (double)card.value / (card.costRed);
					colorValues[(int)Color.Black] += (double)card.value / (card.costBlack);
				}
			}
			foreach (Noble noble in state.Nobles)
			{
				for (int i = 0; i < 5; i++)
				{
					colorValues[i] += 3.0d / noble.requires[i];
				}
			}
			return colorValues.Select((v, i) => new { Value = v, Color = (Color)i }).OrderByDescending(a => a.Value).Select(v => v.Color).First();
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
			this.favoriteColor = Color.Gold;
		}
	}
}
