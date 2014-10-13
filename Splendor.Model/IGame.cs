using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	interface IGame
	{
		IList<IAction> Actions { get; }

		int Supply(Color color);

		//int[] Tokens { get; }
		Card[] Market { get; }
		Noble[] Nobles { get; }

		int CurrentPlayer { get; }

		IPlayer GetPlayer(int playerIndex);

		void GainToken(int playerIndex, Color color);

		void SpendToken(int playerIndex, Color color);
	}
}
