namespace Splendor.Model
{
	using System.Collections.Generic;

	internal static class Rules
	{
		public const int Tiers = 3;
		public const int CardsPerTier = 4;
		public const int MarketSize = Tiers * CardsPerTier;

		public const int RequiredPoints = 15;

		public const int MaxHandSize = 3;
		public const int MaxTableauSize = 32;

		public const int CardinalColorCount = 5;

		public const int GoldCount = 5;

		public static readonly IAction[] BuildActions;
		public static readonly IAction[] ReserveActions;
		public static readonly IAction[] TakeActions;
		public static readonly IAction[] ReplaceActions;
		public static readonly IAction[] NobleActions;

		static Rules()
		{
			// generate all card actions
			BuildActions = new IAction[Rules.Cards.Length];
			ReserveActions = new IAction[Rules.Cards.Length];
			for (int i = 0; i < Rules.Cards.Length; i++)
			{
				BuildActions[i] = new BuildCardAction(Rules.Cards[i]);
				ReserveActions[i] = new ReserveCardAction(Rules.Cards[i]);
			}
			// generate take actions
			// 10 (5C3) + 5 
			int ti = 0;
			TakeActions = new IAction[15];
			// generate permutations of 3 different
			for (int i = 0; i < 5; i++)
			{
				for (int j = i + 1; j < 5; j++)
				{
					for (int k = j + 1; k < 5; k++)
					{
						TakeActions[ti++] = new TakeTokensAction((Color)i, (Color)j, (Color)k);
					}
				}
			}
			// generate permutations of 2 identical
			for (int i = 0; i < 5; i++)
			{
				TakeActions[ti++] = new TakeTokensAction((Color)i, (Color)i);
			}
			// generate all noble visit actions
			NobleActions = new IAction[Rules.Nobles.Length];
			for (int i = 0; i < Rules.Nobles.Length; i++)
			{
				NobleActions[i] = new NobleVisitAction(Rules.Nobles[i]);
			}
			// generate all token return moves
			ReplaceActions = new IAction[6];
			for (int i = 0; i < 6; i++)
			{
				ReplaceActions[i] = new ReplaceTokensAction((Color)i);
			}
			//ti = 0;
			//ReplaceActions = new IAction[258]; // (6 + 6*6 + 6*6*6)
			//for (int i = 0; i < 6; i++)
			//{
			//	actions.Add(new ReplaceTokensAction((Color)i));
			//	for (int j = 0; j < 6; j++)
			//	{
			//		actions.Add(new ReplaceTokensAction((Color)i, (Color)j));
			//		for (int k = 0; k < 6; k++)
			//		{
			//			actions.Add(new ReplaceTokensAction((Color)i, (Color)j, (Color)k));
			//		}
			//	}
			//}
		}

		public static readonly Noble SentinelNoble = new Noble() { id = 0, requires = new byte[] { 255, 255, 255, 255, 255 }, value = 0 };
		public static readonly Noble[] Nobles =
		{
			SentinelNoble,
			// white, blue, green, red, black
			new Noble() { id = 1, requires = new byte[] { 3, 3, 0, 0, 3 }, value = 3 },
			new Noble() { id = 2, requires = new byte[] { 0, 3, 3, 3, 0 }, value = 3 },
			new Noble() { id = 3, requires = new byte[] { 3, 0, 0, 3, 3 }, value = 3 },
			new Noble() { id = 4, requires = new byte[] { 0, 0, 4, 4, 0 }, value = 3 },
			new Noble() { id = 5, requires = new byte[] { 0, 4, 4, 0, 0 }, value = 3 },
			new Noble() { id = 6, requires = new byte[] { 0, 0, 0, 4, 4 }, value = 3 },
			new Noble() { id = 7, requires = new byte[] { 4, 0, 0, 0, 4 }, value = 3 },
			new Noble() { id = 8, requires = new byte[] { 3, 3, 3, 0, 0 }, value = 3 },
			new Noble() { id = 9, requires = new byte[] { 0, 0, 3, 3, 3 }, value = 3 },
			new Noble() { id = 10, requires = new byte[] { 4, 4, 0, 0, 0 }, value = 3 },
		};

