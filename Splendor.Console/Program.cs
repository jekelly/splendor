using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splendor.AI;
using Splendor.Model;

namespace Splendor.Console
{
	class Program
	{
		class RandomChooser : IChooser
		{
			private readonly Random rand = new Random();
			private readonly IGame game;

			public RandomChooser(IGame game, int index)
			{
				this.game = game;
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
				if (IsGoodChoice(action))
				{
					return action;
				}
				else
				{
					return actions.FirstOrDefault(s => !(s is TakeTokensAction)) ?? action;
				}
			}

			private bool IsGoodChoice(IAction action)
			{
				return true;
				//IPlayer player = this.game.CurrentPlayer;
				//return player.TokenCount < 8 || !(action is TakeTokensAction);
			}
		}

		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			// 1668
			int ma = 0;
			int[] movingAverage = new int[100];
			int[] runningTotal = new int[2];
			runningTotal[0] = runningTotal[1] = 1;
			const int gamesToPlay = int.MaxValue;
			var td = new TDChooser(1);
			for (int i = 0; i < gamesToPlay; i++)
			{
				//using (System.IO.StreamWriter log = new StreamWriter("out" + i + ".log"))
				{
					LoggingEventSink logger = null; // new LoggingEventSink(log);
					IRandomizer r = new Randomizer();
					Game game = new Game(Setups.All[0], r, logger);
					IChooser[] c = new IChooser[2];
					c[0] = new RandomChooser(game, 0);
					//c[1] = new RandomChooser(game, 0); //
					c[1] = td;
					int t = 0;
					while (game.CurrentPhase != Phase.GameOver)
					{
						t++;
						game.Step(c[game.CurrentPlayerIndex].Choose(game));
					}
					int winnerIndex = game.Players[0].Score > game.Players[1].Score ? 0 : 1;
					runningTotal[winnerIndex]++;
					//((TDChooser)c[1]).Learn(winnerIndex == 1);
					movingAverage[ma] = winnerIndex;
					ma = (ma + 1) % movingAverage.Length;
					System.Console.WriteLine("Game over after {0} turns: {1} wins, {2} to {3} [{4}] - {5}|{6}", t, winnerIndex, game.Players[winnerIndex].Score, game.Players[(winnerIndex + 1) % 2].Score, ((double)runningTotal[0] / runningTotal[1]), movingAverage.Count(a => a == 0), movingAverage.Count(a => a == 1));
				}
			}
			sw.Stop();
			double avg = sw.ElapsedMilliseconds / gamesToPlay;
			System.Console.WriteLine(avg);
		}
	}

	class LoggingEventSink : IEventSink
	{
		private readonly TextWriter output;

		public LoggingEventSink(TextWriter output)
		{
			this.output = output;
		}

		public void OnCardBuild(IPlayer player, Card card)
		{
			string text = string.Format("P{0} builds {1}.", player.Index, card);
			this.output.WriteLine(text);
		}

		public void OnCardReserved(IPlayer player, Card card)
		{
			string text = string.Format("P{0} reserves {1}.", player.Index, card);
			this.output.WriteLine(text);
		}

		public void OnTokensTaken(IPlayer player, Color[] tokens)
		{
			string tokenDescription = string.Join(", ", tokens);
			string text = string.Format("P{0} takes {1} from the supply.", player.Index, tokenDescription);
			this.output.WriteLine(text);
		}

		public void OnTokenReturned(IPlayer player, Color token)
		{
			string text = string.Format("P{0} returns {1} to the supply.", player.Index, token);
			this.output.WriteLine(text);
		}

		public void OnNobleVisit(IPlayer player, Noble noble)
		{
			string text = string.Format("P{0} is visited by Noble {1}.", player.Index, noble);
		}

		public void SummarizeGame(IGame game)
		{
			for (int i = 0; i < 2; i++)
			{
				IPlayer p = game.GetPlayer(i);
				this.SummarizePlayer(p);
			}
		}

		private void SummarizePlayer(IPlayer player)
		{
			string tableauSummary = "\tP{0} Tableau: {1}";
			var ts = player.Tableau.GroupBy(card => card.gives).Select(s => s.Count().ToString() + Colors.Short(s.Key).ToString());
			string ss = string.Join(", ", ts);
			this.output.WriteLine(tableauSummary, player.Index, ss);

			string gemsSummary = "\tP{0} Tokens: {1}";
			ss = string.Join(" ", Enumerable.Range(0, 6).Select(s => player.Tokens((Color)s).ToString() + Colors.Short((byte)s).ToString()));
			this.output.WriteLine(gemsSummary, player.Index, ss);

			this.output.WriteLine("\tP{0} Score: {1}", player.Index, player.Score);
		}
	}
}
