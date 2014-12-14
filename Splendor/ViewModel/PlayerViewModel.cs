namespace Splendor.ViewModel
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using GalaSoft.MvvmLight;
	using Splendor.Model;

	public class PlayerViewModel : ViewModelBase
	{
		private int score;
		private readonly IPlayer player;
		private readonly Dictionary<Color, TokenCounterViewModel> tokens;
		private readonly Dictionary<Color, TokenCounterViewModel> gems;
		private readonly ObservableCollection<Card> hand;

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

		public PlayerViewModel(IPlayer player, EventService eventService)
		{
			this.player = player;

			this.Name = string.Format("Player {0}", player.Index);
			this.tokens = Colors.All.ToDictionary(color => color, color => new TokenCounterViewModel(color, () => player.Tokens(color)));
			this.gems = Colors.CardinalColors.ToDictionary(color => color, color => new TokenCounterViewModel(color, () => player.Gems(color)));
			this.hand = new ObservableCollection<Card>();

			eventService.TokenTaken += this.OnTokenTaken;
			eventService.TokenReturned += this.OnTokenReturned;
			eventService.CardBuilt += this.OnCardBuilt;
			eventService.CardReserved += this.OnCardReserved;
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
