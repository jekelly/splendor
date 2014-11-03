using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public class Randomizer : IRandomizer
	{
		private readonly Random random;

		public Randomizer()
		{
			this.random = new Random();
		}

		public Randomizer(int seed)
		{
			this.random = new Random(seed);
		}

		public int Next(int max)
		{
			return this.random.Next(max);
		}
	}
}
