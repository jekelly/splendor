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
			choosers[1] = new TDChooser(1);
			for (int i = 0; i < GamesToPlay; i++)
			{
				//using (LoggingEventSink logger = new LoggingEventSink(i))
				{
					IRandomizer r = new Randomizer();
					Game game = new Game(Setups.All[0], r, null);
					int winnerIndex = RunGame(game, choosers);
					runningTotal[winnerIndex]++;
					movingAverage[ma] = winnerIndex;
					ma = (ma + 1) % movingAverage.Length;
					System.Console.WriteLine("Game over after {0} turns: {1} wins, {2} to {3} [{4}] - {5}|{6}", game.Turns, winnerIndex, game.Players[winnerIndex].Score, game.Players[(winnerIndex + 1) % 2].Score, ((double)runningTotal[0] / runningTotal[1]), movingAverage.Count(a => a == 0), movingAverage.Count(a => a == 1));
				}
			}
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
				choosers[i].PostGame(winner);
			}
			return winner;
		}
	}

	class LoggingEventSink : IEventSink, IDisposable
	{
		private readonly TextWriter output;
		
		public LoggingEventSink(int i)
		{
			if (i % 1000 == 0)
			{
				this.output = System.Console.Out;
			}
			else
			{
				this.output = new StreamWriter("out" + i + ".log");
			}
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

		public void Dispose()
		{
			this.output.Dispose();
		}
	}
}
