namespace Splendor.ViewModel
{
	using System;
	using System.Windows.Input;
	using Splendor.Model;

	public abstract class ActionCommand<T> : ICommand, IRefreshable
	{
		private readonly CommandService commandService;

		public ActionCommand(CommandService commandService)
		{
			this.commandService = commandService;
		}

		public bool CanExecute(object parameter)
		{
			if (parameter == null) return false;
			T typedParameter = (T)parameter;
			IAction action = this.GetActionFromParameter(typedParameter);
			return this.commandService.IsActionAvailable(action);
		}

		protected abstract IAction GetActionFromParameter(T parameter);

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			T typedParameter = (T)parameter;
			IAction action = this.GetActionFromParameter(typedParameter);
			this.commandService.ChooseAction(action);
		}

		public void Refresh()
		{
			if (this.CanExecuteChanged != null)
			{
				this.CanExecuteChanged(this, EventArgs.Empty);
			}
		}
	}
}
