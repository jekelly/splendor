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
		private const double epsilon = 0.00;
		private const int HiddenUnits = 80;

		private readonly Random rand;
		private readonly int playerIndex;
		private readonly Net<IGame> net;
		private readonly List<IGame> history;
		private readonly bool shouldTrain;

		private readonly ISensor<IGame>[] sensors;

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

			this.sensors = new ISensor<IGame>[2];
			this.sensors[(playerIndex + 1) % 2] = new GameSensor3((playerIndex + 1) % 2);
			this.sensors[playerIndex] = new GameSensor3(playerIndex);

			this.net = new Net<IGame>(this.sensors[playerIndex], HiddenUnits);
			this.history = new List<IGame>();
		}

		private const int MaxPly = 0;
		private double AlphaBetaEval(IGame state, IEventSink eventSink, int ply, double alpha, double beta)
		{
			IAction[] actions = state.AvailableActions.ToArray();
			if (ply == MaxPly)
			{
				return this.net.Eval(state, eventSink);
			}
			bool maxPlayer = (ply % 2) == 0;
			if (maxPlayer)
			{
				for (int i = 0; i < actions.Length; i++)
				{
					IGame childState = state.Clone();
					childState.Step(actions[i]);
					alpha = Math.Max(alpha, this.AlphaBetaEval(childState, eventSink, ply + 1, alpha, beta));
					if (beta <= alpha) break;
				}
				return alpha;
			}
			else
			{
				for (int i = 0; i < actions.Length; i++)
				{
					IGame childState = state.Clone();
					childState.Step(actions[i]);
					beta = Math.Min(beta, this.AlphaBetaEval(childState, eventSink, ply + 1, alpha, beta));
					if (beta <= alpha) break;
				}
				return beta;
			}
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
					// hack to avoid the allure of gold
					ReserveCardAction rca = actions[i] as ReserveCardAction;
					if (rca != null) continue;
					IGame clone = state.Clone();
					Debug.Assert(actions.Length == clone.AvailableActions.Count());
					clone.Step(actions[i]);
					while (clone.CurrentPhase != Phase.EndTurn)
					{
						clone.Step(clone.AvailableActions.FirstOrDefault());
					}
					//var value = this.Eval(clone, state.EventSink);
					double value = this.AlphaBetaEval(clone, state.EventSink, 0, double.MinValue, double.MaxValue);
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
			if (maxValueIndex == -1)
			{
				maxValueIndex = this.rand.Next(actions.Length);
			}
			//epsilon = 1.0 / this.history.Count;
			return actions[maxValueIndex];
		}

		private double Eval(IGame state, IEventSink eventSink)
		{
			return this.net.Eval(state, eventSink);
		}

		//private double Evaluate(IGame state)
		//{
		//	return this.net.Eval(state);
		//}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
			if (this.shouldTrain)
			{
				for (int i = 0; i < history.Length; i++)
				{
					var sensor = this.net.Sensor;
					this.net.Sensor = this.sensors[i];
					this.net.Learn(history[i].ToArray(), (winner == i ? 1.0 : 0.0), eventSink);
					this.net.Sensor = sensor;
				}

			}
			this.history.Clear();
		}
	}
}
