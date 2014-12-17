namespace Splendor.Model.AI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Splendor.Model;

	public class TDChooser : IChooser
	{
		// TODO: control this via a policy instead of hardcoding e-greedy
		private const double epsilon = 0;
		private const int HiddenUnits = 16;

		private readonly Random rand;
		private readonly int playerIndex;
		private readonly Net<IGame> net;
		private readonly List<IGame> history;
		private readonly bool shouldTrain;

		public double Alpha
		{
			get { return this.net.Alpha; }
			set { this.net.Alpha = value; }
		}

		public double Beta
		{
			get { return this.net.Beta; }
			set { this.net.Beta = value; }
		}

		public TDChooser(int playerIndex, bool shouldTrain)
		{
			this.rand = new Random();
			this.playerIndex = playerIndex;
			this.shouldTrain = shouldTrain;

			ISensor<IGame> gameSensor = new GameSensor4(playerIndex);
			this.net = new Net<IGame>(gameSensor, HiddenUnits);
			this.history = new List<IGame>();
		}

		public IAction Choose(IGame state)
		{
			IAction[] actions = state.AvailableActions.ToArray();
			IGame predictedBest = null;
			double maxValue = double.MinValue;
			int maxValueIndex = -1;

			// will this hurt TD?s
			if (!actions.Any())
			{
				return null;
			}
			if (actions.Length == 1)
			{
				return actions[0];
			}
			this.history.Add(state.Clone());
			double x = this.rand.NextDouble();
			if (x < epsilon || state.CurrentPhase != Phase.Choose)
			{
				maxValueIndex = this.rand.Next(actions.Length);
				//Debug.WriteLine("Choose randomly");
			}
			else
			{
				//Debug.WriteLine("Choose from net");
				//Debug.WriteLine("Choosing an action from {0} choices", actions.Length);
				//Debug.WriteLine("-----------------------------------------------");
				for (int i = 0; i < actions.Length; i++)
				{
					IGame clone = state.Clone();
					Debug.Assert(actions.Length == clone.AvailableActions.Count());
					clone.Step(actions[i]);
					while (clone.CurrentPhase != Phase.EndTurn)
					{
						clone.Step(clone.AvailableActions.FirstOrDefault());
					}
					var value = this.net.Eval(clone, state.EventSink);
					if (value > maxValue)
					{
						maxValue = value;
						maxValueIndex = i;
						predictedBest = clone;
					}
					//Debug.WriteLine("Action {0} had fitness {1}", i, value);
				}
				//Debug.WriteLine("Choose action {0}: {1}", maxValueIndex, actions[maxValueIndex]);
				//Debug.WriteLine("");
			}
			//epsilon = 1.0 / this.history.Count;
			return actions[maxValueIndex];
		}

		//private double Evaluate(IGame state)
		//{
		//	return this.net.Eval(state);
		//}

		public void PostGame(int winner, IEventSink eventSink)
		{
			if (this.shouldTrain)
			{
				this.net.Learn(this.history.ToArray(), (winner == this.playerIndex ? 1.0 : 0), eventSink);
			}
			this.history.Clear();
		}
	}
}
