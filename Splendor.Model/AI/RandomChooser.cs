namespace Splendor.Model.AI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class RandomChooser : IChooser
	{
		private readonly Random rand;

		public RandomChooser(int index)
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
			int rand = this.rand.Next(actions.Count());
			return actions.ElementAt(rand);
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}
	}
}
