namespace Splendor.Model.AI
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;

	public abstract class GreedyChooser : IChooser
	{
		private readonly int playerIndex;

		protected int PlayerIndex { get { return this.playerIndex; } }

		public GreedyChooser(int playerIndex)
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
			Task<double>[] tasks = new Task<double>[actions.Length];
			if (state.CurrentPhase == Phase.Choose)
			{
				for (int i = 0; i < actions.Length; i++)
				{
					IAction action = actions[i];
					IGame clone = state.Clone();
					clone.Step(action);
					while (clone.CurrentPhase != Phase.GameOver && clone.CurrentPhase != Phase.EndTurn)
					{
						clone.Step(clone.AvailableActions.FirstOrDefault());
					}
					tasks[i] = this.EvaluateStateAsync(clone);
				}
				Task.WaitAll(tasks);
				double max = double.MinValue;
				double[] results = tasks.Select(t => t.Result).ToArray();
				for (int i = 0; i < results.Length; i++)
				{
					if (results[i] > max)
					{
						index = i;
						max = results[i];
					}
				}
			}
			if (index == -1)
			{
				index = 0;
			}
			Debug.WriteLine("Choose {0}", actions[index]);
			return actions[index];
		}

		public virtual void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}

		protected abstract Task<double> EvaluateStateAsync(IGame state);
	}
}
