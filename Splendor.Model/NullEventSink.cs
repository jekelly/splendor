namespace Splendor.Model
{
	sealed class NullEventSink : IEventSink
	{
		public static readonly IEventSink Instance = new NullEventSink();

		private NullEventSink()
		{
		}

		public void OnCardBuild(IPlayer player, Card card)
		{
		}

		public void OnCardReserved(IPlayer player, Card card)
		{
		}

		public void OnTokensTaken(IPlayer player, Color[] tokens)
		{
		}

		public void OnTokenReturned(IPlayer player, Color token)
		{
		}

		public void OnNobleVisit(IPlayer player, Noble noble)
		{
		}
		public void SummarizeGame(IGame game)
		{
		}

		public void DebugMessage(string messageFormat, params object[] args)
		{
		}
	}
}
