using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public interface IRandomizer
	{
		int Next(int max);
	}
}
