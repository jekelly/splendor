namespace Splendor.Model
{
	public interface IChooser
	{
		IAction Choose(IGame state);

		void PostGame(int winner, IEventSink eventSink);
	}
}
