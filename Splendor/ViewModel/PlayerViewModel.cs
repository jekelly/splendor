namespace Splendor.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Command;
	using GalaSoft.MvvmLight.Ioc;
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

	public interface IRefreshable
	{
		void Refresh();
	}

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

	class TakeTokenCommand : ActionCommand<Color[]>
	{
		public TakeTokenCommand(CommandService commandService)
			: base(commandService)
		{ }

		protected override IAction GetActionFromParameter(Color[] parameter)
		{
			if (parameter[0] == parameter[1])
			{
				return Rules.TakeActions[10 + (int)parameter[0]];
			}
			// assume parameters come in ordered
			int a = (int)parameter[0]; int b = (int)parameter[1]; int c = (int)parameter[2];
			if (a == 0 && b == 1 && c == 2) return Rules.TakeActions[0];
			else if (a == 0 && b == 1 && c == 3) return Rules.TakeActions[1];
			else if (a == 0 && b == 1 && c == 4) return Rules.TakeActions[2];
			else if (a == 0 && b == 2 && c == 3) return Rules.TakeActions[3];
			else if (a == 0 && b == 2 && c == 4) return Rules.TakeActions[4];
			else if (a == 0 && b == 3 && c == 4) return Rules.TakeActions[5];
			else if (a == 1 && b == 2 && c == 3) return Rules.TakeActions[6];
			else if (a == 1 && b == 2 && c == 4) return Rules.TakeActions[7];
			else if (a == 1 && b == 3 && c == 4) return Rules.TakeActions[8];
			else if (a == 2 && b == 3 && c == 4) return Rules.TakeActions[9];
			throw new InvalidOperationException();
		}
	}

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

	public class PlayerViewModel : ViewModelBase
	{
		private int score;
		private readonly IPlayer player;
		private readonly Dictionary<Color, TokenCounterViewModel> tokens;
		private readonly Dictionary<Color, TokenCounterViewModel> gems;
		private readonly ObservableCollection<Card> hand;
		private readonly ObservableCollection<Noble> nobles;

		public string Name { get; private set; }

		public int Score
		{
			get { return this.score; }
			set
			{
				if (this.score != value)
				{
					this.score = value;
					this.RaisePropertyChanged("Score");
				}
			}
		}

		public IEnumerable<TokenCounterViewModel> Tokens { get { return this.tokens.Values; } }
		public IEnumerable<TokenCounterViewModel> Gems { get { return this.gems.Values; } }
		public ObservableCollection<Card> Hand { get { return this.hand; } }
		public ObservableCollection<Noble> Nobles { get { return this.nobles; } }

		public PlayerViewModel(IPlayer player, EventService eventService)
		{
			this.player = player;

			this.Name = string.Format("Player {0}", player.Index);
			this.tokens = Colors.All.ToDictionary(color => color, color => new TokenCounterViewModel(this.player.Index, color, () => player.Tokens(color)));
			this.gems = Colors.CardinalColors.ToDictionary(color => color, color => new TokenCounterViewModel(this.player.Index, color, () => player.Gems(color)));
			this.hand = new ObservableCollection<Card>();
			this.nobles = new ObservableCollection<Noble>();

			eventService.TokenTaken += this.OnTokenTaken;
			eventService.TokenReturned += this.OnTokenReturned;
			eventService.CardBuilt += this.OnCardBuilt;
			eventService.CardReserved += this.OnCardReserved;
			eventService.NobleVisited += this.OnNobleVisited;
		}

		private void OnNobleVisited(object sender, NobleEventArgs e)
		{
			if (this.player.Nobles.Contains(e.Noble))
			{
				this.nobles.Add(e.Noble);
				this.Score = this.player.Score;
			}
		}

		private void OnTokenTaken(object sender, TokenEventArgs e)
		{
			this.tokens[e.Color].Refresh();
		}

		private void OnTokenReturned(object sender, TokenEventArgs e)
		{
			this.tokens[e.Color].Refresh();
		}

		private void OnCardReserved(object sender, CardEventArgs e)
		{
			if (e.Player == this.player)
			{
				this.UpdateTokens();
				this.hand.Add(e.Card);
			}
		}

		private void OnCardBuilt(object sender, CardEventArgs e)
		{
			if (e.Player == this.player)
			{
				this.hand.Remove(e.Card);
				this.gems[(Color)e.Card.gives].Refresh();
				this.Score = player.Score;
			}
		}

		private void UpdateTokens()
		{
			foreach (var token in this.Tokens)
			{
				token.Refresh();
			}
		}
	}
}
