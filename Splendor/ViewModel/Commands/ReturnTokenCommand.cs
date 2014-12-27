namespace Splendor.ViewModel
{
	using Splendor.Model;

	class ReturnTokenCommand : ActionCommand<Color>
	{
		public ReturnTokenCommand(CommandService commandService)
			: base(commandService)
		{ }

		protected override IAction GetActionFromParameter(Color parameter)
		{
			return Rules.ReplaceActions[(int)parameter];
		}
	}
}
