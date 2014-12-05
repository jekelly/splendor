using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	class ReserveCardAction : IAction
	{
		private readonly Card card;

		public ReserveCardAction(Card card)
		{
			this.card = card;
		}

		public bool CanExecute(IGame game)
		{
			return game.Market.Contains(this.card);
		}

		public void Execute(IGame game)
		{
			throw new NotImplementedException();
		}
	}
}
