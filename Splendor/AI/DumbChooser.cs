using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splendor.Model;

namespace Splendor
{
	class DumbChooser : IChooser
	{
		public IAction Choose(IEnumerable<IAction> actions)
		{
			return actions.First();
		}
	}
}
