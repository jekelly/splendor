namespace Splendor.AI
{
	using System.Linq;
	using Splendor.Model;

	class FirstChooser : IChooser
	{
		public IAction Choose(IGame state)
		{
			return state.AvailableActions.First();
		}

		public void PostGame(int winner, IEventSink eventSink)
		{
		}
	}
}
