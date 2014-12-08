namespace Splendor.Model
{
	using System;

	public class Card
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
