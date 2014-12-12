namespace Splendor.Model
{
	using System.Linq;

	class ReserveCardAction : IAction
	{
		private readonly Card card;

		public ReserveCardAction(Card card)
		{
			this.card = card;
		}

		public bool CanExecute(IGame game)
		{
			return this.card != Rules.SentinelCard && game.CurrentPhase == Phase.Choose && game.Market.Contains(this.card) && game.CurrentPlayer.Hand.Count() < 3;
		}

		public void Execute(IGame game)
		{
			var currentPlayer = game.CurrentPlayer;
			currentPlayer.MoveCardToHand(this.card);
			game.EventSink.OnCardReserved(currentPlayer, this.card);

			// TODO need test for gaining a gold
			if (game.Supply(Color.Gold) > 0)
			{
				currentPlayer.GainToken(Color.Gold);
				game.EventSink.OnTokensTaken(currentPlayer, new Color[] { Color.Gold });
			}
		}
	}
}
