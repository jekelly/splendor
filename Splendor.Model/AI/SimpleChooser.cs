namespace Splendor.Model.AI
{
	using System;
	using System.Linq;

	public class SimpleChooser : IChooser
	{
		private readonly Random rand;

		public SimpleChooser(int index)
		{
			this.rand = new Random();
		}

		public IAction Choose(IGame state)
		{
			var actions = state.AvailableActions;
			if (!actions.Any())
			{
				return null;
			}
			int size = actions.Count();
			int rand = this.rand.Next(size);
			IAction action = actions.ElementAt(rand);
			if (IsGoodChoice(state, action))
			{
				return action;
			}
			else
			{
				return actions.FirstOrDefault(s => !(s is TakeTokensAction)) ?? action;
			}
		}

		private static bool IsGoodChoice(IGame game, IAction action)
		{
			IPlayer player = game.CurrentPlayer;
			return player.TokenCount < 8 || !(action is TakeTokensAction);
		}

		public void PostGame(int winner, IEventSink eventSink)
		{
		}
	}
}
