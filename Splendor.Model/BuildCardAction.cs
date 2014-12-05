using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	class BuildCardAction : IAction
	{
		private readonly Card card;

		public BuildCardAction(Card card)
		{
			this.card = card;
		}

		public bool CanExecute(IGame game)
		{
			bool available = game.Market.Contains(this.card) || game.CurrentPlayer.Hand.Contains(this.card);
			bool buildable = this.card.CanBuy(game.CurrentPlayer.BuyingPower);
			return available && buildable;
		}

		public void Execute(IGame game)
		{
			throw new NotImplementedException();
		}
	}
}
