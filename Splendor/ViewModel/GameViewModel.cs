using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

		public PlayerViewModel MainPlayer { get; private set; }
		public IEnumerable<PlayerViewModel> OtherPlayers { get; private set; }

		public IEnumerable<TokenCounterViewModel> TokenSupply { get { return this.supply.Values; } }

		public GameViewModel(GameService gameService, EventService eventService)
		{
			this.game = gameService.CreateGame();

			this.MainPlayer = new PlayerViewModel(this.game.Players[0], eventService);
			this.OtherPlayers = this.game.Players.Skip(1).Select(player => new PlayerViewModel(player, eventService));

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
			this.game.Step(game.AvailableActions.FirstOrDefault());
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
