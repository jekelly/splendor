namespace Splendor.Model.AI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Choose the action that gives the biggest score increase, otherwise act randomly.
	/// </summary>
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
}
