namespace Splendor.Model
{
	using System.Collections.Generic;

	public interface IGame
	{
		IEventSink EventSink { get; }

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
