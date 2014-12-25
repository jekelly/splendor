namespace Splendor.ViewModel
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Splendor.Model;

	class HumanChooser : IChooser
	{
		private readonly CommandService commandService;
		private readonly IAction replaceGoldAction = Rules.ReplaceActions[(int)Color.Gold];

		public HumanChooser(CommandService commandService)
		{
			this.commandService = commandService;
		}

		public IAction Choose(IGame state)
		{
			if (!state.AvailableActions.Any())
			{
				return null;
			}
			// TODO: make this a user-configurable setting
			if (state.CurrentPhase == Phase.Pay)
			{
				var action = state.AvailableActions.First();
				Debug.Assert(action != replaceGoldAction || state.AvailableActions.Count() == 1);
				return action;
			}
			return this.commandService.GetActionAsync(state).Result;
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}
	}
}
