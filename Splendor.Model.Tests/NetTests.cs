namespace Splendor.Model.Tests
{
	using System;
	using System.Linq;
	using FluentAssertions;
	using Splendor.Model.AI;
	using Xunit;
	using Xunit.Extensions;

	public class NetTests
	{
		class DoubleSensor : ISensor<double>
		{
			public int DimensionCount
			{
				get { return 1; }
			}

			public string[] Descriptions
			{
				get { return new string[] { "double" }; }
			}

			public double[] Sense(double environment)
			{
				return new double[] { environment, 1.0 };
			}
		}

		[Fact]
		public void CanLearnSine()
		{
			this.ValidateSingleVariableFunction(d => Math.Sin(d), d => 0.5 * (d + 1.0), -0.5 * Math.PI, 0.5 * Math.PI);
		}

		[Fact]
		public void CanLearnCosine()
		{
			this.ValidateSingleVariableFunction(d => Math.Cos(d), d => 0.5 * (d + 1.0), 0, Math.PI);
		}

		[Fact]
		public void CanLearnPolynomial()
		{
			this.ValidateSingleVariableFunction(d => d * d, d => (d / (10 * 10)), -10, 10);
		}


		private void ValidateSingleVariableFunction(Func<double, double> func, Func<double, double> normalize, double inputMin, double inputMax)
		{
			Random r = new Random(0);
			Net<double> net = new Net<double>(new DoubleSensor(), 6);
			net.Alpha = 0.35;
			net.Beta = 0.35;
			for (int i = 0; i < 150000; i++)
			{
				var input = (r.NextDouble() * (inputMax - inputMin)) + inputMin;
				var expected = func(input);
				var normalizedResult = normalize(expected);
				net.BPLearn(input, normalizedResult, NullEventSink.Instance);
			}
			for (double d = inputMin; d <= inputMax; d += 0.1)
			{
				net.Eval(d, NullEventSink.Instance).Should().BeApproximately(normalize(func(d)), 0.1);
			}
		}
	}
}
