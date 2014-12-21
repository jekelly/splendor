using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model.AI
{
	interface ISensor<T>
	{
		int DimensionCount { get; }
		string[] Descriptions { get; }
		double[] Sense(T environment);
	}
}
