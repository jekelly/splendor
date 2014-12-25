namespace Splendor.ViewModel
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using GalaSoft.MvvmLight;
	using Splendor.Model;

	public class GameViewModel : ViewModelBase
	{
		private readonly IGame game;
		private readonly Task gameTask;
		private readonly ObservableCollection<Card> market;
		private readonly ObservableCollection<Noble> nobles;
		private readonly Dictionary<Color, TokenCounterViewModel> supply;
		private readonly IChooser[] choosers;

		private bool isRunning;

		public ObservableCollection<Card> Market { get { return this.market; } }

		public ObservableCollection<Noble> Nobles { get { return this.nobles; } }

		public ICommand StepCommand { get; private set; }

		public PlayerViewModel MainPlayer { get; private set; }

		public IEnumerable<PlayerViewModel> OtherPlayers { get; private set; }

		public IEnumerable<TokenCounterViewModel> TokenSupply { get { return this.supply.Values; } }

		public GameViewModel(GameService gameService, EventService eventService, CommandService commandService)
		{
			this.game = gameService.CreateGame();

			this.choosers = new IChooser[2];
			this.choosers[0] = new HumanChooser(commandService);
			this.choosers[1] = new Splendor.Model.AI.IanMStrategy(1);

			this.MainPlayer = new PlayerViewModel(this.game.Players[0], eventService);
			this.OtherPlayers = this.game.Players.Skip(1).Select(player => new PlayerViewModel(player, eventService));

			this.supply = Colors.All.ToDictionary(color => color, color => new TokenCounterViewModel(-1, color, () => game.Supply(color)));

			this.market = new ObservableCollection<Card>();
			this.RefreshMarket();
			this.nobles = new ObservableCollection<Noble>();
			this.RefreshNobles();

			this.gameTask = this.RunGameAsync();

			eventService.CardBuilt += this.OnCardBuilt;
			eventService.CardReserved += this.OnCardReserved;
			eventService.TokenReturned += this.OnTokenReturned;
			eventService.TokenTaken += this.OnTokenTaken;
			eventService.NobleVisited += this.OnNobleVisited;
		}

		private void OnNobleVisited(object sender, NobleEventArgs e)
		{
			this.RefreshNobles();
		}

		private void OnCardBuilt(object sender, CardEventArgs e)
		{
			// TODO: is this check worth it?
			if (this.market.Contains(e.Card))
			{
				this.RefreshMarket();
			}
		}

		private void OnCardReserved(object sender, CardEventArgs e)
		{
			// TODO: is this check worth it?
			if (this.market.Contains(e.Card))
			{
				this.RefreshMarket();
			}
		}

		private void OnTokenReturned(object sender, TokenEventArgs e)
		{
			this.supply[e.Color].Refresh();
		}

		private void OnTokenTaken(object sender, TokenEventArgs e)
		{
			this.supply[e.Color].Refresh();
		}

		private async Task RunGameAsync()
		{
			if(this.isRunning)
			{
				return;
			}
			while (this.game.CurrentPhase != Phase.GameOver)
			{
				var action = await Task.Run(() => this.choosers[this.game.CurrentPlayerIndex].Choose(this.game));
				this.game.Step(action);
			}
			this.isRunning = false;
		}

		private void RefreshNobles()
		{
			HashSet<Noble> toRemove = new HashSet<Noble>(this.nobles);
			foreach(var noble in this.game.Nobles)
			{
				if(toRemove.Contains(noble))
				{
					toRemove.Remove(noble);
				}
				else
				{
					this.nobles.Add(noble);
				}
			}
			foreach(var noble in toRemove)
			{
				this.nobles.Remove(noble);
			}
		}

		private void RefreshMarket()
		{
			HashSet<Card> toRemove = new HashSet<Card>(this.market);
			Queue<Card> toAdd = new Queue<Card>();
			foreach (Card card in this.game.Market)
			{
				if (toRemove.Contains(card))
				{
					toRemove.Remove(card);
				}
				else if (card.id != 0) // check for sentinel
				{
					toAdd.Enqueue(card);
				}
			}
			foreach (Card card in toRemove.OrderBy(card => card.tier).ThenBy(card => card.id))
			{
				int index = this.market.IndexOf(card);
				this.market.RemoveAt(index);
				if (toAdd.Any())
				{
					this.market.Insert(index, toAdd.Dequeue());
				}
			}
			foreach (Card card in toAdd.OrderBy(card => card.tier).ThenBy(card => card.id))
			{
				this.market.Add(card);
			}
		}
	}
}
