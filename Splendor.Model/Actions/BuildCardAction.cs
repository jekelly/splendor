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
				(game.Market.Contains(this.card) || game.CurrentPlayer.Hand.Contains(this.card)) &&
				this.card.CanBuy(this.BuyingPower(game));
		}

		public void Execute(IGame game)
		{
			game.CurrentPlayer.MoveCardToTableau(this.card);
			game.EventSink.OnCardBuild(game.CurrentPlayer, this.card);
		}

		private int[] BuyingPower(IGame game)
		{
			int[] power = new int[6];
			for (int i = 0; i < 5; i++)
			{
				Color c = (Color)i;
				power[i] = game.CurrentPlayer.Gems(c) + game.CurrentPlayer.Tokens(c);
			}
			power[(int)Color.Gold] = game.CurrentPlayer.Tokens(Color.Gold);
			return power;
		}

		public override string ToString()
		{
			return string.Format("Build {0}", this.card);
		}
	}
}
