namespace Splendor.ViewModel
{
	using Splendor.Model;

	public class NobleVisitCommand : ActionCommand<Noble>
	{
		public NobleVisitCommand(CommandService commandService)
			: base(commandService)
		{ }

		protected override IAction GetActionFromParameter(Noble parameter)
		{
			return Rules.NobleActions[parameter.id];
		}
	}
}
