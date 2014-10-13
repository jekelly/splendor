using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	class ReplaceTokenAction : IAction
	{
		private readonly Color color;

		public ReplaceTokenAction(Color color)
		{
			this.color = color;
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
