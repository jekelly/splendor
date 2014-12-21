namespace Splendor.Model
{
	using System.Linq;

	class BuildCardAction : IAction
	{
		private readonly Card card;

		public BuildCardAction(Card card)
		{
			this.card = card;
		}

		public bool CanExecute(IGame game)
		{
			return game.CurrentPhase == Phase.Choose &&
				game.CurrentPlayer.Tableau.Count() < Rules.MaxTableauSize &&
				(game.Market.Contains(this.card) || game.CurrentPlayer.Hand.Contains(this.card)) &&
				this.card.CanBuy(game.CurrentPlayer.BuyingPower);
		}

		public void Execute(IGame game)
		{
			game.CurrentPlayer.MoveCardToTableau(this.card);
			game.EventSink.OnCardBuild(game.CurrentPlayer, this.card);
		}

		public override string ToString()
		{
			return string.Format("Build {0}", this.card);
		}
	}
}
