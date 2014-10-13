using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	class ReplaceTokenAction : IAction
	{
		private readonly Color color;

		public ReplaceTokenAction(Color color)
		{
			this.color = color;
		}

		public bool CanExecute(IGame game)
		{
			int totalTokens = 0;
			IPlayer player = game.GetPlayer(game.CurrentPlayer);
			for(Color c = Color.White; c <= Color.Gold; c++)
			{
				totalTokens += player.Tokens(c);
			}
			return player.Tokens(this.color) > 0 && totalTokens > 10;
		}

		public void Execute(IGame game)
		{
			game.SpendToken(game.CurrentPlayer, this.color);
		}
	}
}
