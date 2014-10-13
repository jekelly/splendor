using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public interface IGame
	{
		int[] Tokens { get; }
		Card[] Market { get; }
		Noble[] Nobles { get; }
	}
}
