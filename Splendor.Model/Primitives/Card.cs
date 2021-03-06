﻿namespace Splendor.Model
{
	using System;
	using System.Linq;
	using System.Runtime.InteropServices;

	public static class Colors
	{
		private static readonly Color[] verboseColors = Enum.GetValues(typeof(Color)).OfType<Color>().ToArray();
		private static readonly Color[] cardinalColors = verboseColors.Where(c => c != Color.Gold).ToArray();
		private static readonly char[] shortColors = new char[] { 'W', 'U', 'G', 'R', 'B', 'A' };

		public static Color[] All { get { return verboseColors; } }

		public static Color[] CardinalColors { get { return cardinalColors; } }

		public static string Verbose(byte index)
		{
			return verboseColors[index].ToString();
		}

		public static char Short(byte index)
		{
			return shortColors[index];
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Card
	{
		public byte id;
		public byte tier;
		public byte costWhite;
		public byte costBlue;
		public byte costGreen;
		public byte costRed;
		public byte costBlack;
		public byte value;
		public byte gives;

		public int Value { get { return this.value; } }
		public Color Gives { get { return (Color)this.gives; } }

		public int Cost(Color color)
		{
			switch (color)
			{
				case Color.White:
					return this.costWhite;
				case Color.Blue:
					return this.costBlue;
				case Color.Green:
					return this.costGreen;
				case Color.Red:
					return this.costRed;
				case Color.Black:
					return this.costBlack;
				default:
					throw new InvalidOperationException();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Card)
			{
				return ((Card)obj).id == this.id;
			}
			return false;
		}

		public override string ToString()
		{
			// [value] (gives) [xW] [xU] [xG] [xR] [xB]"
			string white = this.costWhite > 0 ? this.costWhite + "W " : string.Empty;
			string blue = this.costBlue > 0 ? this.costBlue + "U " : string.Empty;
			string green = this.costGreen > 0 ? this.costGreen + "G " : string.Empty;
			string red = this.costRed > 0 ? this.costRed + "R " : string.Empty;
			string black = this.costBlack > 0 ? this.costBlack + "B" : string.Empty;
			return string.Format("{7} [{0}] ({1}) {2}{3}{4}{5}{6}", this.value, Colors.Short(this.gives), white, blue, green, red, black, string.Join("", Enumerable.Repeat('.', this.tier + 1)));
		}

		public bool CanBuy(int[] resources)
		{
			int needed = Math.Max(this.costBlack - resources[(int)Color.Black], 0) +
				Math.Max(this.costBlue - resources[(int)Color.Blue], 0) +
				Math.Max(this.costGreen - resources[(int)Color.Green], 0) +
				Math.Max(this.costRed - resources[(int)Color.Red], 0) +
				Math.Max(this.costWhite - resources[(int)Color.White], 0);
			return needed <= resources[(int)Color.Gold];
		}
	}
}
