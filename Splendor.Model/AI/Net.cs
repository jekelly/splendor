﻿namespace Splendor.Model.AI
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using PCLStorage;

	interface IWeightStorage
	{
		void SaveWeights(double[] weights);
		double[] LoadWeights();
	}

	class Net<T>
	{
		private ISensor<T> sensor;
		private readonly int hiddenUnitCount;
		private readonly int inputSize;

		//private const double L1_LEARNING_RATE = 0.05;	// alpha, scales learning in the input->hidden layer
		//private const double L2_LEARNING_RATE = 0.05;	// beta, scale learning in the hidden->output layer
		private const double DISCOUNT_RATE = 0.9;		// "gamma", controls the importance of future rewards
		private const double TRACE_DECAY = 0.4;		// "lambda", controls the degree to which previous weight values are updated

		// network weights
		private readonly double[][] w1;
		private readonly double[] w2;

		// eligibility traces (TD)
		private readonly double[][] e1;
		private readonly double[] e2;

		// internal tracking
		private int trainingIteration;

		public double Alpha { get; set; }
		public double Beta { get; set; }

		public ISensor<T> Sensor { get { return this.sensor; } set { if (this.inputSize != value.DimensionCount) throw new InvalidOperationException();  this.sensor = value; } }

		public Net(ISensor<T> sensor, int hiddenUnitCount)
		{
			this.sensor = sensor;
			this.hiddenUnitCount = hiddenUnitCount;
			this.inputSize = sensor.DimensionCount;
			this.w1 = new double[this.inputSize + 1][];

			for (int w = 0; w <= this.inputSize; w++)
			{
				this.w1[w] = new double[this.hiddenUnitCount + 1];
			}
			this.e1 = new double[this.inputSize + 1][];
			for (int w = 0; w <= this.inputSize; w++)
			{
				this.e1[w] = new double[this.hiddenUnitCount + 1];
			}

			this.w2 = new double[this.hiddenUnitCount + 1];
			this.e2 = new double[this.hiddenUnitCount + 1];

			this.trainingIteration = 0;

			this.LoadWeights();
		}

		private void SaveWeights()
		{
			string filename = string.Format("AI\\2p_{0}h_weights.txt", this.hiddenUnitCount);
			var folder = FileSystem.Current.GetFolderFromPathAsync(".").Result;
			var file = folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting).Result;
			using (StreamWriter w = new StreamWriter(file.OpenAsync(FileAccess.ReadAndWrite).Result))
			{
				for (int j = 0; j < this.w1[0].Length; j++)
				{
					for (int i = 0; i < this.w1.Length; i++)
					{
						w.WriteLine(this.w1[i][j].ToString() + " //" + this.sensor.Descriptions[i] + " to H" + j);
					}
				}
				for (int i = 0; i < this.w2.Length; i++)
				{
					w.WriteLine(this.w2[i].ToString());
				}
			}
			var summary = folder.CreateFileAsync("AI\\summary" + this.trainingIteration + ".txt", CreationCollisionOption.ReplaceExisting).Result;
			using (StreamWriter w = new StreamWriter(summary.OpenAsync(FileAccess.ReadAndWrite).Result))
			{
				for (int j = 0; j < this.hiddenUnitCount; j++)
				{
					double[] weights = new double[this.w1.Length];
					for (int i = 0; i < this.inputSize + 1; i++)
					{
						weights[i] = this.w1[i][j];
					}
					var topFive = weights.Select((v, i) => new { Value = v, Index = i }).OrderByDescending(x => x.Value).Take(5);
					var bottomFive = weights.Select((v, i) => new { Value = v, Index = i }).OrderBy(x => x.Value).Take(5);
					w.WriteLine(string.Format("Summary of H{0} top weights", j));
					foreach (var x in topFive)
					{
						w.WriteLine("\t{0}: {1}", this.sensor.Descriptions[x.Index], x.Value);
					}
					w.WriteLine(string.Format("Summary of H{0} smallest weights", j));
					foreach (var x in bottomFive)
					{
						w.WriteLine("\t{0}: {1}", this.sensor.Descriptions[x.Index], x.Value);
					}
				}
			}
		}

		private void LoadWeights()
		{
			// try to load from file
			string filename = string.Format("AI\\2p_{0}h_weights.txt", this.hiddenUnitCount);
			var weights = FileSystem.Current.GetFileFromPathAsync(filename).Result;
			if (weights != null)
			{
				using (StringReader sr = new StringReader(weights.ReadAllTextAsync().Result))
				{
					for (int j = 0; j < this.w1[0].Length; j++)
					{
						for (int i = 0; i < this.w1.Length; i++)
						{
							string d = sr.ReadLine().Split(' ')[0];
							this.w1[i][j] = double.Parse(d);
						}
					}
					for (int i = 0; i < this.w2.Length; i++)
					{
						string d = sr.ReadLine().Split(' ')[0];
						this.w2[i] = double.Parse(d);
					}
				}
				return;
			}
			Random r = new Random();
			for (int i = 0; i < this.w1.Length; i++)
			{
				for (int j = 0; j < this.w1[i].Length; j++)
				{
					this.w1[i][j] = GenerateInitialWeight(this.inputSize, r);
				}
			}
			for (int i = 0; i < this.w2.Length; i++)
			{
				this.w2[i] = GenerateInitialWeight(this.inputSize, r);
			}
		}

		private static double GenerateInitialWeight(int inputs, Random r)
		{
			// normalize between -1 / sqrt(inputs) and 1 /sqrt(inputs)
			//double x = 1.0d / Math.Sqrt(inputs);
			//return r.NextDouble() - 0.5d;
			return r.NextGaussian(0.0, 0.1);
		}

		public double Eval(T environment, IEventSink eventSink)
		{
			return this.Eval(environment, updateEligibilityTraces: false, eventSink: eventSink ?? NullEventSink.Instance);
		}

		private double Eval(T environment, bool updateEligibilityTraces, IEventSink eventSink)
		{
			double[] inputs = this.sensor.Sense(environment);
			double[] hiddenOutput = new double[this.hiddenUnitCount + 1];

			hiddenOutput[this.hiddenUnitCount] = 1.0; // set hidden layer bias
			Debug.Assert(inputs.Length == this.inputSize + 1);
			Debug.Assert(inputs[inputs.Length - 1] == 1.0);

			// for each hidden unit, calculate it's output from the input
			//for (int h = 0; h < this.hiddenUnitCount; h++)
			Parallel.For(0, this.hiddenUnitCount, (h) =>
			{
				//Parallel.For(0, this.inputSize + 1, (i) => hiddenOutput[h] += inputs[i] * this.w1[i][h]);
				for (int i = 0; i <= this.inputSize; i++)
				{
					hiddenOutput[h] += inputs[i] * this.w1[i][h];
				}
				hiddenOutput[h] = sigmoid(hiddenOutput[h]);
			});
			double output = 0;
			// for each hidden output, accumulate to the final result
			for (int h = 0; h <= this.hiddenUnitCount; h++)
			{
				output += this.w2[h] * hiddenOutput[h];
			}
			output = sigmoid(output);

			if (updateEligibilityTraces)
			{
				this.UpdateEligibilityTraces(output, inputs, hiddenOutput);
			}

			return output;
		}

		private static double sigmoid(double input)
		{
			// special logrithm
			return (1.0d / (1.0d + Math.Exp(-input)));
			// tanh
			//return Math.Tanh(input);
		}

		private static double dsigmoid(double sigmoid)
		{
			return sigmoid * (1.0 - sigmoid);
			// return (1.0 - output * output);
		}

		// assumes pure sigmoid function; need a different derivative calculation otherwise
		private void UpdateEligibilityTraces(double output, double[] input, double[] h)
		{
			double temp = dsigmoid(output);

			for (int j = 0; j <= this.hiddenUnitCount; j++)
			{
				this.e2[j] = TRACE_DECAY * this.e2[j] + temp * h[j];
				double tempH = dsigmoid(h[j]) * this.w2[j];
				for (int i = 0; i <= this.inputSize; i++)
				{
					this.e1[i][j] = TRACE_DECAY * this.e1[i][j] + temp * tempH * input[i];
				}
			}
		}

		private void UpdateWeights(double error)
		{
			double total1 = 0, total2 = 0;
			for (int j = 0; j <= this.hiddenUnitCount; j++)
			{
				double d2 = this.Beta * error * this.e2[j];
				this.w2[j] += d2;
				total2 += Math.Abs(d2);
				for (int i = 0; i <= this.inputSize; i++)
				{
					double d1 = this.Alpha * error * this.e1[i][j];
					this.w1[i][j] += d1;
					total1 += Math.Abs(d1);
				}
			}
		}

		public void BPLearn(T input, double target, IEventSink eventSink)
		{
			double output = this.Eval(input, true, eventSink);
			double error = target - output;
			this.UpdateWeights(error);
			eventSink.DebugMessage("Error: {0}", error);
		}

		public void Learn(T[] states, double finalReward, IEventSink eventSink)
		{
			double[] output = new double[states.Length];
			// initialize training storage
			for (int i = 0; i < this.e1.Length; i++)
			{
				Array.Clear(this.e1[i], 0, this.e1[i].Length);
			}
			Array.Clear(this.e2, 0, this.e2.Length);
			output[0] = this.Eval(states[0], updateEligibilityTraces: true, eventSink: NullEventSink.Instance);
			//UpdateElig
			for (int t = 1; t < states.Length; t++)
			{
				// evaluate Y[t]
				output[t] = this.Eval(states[t], updateEligibilityTraces: false, eventSink: NullEventSink.Instance);
				eventSink.DebugMessage("Eval 1 at time step {0}: {1}", t, output[t]);
				// TODO: inefficient, could only store current/last instead of all
				double error = 0.0;
				// error = (y[t] + lambda * Y[t] - Y[t-1]). no y[t] measured until the last state.
				eventSink.DebugMessage("Found error {0} at time step {1}", error, t);
				eventSink.DebugMessage("Found error {0} at time step {1}", error, t);
				this.UpdateWeights(error);
				// reevaluate Y[t] (soon to be Y[t-1]) with new weights
				output[t] = this.Eval(states[t], updateEligibilityTraces: true, eventSink: NullEventSink.Instance);
				eventSink.DebugMessage("Eval 2 at time step {0}: {1}", t, output[t]);
			}
			// after last observed state, we know if we actually won or lost, so do one last update
			double finalError = finalReward - output[states.Length - 1];
			eventSink.DebugMessage("Final error: {0}", finalError);
			this.UpdateWeights(finalError);
			if (++this.trainingIteration % 100 == 0)
			{
				this.SaveWeights();
			}
		}
	}
}
