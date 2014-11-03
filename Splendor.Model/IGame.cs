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

		Card[] Market { get; }

		Noble[] Nobles { get; }

		int CurrentPlayerIndex { get; }

		IPlayer CurrentPlayer { get; }

		IPlayer GetPlayer(int playerIndex);

		void GainToken(int playerIndex, Color color);

		void SpendToken(int playerIndex, Color color);

		void MoveCardToTableau(int playerIndex, Card card);

		void MoveCardToHand(int playerIndex, Card card);
	}
}
