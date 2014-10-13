using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	interface IPlayer
	{
		int[] Tokens { get; }
		Card[] Hand { get; }
		Card[] Tableau { get; }
	}
}