		public static readonly Card SentinelCard = new Card() { id = 0, tier = 255, costBlack = 255, costBlue = 255, costGreen = 255, costRed = 255, costWhite = 255, gives = 255, value = 255 };
		public static readonly Card[] Cards = 
		{
			SentinelCard,
			// White Tier 1
			new Card() { id = 1, tier = 0, costWhite = 0, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.White },
			new Card() { id = 2, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 2, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 3, tier = 0, costWhite = 0, costBlue= 1, costGreen= 1, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 4, tier = 0, costWhite = 0, costBlue= 2, costGreen= 0, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.White },
			new Card() { id = 5, tier = 0, costWhite = 0, costBlue= 0, costGreen= 4, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.White },
			new Card() { id = 6, tier = 0, costWhite = 0, costBlue= 1, costGreen= 2, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 7, tier = 0, costWhite = 0, costBlue= 2, costGreen= 2, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.White },
			new Card() { id = 8, tier = 0, costWhite = 3, costBlue= 1, costGreen= 0, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.White },
			// Blue Tier 1
			new Card() { id = 9, tier = 0, costWhite = 1, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 10, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 3, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 11, tier = 0, costWhite = 1, costBlue= 0, costGreen= 1, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 12, tier = 0, costWhite = 0, costBlue= 0, costGreen= 2, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 13, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 4, costBlack= 0, value= 1, gives= (byte)Color.Blue },
			new Card() { id = 14, tier = 0, costWhite = 1, costBlue= 0, costGreen= 1, costRed= 2, costBlack= 1, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 15, tier = 0, costWhite = 1, costBlue= 0, costGreen= 2, costRed= 2, costBlack= 0, value= 0, gives= (byte)Color.Blue },
			new Card() { id = 16, tier = 0, costWhite = 0, costBlue= 1, costGreen= 3, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Blue },
			// Green Tier 1	  
			new Card() { id = 17, tier = 0, costWhite = 2, costBlue= 1, costGreen= 0, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Green },
			new Card() { id = 18, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 3, costBlack= 0, value= 0, gives= (byte)Color.Green },
			new Card() { id = 19, tier = 0, costWhite = 1, costBlue= 1, costGreen= 0, costRed= 1, costBlack= 1, value= 0, gives= (byte)Color.Green },
			new Card() { id = 20, tier = 0, costWhite = 0, costBlue= 2, costGreen= 0, costRed= 2, costBlack= 0, value= 0, gives= (byte)Color.Green },
			new Card() { id = 21, tier = 0, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 4, value= 1, gives= (byte)Color.Green },
			new Card() { id = 22, tier = 0, costWhite = 1, costBlue= 1, costGreen= 0, costRed= 1, costBlack= 2, value= 0, gives= (byte)Color.Green },
			new Card() { id = 23, tier = 0, costWhite = 0, costBlue= 1, costGreen= 0, costRed= 2, costBlack= 2, value= 0, gives= (byte)Color.Green },
			new Card() { id = 24, tier = 0, costWhite = 1, costBlue= 3, costGreen= 1, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Green },
			// Red Tier 1	  
			new Card() { id = 25, tier = 0, costWhite = 0, costBlue= 2, costGreen= 1, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Red },
			new Card() { id = 26, tier = 0, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Red },
			new Card() { id = 27, tier = 0, costWhite = 1, costBlue= 1, costGreen= 1, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.Red },
			new Card() { id = 28, tier = 0, costWhite = 2, costBlue= 0, costGreen= 0, costRed= 2, costBlack= 0, value= 0, gives= (byte)Color.Red },
			new Card() { id = 29, tier = 0, costWhite = 4, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.Red },
			new Card() { id = 30, tier = 0, costWhite = 2, costBlue= 1, costGreen= 1, costRed= 0, costBlack= 1, value= 0, gives= (byte)Color.Red },
			new Card() { id = 31, tier = 0, costWhite = 2, costBlue= 0, costGreen= 1, costRed= 0, costBlack= 2, value= 0, gives= (byte)Color.Red },
			new Card() { id = 32, tier = 0, costWhite = 1, costBlue= 0, costGreen= 0, costRed= 1, costBlack= 3, value= 0, gives= (byte)Color.Red },
			// Black Tier 1	  
			new Card() { id = 33, tier = 0, costWhite = 0, costBlue= 0, costGreen= 2, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 34, tier = 0, costWhite = 0, costBlue= 0, costGreen= 3, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 35, tier = 0, costWhite = 1, costBlue= 1, costGreen= 1, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 36, tier = 0, costWhite = 2, costBlue= 0, costGreen= 2, costRed= 0, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 37, tier = 0, costWhite = 0, costBlue= 4, costGreen= 0, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.Black },
			new Card() { id = 38, tier = 0, costWhite = 1, costBlue= 2, costGreen= 1, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 39, tier = 0, costWhite = 2, costBlue= 2, costGreen= 0, costRed= 1, costBlack= 0, value= 0, gives= (byte)Color.Black },
			new Card() { id = 40, tier = 0, costWhite = 0, costBlue= 0, costGreen= 1, costRed= 3, costBlack= 1, value= 0, gives= (byte)Color.Black },
			// White Tier 2	  
			new Card() { id = 41, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 5, costBlack= 0, value= 2, gives= (byte)Color.White },
			new Card() { id = 42, tier = 1, costWhite = 6, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 3, gives= (byte)Color.White },
			new Card() { id = 43, tier = 1, costWhite = 0, costBlue= 0, costGreen= 3, costRed= 2, costBlack= 2, value= 1, gives= (byte)Color.White },
			new Card() { id = 44, tier = 1, costWhite = 0, costBlue= 0, costGreen= 1, costRed= 4, costBlack= 2, value= 2, gives= (byte)Color.White },
			new Card() { id = 45, tier = 1, costWhite = 2, costBlue= 3, costGreen= 0, costRed= 3, costBlack= 0, value= 1, gives= (byte)Color.White },
			new Card() { id = 46, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 5, costBlack= 3, value= 2, gives= (byte)Color.White },
			// Blue Tier 2	  
			new Card() { id = 47, tier = 1, costWhite = 0, costBlue= 5, costGreen= 0, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Blue },
			new Card() { id = 48, tier = 1, costWhite = 0, costBlue= 6, costGreen= 0, costRed= 0, costBlack= 0, value= 3, gives= (byte)Color.Blue },
			new Card() { id = 49, tier = 1, costWhite = 0, costBlue= 2, costGreen= 2, costRed= 3, costBlack= 0, value= 1, gives= (byte)Color.Blue },
			new Card() { id = 50, tier = 1, costWhite = 2, costBlue= 0, costGreen= 0, costRed= 1, costBlack= 4, value= 2, gives= (byte)Color.Blue },
			new Card() { id = 51, tier = 1, costWhite = 5, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Blue },
			// Green Tier 2	  
			new Card() { id = 52, tier = 1, costWhite = 0, costBlue= 0, costGreen= 5, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Green },
			new Card() { id = 53, tier = 1, costWhite = 0, costBlue= 0, costGreen= 6, costRed= 0, costBlack= 0, value= 3, gives= (byte)Color.Green },
			new Card() { id = 54, tier = 1, costWhite = 2, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 2, value= 1, gives= (byte)Color.Green },
			new Card() { id = 55, tier = 1, costWhite = 3, costBlue= 0, costGreen= 2, costRed= 2, costBlack= 0, value= 1, gives= (byte)Color.Green },
			new Card() { id = 56, tier = 1, costWhite = 4, costBlue= 2, costGreen= 0, costRed= 0, costBlack= 1, value= 2, gives= (byte)Color.Green },
			new Card() { id = 57, tier = 1, costWhite = 0, costBlue= 5, costGreen= 3, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Green },
			// Red Tier 2	  
			new Card() { id = 58, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 5, value= 2, gives= (byte)Color.Red },
			new Card() { id = 59, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 6, costBlack= 0, value= 3, gives= (byte)Color.Red },
			new Card() { id = 60, tier = 1, costWhite = 2, costBlue= 0, costGreen= 0, costRed= 2, costBlack= 3, value= 1, gives= (byte)Color.Red },
			new Card() { id = 61, tier = 1, costWhite = 1, costBlue= 4, costGreen= 2, costRed= 0, costBlack= 0, value= 2, gives= (byte)Color.Red },
			new Card() { id = 62, tier = 1, costWhite = 0, costBlue= 3, costGreen= 0, costRed= 2, costBlack= 3, value= 1, gives= (byte)Color.Red },
			new Card() { id = 63, tier = 1, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 5, value= 2, gives= (byte)Color.Red },
			// Black Tier 2	  
			new Card() { id = 64, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 5, value= 2, gives= (byte)Color.Black },
			new Card() { id = 65, tier = 1, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 6, value= 3, gives= (byte)Color.Black },
			new Card() { id = 66, tier = 1, costWhite = 3, costBlue= 2, costGreen= 2, costRed= 0, costBlack= 0, value= 1, gives= (byte)Color.Black },
			new Card() { id = 67, tier = 1, costWhite = 0, costBlue= 1, costGreen= 4, costRed= 2, costBlack= 0, value= 2, gives= (byte)Color.Black },
			new Card() { id = 68, tier = 1, costWhite = 3, costBlue= 0, costGreen= 3, costRed= 0, costBlack= 2, value= 1, gives= (byte)Color.Black },
			new Card() { id = 69, tier = 1, costWhite = 0, costBlue= 0, costGreen= 5, costRed= 3, costBlack= 0, value= 2, gives= (byte)Color.Black },
			// White Tier 3	  
			new Card() { id = 70, tier = 2, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 7, value= 4, gives= (byte)Color.White },
			new Card() { id = 71, tier = 2, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 7, value= 5, gives= (byte)Color.White },
			new Card() { id = 72, tier = 2, costWhite = 3, costBlue= 0, costGreen= 0, costRed= 3, costBlack= 6, value= 4, gives= (byte)Color.White },
			new Card() { id = 73, tier = 2, costWhite = 0, costBlue= 3, costGreen= 3, costRed= 5, costBlack= 3, value= 3, gives= (byte)Color.White },
			// Blue Tier 3	  
			new Card() { id = 74, tier = 2, costWhite = 7, costBlue= 0, costGreen= 0, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Blue },
			new Card() { id = 75, tier = 2, costWhite = 7, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 0, value= 5, gives= (byte)Color.Blue },
			new Card() { id = 76, tier = 2, costWhite = 6, costBlue= 3, costGreen= 0, costRed= 0, costBlack= 3, value= 4, gives= (byte)Color.Blue },
			new Card() { id = 77, tier = 2, costWhite = 3, costBlue= 0, costGreen= 3, costRed= 3, costBlack= 5, value= 3, gives= (byte)Color.Blue },
			// Green Tier 3	  
			new Card() { id = 78, tier = 2, costWhite = 0, costBlue= 7, costGreen= 0, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Green },
			new Card() { id = 79, tier = 2, costWhite = 0, costBlue= 7, costGreen= 3, costRed= 0, costBlack= 0, value= 5, gives= (byte)Color.Green },
			new Card() { id = 80, tier = 2, costWhite = 3, costBlue= 6, costGreen= 3, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Green },
			new Card() { id = 81, tier = 2, costWhite = 5, costBlue= 3, costGreen= 0, costRed= 3, costBlack= 3, value= 3, gives= (byte)Color.Green },
			// Red Tier 3	  
			new Card() { id = 82, tier = 2, costWhite = 0, costBlue= 0, costGreen= 7, costRed= 0, costBlack= 0, value= 4, gives= (byte)Color.Red },
			new Card() { id = 83, tier = 2, costWhite = 0, costBlue= 0, costGreen= 7, costRed= 3, costBlack= 0, value= 5, gives= (byte)Color.Red },
			new Card() { id = 84, tier = 2, costWhite = 0, costBlue= 3, costGreen= 6, costRed= 3, costBlack= 0, value= 4, gives= (byte)Color.Red },
			new Card() { id = 85, tier = 2, costWhite = 3, costBlue= 5, costGreen= 3, costRed= 0, costBlack= 3, value= 3, gives= (byte)Color.Red },
			// Black Tier 3	  
			new Card() { id = 86, tier = 2, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 7, costBlack= 0, value= 4, gives= (byte)Color.Black },
			new Card() { id = 87, tier = 2, costWhite = 0, costBlue= 0, costGreen= 0, costRed= 7, costBlack= 3, value= 5, gives= (byte)Color.Black },
			new Card() { id = 88, tier = 2, costWhite = 0, costBlue= 0, costGreen= 3, costRed= 6, costBlack= 3, value= 4, gives= (byte)Color.Black },
			new Card() { id = 89, tier = 2, costWhite = 3, costBlue= 3, costGreen= 5, costRed= 3, costBlack= 0, value= 3, gives= (byte)Color.Black },
		};
	};
}
