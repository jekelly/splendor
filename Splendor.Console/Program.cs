using System;
using System.Collections.Generic;
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
			private readonly Random rand = new Random();

			public IAction Choose(IEnumerable<IAction> actions)
			{
				int size = actions.Count();
				int rand = this.rand.Next(size);
				return actions.ElementAt(rand);
			}
		}

		static void Main(string[] args)
		{
			Game game = new Game(Setups.All[0]);
			RandomChooser c = new RandomChooser();
			while(game.CurrentPhase != Phase.GameOver)
			{
				game.Step(c);
			}
		}
	}
}
