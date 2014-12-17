namespace Splendor.Console
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Splendor.Model;
	using Splendor.Model.AI;

	class Program
	{
		private const int GamesToPlay = 20000;

		public static void Main(string[] args)
		{
			int ma = 0;
			int[] movingAverage = new int[100];
			int[] runningTotal = new int[2];
			IChooser[] choosers = new IChooser[2];
			//choosers[0] = new SimpleChooser(0);
			//choosers[1] = new IanMStrategy(1);
			choosers[0] = new IanMStrategy(0);
			//choosers[0] = new TDChooser(0, false);
			choosers[1] = new TDChooser(1, true);
			for (int i = 0; i < GamesToPlay; i++)
			{
				((TDChooser)choosers[1]).Alpha = GetLearningRateForIteration(i);
				((TDChooser)choosers[1]).Beta = GetLearningRateForIteration(i);
				if (!Directory.Exists("AI"))
				{
					Directory.CreateDirectory("AI");
				}
				TextWriter output = i % 100 == 0 ? new StreamWriter("AI\\game" + i + " .log") : null;
				using (LoggingEventSink logger = new LoggingEventSink(output))
				{
					IRandomizer r = new Randomizer();
					Game game = new Game(Setups.All[0], r, logger);
					int winnerIndex = RunGame(game, choosers);
					runningTotal[winnerIndex]++;
					movingAverage[ma] = winnerIndex;
					ma = (ma + 1) % movingAverage.Length;
					System.Console.WriteLine("Game over after {0} turns: {1} wins, {2} to {3} [{4}] - {5}|{6}", game.Turns, winnerIndex, game.Players[winnerIndex].Score, game.Players[(winnerIndex + 1) % 2].Score, ((double)runningTotal[0] / runningTotal[1]), movingAverage.Count(a => a == 0), movingAverage.Count(a => a == 1));
				}
			}
		}

		private static double GetLearningRateForIteration(int i)
		{
			if (i < 100)
			{
				return 0.1;
			}
			if (i < 1000)
			{
				return 0.01;
			}
			if (i < 10000)
			{
				return 0.001;
			}
			return 0.0001;
		}

		private static int RunGame(IGame game, IChooser[] choosers)
		{
			while (game.CurrentPhase != Phase.GameOver)
			{
				var action = choosers[game.CurrentPlayerIndex].Choose(game);
				game.Step(action);
			}
			int maxScore = int.MinValue;
			int winner = int.MinValue;
			foreach (IPlayer player in game.Players)
			{
				if (player.Score >= maxScore)
				{
					winner = player.Index;
					maxScore = player.Score;
				}
			}
			for (int i = 0; i < choosers.Length; i++)
			{
				choosers[i].PostGame(winner, game.EventSink);
			}
			return winner;
		}
	}

	class LoggingEventSink : IEventSink, IDisposable
	{
		private readonly TextWriter output;
		
		public LoggingEventSink(TextWriter output)
		{
			this.output = output ?? new StringWriter();
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
			this.output.WriteLine(text);
		}

		public void SummarizeGame(IGame game)
		{
			for (int i = 0; i < 2; i++)
			{
				IPlayer p = game.GetPlayer(i);
				this.SummarizePlayer(p);
			}
		}

		public void DebugMessage(string message, params object[] args)
		{
			this.output.WriteLine("\tDEBUG: " + string.Format(message, args));
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

		public void Dispose()
		{
			this.output.Dispose();
		}
	}
}
