namespace Splendor.Model.Tests
{
	using System;
	using System.Linq;
	using FluentAssertions;
	using Xunit;
	using Xunit.Extensions;

	public class RandomExtensionsTests
	{
		[Theory]
		[InlineData(10000, 0.0, 0.01)]
		[InlineData(10000, 0.0, 0.01)]
		public void TestBoxMuller(int iterations, double mean, double stddev)
		{
			Random r = new Random(0);
			double[] results = new double[iterations];
			for (int i = 0; i < iterations; i++)
			{
				results[i] = r.NextGaussian(mean, stddev);
			}
			var avg = results.Average();
			avg.Should().BeApproximately(mean, 0.01);
		}
	}
}
