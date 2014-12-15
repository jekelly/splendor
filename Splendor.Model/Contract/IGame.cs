namespace Splendor.Model
{
	using System.Collections.Generic;

	public interface IGame
	{
		IEventSink EventSink { get; }

		int Turns { get; }

		int Supply(Color color);

		int Debt(Color color);

		Card[] Market { get; }

		Noble[] Nobles { get; }

		IEnumerable<IAction> AvailableActions { get; }

		int CurrentPlayerIndex { get; }

		Phase CurrentPhase { get; }

		IPlayer[] Players { get; }

		IPlayer CurrentPlayer { get; }

		IPlayer GetPlayer(int playerIndex);

		void Step(IAction choice);

		void NextPhase();

		IGame Clone();
	}
}
