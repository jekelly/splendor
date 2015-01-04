namespace Splendor.Model.AI
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	/// <summary>
	/// Choose actions based on value ratio of Score, then Gems gained to Tokens spent, or failing that, prefer actions that 
	/// gain the most Tokens.
	/// </summary>
	public class IanMStrategy : IChooser
	{
		private readonly IRandomizer rand;
		private readonly int playerIndex;

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
			this.playerIndex = playerIndex;
			this.rand = new Randomizer();
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
				IPlayer currentPlayer = state.GetPlayer(this.playerIndex);
				int initialTokens = currentPlayer.TokenCount;
				int initialScore = currentPlayer.Score;
				int initialGems = currentPlayer.GemCount;
				Diff[] diffs = new Diff[actions.Length];
				for (int i = 0; i < actions.Length; i++)
				{
					IGame clone = state.Clone();
					IPlayer clonePlayer = clone.GetPlayer(this.playerIndex);
					clone.Step(actions[i]);
					while (clone.CurrentPhase != Phase.EndTurn && clone.CurrentPhase != Phase.GameOver)
					{
						clone.Step(clone.AvailableActions.FirstOrDefault());
					}
					int scoreDiff = clonePlayer.Score - initialScore;
					int tokenDiff = clonePlayer.TokenCount - initialTokens;
					int gemDiff = clonePlayer.GemCount - initialGems;
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

}
