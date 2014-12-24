namespace Splendor.Model
{
	using System;

	public static class RandomExtensions
	{
		private static double nextGaussian = double.NaN;

		public static double NextGaussian(this Random rand, double mean, double stdDev)
		{
			double v = 0;
			if (!double.IsNaN(nextGaussian))
			{
				v = mean + (nextGaussian * stdDev);
				nextGaussian = double.NaN;
			}
			else
			{
				// box-muller
				double u1 = rand.NextDouble();
				double u2 = rand.NextDouble();
				double a = Math.Sqrt(-2.0 * Math.Log(u1));
				double b = 2.0 * Math.PI * u2;
				double normal = a * Math.Sin(b);
				v = mean + (stdDev * normal);
				nextGaussian = a * Math.Cos(b);
			}
			return v;
		}
	}
}
