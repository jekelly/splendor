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

		public void OnTokensReturned(IPlayer player, Color[] tokens)
		{
		}

		public void OnNobleVisit(IPlayer player, Noble noble)
		{
		}
		public void SummarizeGame(IGame game)
		{
		}
	}
}
