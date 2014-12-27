namespace Splendor.ViewModel
{
	using Splendor.Model;

	class BuildCardCommand : ActionCommand<Card>
	{
		public BuildCardCommand(CommandService commandService)
			: base(commandService)
		{ }

		protected override IAction GetActionFromParameter(Card parameter)
		{
			return Rules.BuildActions[parameter.id];
		}
	}
}
