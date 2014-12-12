using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Splendor.Model;

namespace Splendor.ViewModel
{
	public class GameViewModel : ViewModelBase
	{
		private readonly IGame game;
		private readonly ObservableCollection<Card> market;
		private readonly Dictionary<Color, TokenCounterViewModel> supply;

		public ObservableCollection<Card> Market { get { return this.market; } }

		public ICommand StepCommand { get; private set; }

		public IEnumerable<PlayerViewModel> Players { get; private set; }

		public IEnumerable<TokenCounterViewModel> TokenSupply { get { return this.supply.Values; } }

		public GameViewModel(GameService gameService, EventService eventService)
		{
			this.game = gameService.CreateGame();

			this.Players = this.game.Players.Select(player => new PlayerViewModel(player, eventService));

			this.supply = Colors.All.ToDictionary(color => color, color => new TokenCounterViewModel(color, () => game.Supply(color)));

			this.market = new ObservableCollection<Card>();
			this.RefreshMarket();

			this.StepCommand = new RelayCommand(this.Step);

			eventService.CardBuilt += this.OnCardBuilt;
			eventService.CardReserved += this.OnCardReserved;
			eventService.TokenReturned += this.OnTokenReturned;
			eventService.TokenTaken += this.OnTokenTaken;
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

		private void Step()
		{
			var dumbChooser = new DumbChooser();
			this.game.Step(dumbChooser);
		}

		private void RefreshMarket()
		{
			HashSet<Card> toRemove = new HashSet<Card>(this.market);
			foreach (Card card in this.game.Market)
			{
				if (toRemove.Contains(card))
				{
					toRemove.Remove(card);
				}
				else
				{
					this.market.Add(card);
				}
			}
			foreach (Card card in toRemove)
			{
				this.market.Remove(card);
			}
		}
	}
}
