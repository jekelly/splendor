namespace Splendor.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using Splendor.Model;

	public class CommandService
	{
		private readonly ManualResetEventSlim actionAvailable;
		private readonly List<IRefreshable> refreshables;
		private readonly ActionCommand<Card> buildCardCommand;
		private readonly ActionCommand<Card> reserveCardCommand;
		private readonly ActionCommand<Color[]> takeTokensCommand;
		private readonly ActionCommand<Color> replaceTokenCommand;
		private readonly NobleVisitCommand nobleVisitCommand;

		private HashSet<IAction> availableActions;
		private IAction action;

		public ICommand ReserveCardCommand { get { return this.reserveCardCommand; } }
		public ICommand BuildCardCommand { get { return this.buildCardCommand; } }
		public ICommand TakeTokensCommand { get { return this.takeTokensCommand; } }
		public ICommand ReplaceTokenCommand { get { return this.replaceTokenCommand; } }
		public ICommand NobleVisitCommand { get { return this.nobleVisitCommand; } }

		public CommandService()
		{
			this.refreshables = new List<IRefreshable>();
			this.actionAvailable = new ManualResetEventSlim(false);

			this.reserveCardCommand = new ReserveCardCommand(this);
			this.buildCardCommand = new BuildCardCommand(this);
			this.takeTokensCommand = new TakeTokenCommand(this);
			this.replaceTokenCommand = new ReturnTokenCommand(this);
			this.nobleVisitCommand = new NobleVisitCommand(this);

			this.RegisterCommand(this.reserveCardCommand);
			this.RegisterCommand(this.buildCardCommand);
			this.RegisterCommand(this.takeTokensCommand);
			this.RegisterCommand(this.replaceTokenCommand);
			this.RegisterCommand(this.nobleVisitCommand);
		}

		public bool IsActionAvailable(IAction action)
		{
			if (this.availableActions == null)
			{
				return false;
			}
			return this.availableActions.Contains(action);
		}

		public void ChooseAction(IAction action)
		{
			this.action = action;
			this.actionAvailable.Set();
		}

		public async Task<IAction> GetActionAsync(IGame state)
		{
			try
			{
				this.availableActions = new HashSet<IAction>(state.AvailableActions);
				if (this.availableActions.Count == 0)
				{
					return null;
				}
				if (this.availableActions.Count == 1)
				{
					return this.availableActions.Single();
				}
				this.RefreshCommands();
				this.actionAvailable.Reset();
				return await Task.Run(() =>
				{
					actionAvailable.Wait();
					return this.action;
				});
			}
			finally
			{
				this.availableActions = null;
				this.action = null;
				this.RefreshCommands();
			}
		}

		private void RefreshCommands()
		{
			foreach (IRefreshable refreshable in this.refreshables)
			{
				refreshable.Refresh();
			}
		}
		public void RegisterCommand(IRefreshable refreshable)
		{
			this.refreshables.Add(refreshable);
		}
	}
}
