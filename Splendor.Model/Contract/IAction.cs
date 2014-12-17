namespace Splendor.Model
{
	public interface IAction
	{
		void Execute(IGame game);
		bool CanExecute(IGame game);
	}
}
