namespace Splendor.ViewModel
{
	using Splendor.Model;
	class ReserveCardCommand : ActionCommand<Card>
	{
		public ReserveCardCommand(CommandService commandService)
			: base(commandService)
		{ }

		protected override IAction GetActionFromParameter(Card parameter)
		{
			return Rules.ReserveActions[parameter.id];
		}
	}
}
