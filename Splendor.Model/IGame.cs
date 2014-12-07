using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public interface IGame
	{
		int Supply(Color color);

		Card[] Market { get; }

		Noble[] Nobles { get; }

		IEnumerable<IAction> AvailableActions { get; }

		int CurrentPlayerIndex { get; }

		Phase CurrentPhase { get; }

		IPlayer CurrentPlayer { get; }

		IPlayer GetPlayer(int playerIndex);

		void Step(IChooser chooser);

		void NextPhase();

	}
}
