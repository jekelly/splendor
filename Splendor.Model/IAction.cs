using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public interface IAction
	{
		void Execute(IGame game);
		bool CanExecute(IGame game);
	}
}
