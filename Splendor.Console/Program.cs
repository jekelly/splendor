using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splendor.Model;

namespace Splendor.Console
{
	class Program
	{
		class RandomChooser : IChooser
		{
			private readonly Random rand = new Random(0);
			private readonly IGame game;
			private readonly int index;

			public RandomChooser(IGame game, int index)
			{
				this.game = game;
				this.index = index;
			}


			public IAction Choose(IEnumerable<IAction> actions)
			{
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
				IPlayer player = this.game.GetPlayer(this.index);
				return player.TokenCount < 8 || !(action is TakeTokensAction);
			}
		}

		static void Main(string[] args)
		{
			LoggingEventSink logger = null; // new LoggingEventSink(System.Console.Out);
			Stopwatch sw = new Stopwatch();
			sw.Start();
			// 1668
			for (int i = 0; i < 100; i++)
			{
				IRandomizer r = new Randomizer(i);
				Game game = new Game(Setups.All[0], r, logger);
				RandomChooser[] c = new RandomChooser[2];
				c[0] = new RandomChooser(game, 0);
				c[1] = new RandomChooser(game, 1);
				while (game.CurrentPhase != Phase.GameOver)
				{
					game.Step(c[game.CurrentPlayerIndex]);
				}
			}
			sw.Stop();
			double avg = sw.ElapsedMilliseconds / 100;
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
			string text = string.Format("Player {0} builds {1}.", player.Index, card);
			this.output.WriteLine(text);
		}

		public void OnCardReserved(IPlayer player, Card card)
		{
			string text = string.Format("Player {0} reserves {1}.", player.Index, card);
			this.output.WriteLine(text);
		}

		public void OnTokensTaken(IPlayer player, Color[] tokens)
		{
			string tokenDescription = string.Join(", ", tokens);
			string text = string.Format("Player {0} takes {1} from the supply.", player.Index, tokenDescription);
			this.output.WriteLine(text);
		}

		public void OnTokensReturned(IPlayer player, Color[] tokens)
		{
			string tokenDescription = string.Join(", ", tokens);
			string text = string.Format("Player {0} returns {1} to the supply.", player.Index, tokenDescription);
			this.output.WriteLine(text);
		}

		public void OnNobleVisit(IPlayer player, Noble noble)
		{
			string text = string.Format("Player {0} is visited by Noble {1}.", player.Index, noble);
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
			string tableauSummary = "Player {0} Tableau: {1}";
			var ts = player.Tableau.GroupBy(card => card.gives).Select(s => s.Count() + " " + Colors.Short(s.Key));
			string ss = string.Join(", ", ts);
			this.output.WriteLine(tableauSummary, player.Index, ss);
		}
	}
}
