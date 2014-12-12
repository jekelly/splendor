namespace Splendor.ViewModel
{
	using Splendor.Model;

	public class GameService
	{
		private readonly EventService eventService;

		public GameService(EventService eventService)
		{
			this.eventService = eventService;
		}

		public IGame CreateGame()
		{
			return new Game(Setups.All[0], null, this.eventService.CreateEventSink());
		}
	}
}
