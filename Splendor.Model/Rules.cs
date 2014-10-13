using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public static class Rules
	{
		public const int tiers = 3;
		public const int goldCount = 5;

		public static readonly Setup[] Setups =
		{
			new Setup() { playerCount = 2, nobleCount = 3, tokenCount = 4 },
			new Setup() { playerCount = 3, nobleCount = 4, tokenCount = 5 },
			new Setup() { playerCount = 4, nobleCount = 5, tokenCount = 7 },
		};

		public static readonly Noble[] Nobles =
		{
			new Noble() { id = 0, requiresWhite = 3, requiresBlue = 3, requiresGreen = 0, requiresRed = 0, requiresBlack = 3, value = 3 },
			new Noble() { id = 0, requiresWhite = 0, requiresBlue = 3, requiresGreen = 3, requiresRed = 3, requiresBlack = 0, value = 3 },
			new Noble() { id = 0, requiresWhite = 3, requiresBlue = 0, requiresGreen = 0, requiresRed = 3, requiresBlack = 3, value = 3 },
			new Noble() { id = 0, requiresWhite = 0, requiresBlue = 0, requiresGreen = 4, requiresRed = 4, requiresBlack = 0, value = 3 },
			new Noble() { id = 0, requiresWhite = 0, requiresBlue = 4, requiresGreen = 4, requiresRed = 0, requiresBlack = 0, value = 3 },
			new Noble() { id = 0, requiresWhite = 0, requiresBlue = 0, requiresGreen = 0, requiresRed = 4, requiresBlack = 4, value = 3 },
			new Noble() { id = 0, requiresWhite = 4, requiresBlue = 0, requiresGreen = 0, requiresRed = 0, requiresBlack = 4, value = 3 },
			new Noble() { id = 0, requiresWhite = 3, requiresBlue = 3, requiresGreen = 3, requiresRed = 0, requiresBlack = 0, value = 3 },
			new Noble() { id = 0, requiresWhite = 0, requiresBlue = 0, requiresGreen = 3, requiresRed = 3, requiresBlack = 3, value = 3 },
			new Noble() { id = 0, requiresWhite = 4, requiresBlue = 4, requiresGreen = 0, requiresRed = 0, requiresBlack = 0, value = 3 },
		};

		public static readonly Card[] Cards = 
		{
			// White Tier 1
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 2, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 1, costGreen= 1, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 2, costGreen= 0, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 4, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 1, costGreen= 2, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 2, costGreen= 2, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 0, tier = 0, costWhite = 3, costBlue= 1, costGreen= 0, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.White },
			// Blue Tier 1
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 3, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 0, costGreen= 1, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 2, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 4, costBlack= 0, value= 1, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 0, costGreen= 1, costRed= 2, costBlack= 1, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 0, costGreen= 2, costRed= 2, costBlack= 0, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 1, costGreen= 3, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Blue },
			// Green Tier 1
			new Card() { id = 0, tier = 0, costWhite = 2, costBlue= 1, costGreen= 0, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 3, costBlack= 0, value= 0, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 1, costGreen= 0, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 2, costGreen= 0, costRed= 2, costBlack= 0, value= 0, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 4, value= 1, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 1, costGreen= 0, costRed= 1, costBlack= 2, value= 0, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 1, costGreen= 0, costRed= 2, costBlack= 2, value= 0, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 3, costGreen= 1, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Green },
			// Red Tier 1
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 2, costGreen= 1, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 1, costGreen= 1, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 2, costBlue= 0, costGreen= 0, costRed= 2, costBlack= 0, value= 0, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 4, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 2, costBlue= 1, costGreen= 1, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 2, costBlue= 0, costGreen= 1, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 0, costGreen= 0, costRed= 1, costBlack= 3, value= 0, gives= (byte)Color.Red },
			// Black Tier 1
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 2, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 3, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 1, costGreen= 1, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 2, costBlue= 0, costGreen= 2, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 4, costGreen= 0, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 1, costBlue= 2, costGreen= 1, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 2, costBlue= 2, costGreen= 0, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 0, costWhite = 0, costBlue= 0, costGreen= 1, costRed= 3, costBlack= 1, value= 0, gives= (byte)Color.Black },
			// White Tier 2
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 5, costBlack= 0, value= 2, gives= (byte)Color.White },
			new Card() { id = 0, tier = 1, costWhite = 6, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 3, gives= (byte)Color.White },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 3, costRed= 2, costBlack= 2, value= 1, gives= (byte)Color.White },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 1, costRed= 4, costBlack= 2, value= 2, gives= (byte)Color.White },
			new Card() { id = 0, tier = 1, costWhite = 2, costBlue= 3, costGreen= 0, costRed= 3, costBlack= 0, value= 1, gives= (byte)Color.White },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 5, costBlack= 3, value= 2, gives= (byte)Color.White },
			// Blue Tier 2
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 5, costGreen= 0, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 6, costGreen= 0, costRed= 0, costBlack= 0, value= 3, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 2, costGreen= 2, costRed= 3, costBlack= 0, value= 1, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 1, costWhite = 2, costBlue= 0, costGreen= 0, costRed= 1, costBlack= 4, value= 2, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 1, costWhite = 5, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Blue },
			// Green Tier 2
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 5, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 6, costRed= 0, costBlack= 0, value= 3, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 1, costWhite = 2, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 2, value= 1, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 1, costWhite = 3, costBlue= 0, costGreen= 2, costRed= 2, costBlack= 0, value= 1, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 1, costWhite = 4, costBlue= 2, costGreen= 0, costRed= 0, costBlack= 1, value= 2, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 5, costGreen= 3, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Green },
			// Red Tier 2
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 5, value= 2, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 6, costBlack= 0, value= 3, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 1, costWhite = 2, costBlue= 0, costGreen= 0, costRed= 2, costBlack= 3, value= 1, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 1, costWhite = 1, costBlue= 4, costGreen= 2, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 3, costGreen= 0, costRed= 2, costBlack= 3, value= 1, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 1, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 5, value= 2, gives= (byte)Color.Red },
			// Black Tier 2
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 5, value= 2, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 6, value= 3, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 1, costWhite = 3, costBlue= 2, costGreen= 2, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 1, costGreen= 4, costRed= 2, costBlack= 0, value= 2, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 1, costWhite = 3, costBlue= 0, costGreen= 3, costRed= 0, costBlack= 2, value= 1, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 1, costWhite = 0, costBlue= 0, costGreen= 5, costRed= 3, costBlack= 0, value= 2, gives= (byte)Color.Black },
			// White Tier 3
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 7, value= 4, gives= (byte)Color.White },
			new Card() { id = 0, tier = 2, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 7, value= 5, gives= (byte)Color.White },
			new Card() { id = 0, tier = 2, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 3, costBlack= 6, value= 4, gives= (byte)Color.White },
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 3, costGreen= 3, costRed= 5, costBlack= 3, value= 3, gives= (byte)Color.White },
			// Blue Tier 3
			new Card() { id = 0, tier = 2, costWhite = 7, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 2, costWhite = 7, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 0, value= 5, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 2, costWhite = 6, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 3, value= 4, gives= (byte)Color.Blue },
			new Card() { id = 0, tier = 2, costWhite = 3, costBlue= 0, costGreen= 3, costRed= 3, costBlack= 5, value= 3, gives= (byte)Color.Blue },
			// Green Tier 3
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 7, costGreen= 0, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 7, costGreen= 3, costRed= 0, costBlack= 0, value= 5, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 2, costWhite = 3, costBlue= 6, costGreen= 3, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Green },
			new Card() { id = 0, tier = 2, costWhite = 5, costBlue= 3, costGreen= 0, costRed= 3, costBlack= 3, value= 3, gives= (byte)Color.Green },
			// Red Tier 3
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 0, costGreen= 7, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 0, costGreen= 7, costRed= 3, costBlack= 0, value= 5, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 3, costGreen= 6, costRed= 3, costBlack= 0, value= 4, gives= (byte)Color.Red },
			new Card() { id = 0, tier = 2, costWhite = 3, costBlue= 5, costGreen= 3, costRed= 0, costBlack= 3, value= 3, gives= (byte)Color.Red },
			// Black Tier 3
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 7, costBlack= 0, value= 4, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 7, costBlack= 3, value= 5, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 2, costWhite = 0, costBlue= 0, costGreen= 3, costRed= 6, costBlack= 3, value= 4, gives= (byte)Color.Black },
			new Card() { id = 0, tier = 2, costWhite = 3, costBlue= 3, costGreen= 5, costRed= 3, costBlack= 0, value= 3, gives= (byte)Color.Black },
		};
	}
}
