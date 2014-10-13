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
			throw new NotImplementedException();
		}

		public void Execute(IGame game)
		{
			throw new NotImplementedException();
		}
	}
}
