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
			return game.CurrentPhase == Phase.Choose && game.Market.Contains(this.card) && game.CurrentPlayer.Hand.Count() < 3;
		}

		public void Execute(IGame game)
		{
			game.CurrentPlayer.MoveCardToHand(this.card);
		}
	}
}
