namespace Splendor.ViewModel
{
	using System;
	using Splendor.Model;

	public class EventService
	{
		public event EventHandler<TokenEventArgs> TokenTaken;
		public event EventHandler<TokenEventArgs> TokenReturned;
		public event EventHandler<CardEventArgs> CardBuilt;
		public event EventHandler<CardEventArgs> CardReserved;

		public IEventSink CreateEventSink()
		{
			return new EventSink(this);
		}

		class EventSink : IEventSink
		{
			private readonly EventService eventService;

			public EventSink(EventService eventService)
			{
				this.eventService = eventService;
			}

			public void OnCardBuild(IPlayer player, Card card)
			{
				if (this.eventService.CardBuilt != null)
				{
					this.eventService.CardBuilt(this, new CardEventArgs(player, card));
				}
			}

			public void OnCardReserved(IPlayer player, Card card)
			{
				if (this.eventService.CardReserved != null)
				{
					this.eventService.CardReserved(this, new CardEventArgs(player, card));
				}
			}

			public void OnTokensTaken(IPlayer player, Color[] tokens)
			{
				foreach (Color color in tokens)
				{
					if (this.eventService.TokenTaken != null)
					{
						this.eventService.TokenTaken(this, new TokenEventArgs(color));
					}
				}
			}

			public void OnTokenReturned(IPlayer player, Color tokens)
			{
				if (this.eventService.TokenReturned != null)
				{
					this.eventService.TokenReturned(this, new TokenEventArgs(tokens));
				}
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
}
