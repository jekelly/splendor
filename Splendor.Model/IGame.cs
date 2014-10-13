using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public interface IGame
	{
		IList<IAction> Actions { get; }

		int Supply(Color color);

		//int[] Tokens { get; }
		Card[] Market { get; }
		Noble[] Nobles { get; }

		int CurrentPlayer { get; }

		void GainToken(int playerIndex, Color color);
	}
}
