namespace Splendor.Model
{
	public interface IEventSink
	{
		void OnCardBuild(IPlayer player, Card card);
		void OnCardReserved(IPlayer player, Card card);
		void OnTokensTaken(IPlayer player, Color[] tokens);
		void OnTokenReturned(IPlayer player, Color tokens);
		void OnNobleVisit(IPlayer player, Noble noble);
		void SummarizeGame(IGame game);
		void DebugMessage(string messageFormat, params object[] args);
	}
}
