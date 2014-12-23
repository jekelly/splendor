namespace Splendor.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Splendor.Model;

	class ActionManager
	{

	}

	class HumanChooser : IChooser
	{
		private readonly CommandService commandService;

		public HumanChooser(CommandService commandService)
		{
			this.commandService = commandService;
		}

		public IAction Choose(IGame state)
		{
			return this.commandService.GetActionAsync(state).Result;
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
		}
	}
}
