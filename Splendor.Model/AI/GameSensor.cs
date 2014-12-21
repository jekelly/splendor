namespace Splendor.Model.AI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	interface IDimension<T>
	{
		string Name { get; }
		double Measure(T environment);
	}

	class GameDimension : IDimension<IGame>
	{
		private readonly Func<IGame, double> sensor;
		private readonly int range;

		public GameDimension(string name, Func<IGame, double> sensor)
		{
			this.Name = name;
			this.sensor = sensor;
		}

		public string Name
		{
			get;
			private set;
		}

		public double Measure(IGame environment)
		{
			return this.sensor(environment);
		}
	}

	static class GameDimensions
	{
		/// <summary>
		/// Score / 15. Range of approximately 0 to 1.5.
		/// </summary>
		public static IDimension<IGame> ScaledScore(int playerIndex)
		{
			return new GameDimension("Player" + playerIndex + " Score / 15", game => game.Players[playerIndex].Score / 15.0);
		}

		/// <summary>
		/// Relative score / 15. Range of approximately -1.5 to 1.5.
		/// </summary>
		public static IDimension<IGame> RelativeScore(int playerIndex, int opponentIndex)
		{
			return new GameDimension("Relative Score of " + playerIndex + " to " + opponentIndex, (game) =>
				{
					return (game.Players[playerIndex].Score - game.Players[opponentIndex].Score) / 15.0;
				});
		}

		/// <summary>
		/// Scaled count of permanent gems of a specific color, ranges from 0 to 2.0+.
		/// </summary>
		public static IEnumerable<IDimension<IGame>> ScaledGems(int playerIndex)
		{
			for (int i = 0; i < Rules.CardinalColorCount; i++)
			{
				int c = i;
				yield return new GameDimension(string.Format("{0} gems for P{1}", (Color)c, playerIndex), (game) => game.Players[playerIndex].Gems((Color)c) / 4.0);
			}
		}

		/// <summary>
		/// Scaled count of tokens of a specific color, ranges from 0 to 1.0.
		/// </summary>
		public static IEnumerable<IDimension<IGame>> ScaledTokens(int playerIndex)
		{
			for (int i = 0; i < 6; i++)
			{
				int c = i;
				yield return new GameDimension(string.Format("{0} gems for P{1}", (Color)i, playerIndex), (game) => game.Players[playerIndex].Tokens((Color)c) / 4.0);
			}
		}

		public static IEnumerable<IDimension<IGame>> ScaledCardAffordability(int playerIndex)
		{
			for (int i = 1; i < Rules.Cards.Length; i++)
			{
				Card card = Rules.Cards[i];
				yield return new GameDimension(string.Format("Card {0} affordability for {1}", i, playerIndex), (game) =>
					{
						IPlayer player = game.Players[playerIndex];
						int[] bp = player.BuyingPower;
						double affordability = 0.0;
						affordability += Math.Max(0, card.costBlack - bp[(int)Color.Black]);
						affordability += Math.Max(0, card.costBlue - bp[(int)Color.Blue]);
						affordability += Math.Max(0, card.costGreen - bp[(int)Color.Green]);
						affordability += Math.Max(0, card.costRed - bp[(int)Color.Red]);
						affordability += Math.Max(0, card.costWhite - bp[(int)Color.White]);
						affordability = Math.Max(0, affordability - bp[(int)Color.Gold]);
						return affordability / 10.0; // todo: is this the right scale factor?
					});
			}
		}

		/// <summary>
		/// Dimensions for each card, denoting if it is available in the game. 1.0 if available, else 0.0.
		/// </summary>
		public static IEnumerable<IDimension<IGame>> CardAvailable(int playerIndex)
		{
			for (int i = 1; i < Rules.Cards.Length; i++)
			{
				Card card = Rules.Cards[i];
				yield return new GameDimension(string.Format("Card {0} in Market?", i), game => (game.Players[playerIndex].Hand.Contains(card) || game.Market.Contains(card)) ? 1.0 : 0.0);
			}
		}
	}

	abstract class GameSensorBase : ISensor<IGame>
	{
		private string[] descriptions;

		protected abstract IDimension<IGame>[] Dimensions
		{
			get;
		}

		public int DimensionCount
		{
			get { return this.Dimensions.Length; }
		}

		public string[] Descriptions
		{
			get
			{
				if (this.descriptions == null)
				{
					this.descriptions = new string[this.DimensionCount + 1];
					for (int i = 0; i < this.DimensionCount; i++)
					{
						this.descriptions[i] = this.Dimensions[i].Name;
					}
					this.descriptions[this.DimensionCount] = "Bias";
				}
				return this.descriptions;
			}
		}

		public double[] Sense(IGame environment)
		{
			double[] results = new double[this.DimensionCount + 1];
			for (int i = 0; i < this.DimensionCount; i++)
			{
				results[i] = this.Dimensions[i].Measure(environment);
				Debug.WriteLine("Measured {0} at {1}", this.Dimensions[i].Name, results[i]);
			}
			results[this.DimensionCount] = 1.0; // bias;
			return results;
		}
	}

	class GameSensor3 : GameSensorBase
	{
		private readonly int playerIndex;
		private readonly IDimension<IGame>[] dimensions;

		protected override IDimension<IGame>[] Dimensions
		{
			get { return this.dimensions; }
		}

		public GameSensor3(int playerIndex)
		{
			this.playerIndex = playerIndex;
			List<IDimension<IGame>> gameDimensions = new List<IDimension<IGame>>();
			gameDimensions.Add(GameDimensions.ScaledScore(playerIndex));
			gameDimensions.Add(GameDimensions.RelativeScore(playerIndex, (playerIndex + 1) % 2));
			gameDimensions.AddRange(GameDimensions.ScaledGems(playerIndex));
			gameDimensions.AddRange(GameDimensions.ScaledTokens(playerIndex));
			gameDimensions.AddRange(GameDimensions.ScaledCardAffordability(playerIndex));
			gameDimensions.AddRange(GameDimensions.CardAvailable(playerIndex));

			this.dimensions = gameDimensions.ToArray();
		}
	}

	class GameSensor2 : ISensor<IGame>
	{
		private const int dimensions = 56;
		private readonly int playerIndex;
		private readonly string[] descriptions;

		public GameSensor2(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.descriptions = this.GenerateDescriptions();
		}

		public int DimensionCount
		{
			get { return dimensions; }
		}

		public string[] Descriptions
		{
			get { return this.descriptions; }
		}

		private string[] GenerateDescriptions()
		{
			string[] desc = new string[dimensions + 1];
			int i = 0;
			desc[i++] = "Score / 15";
			desc[i++] = "Relative Score";
			for (int t = 0; t < 10; t++)
			{
				desc[i++] = string.Format("Greater than {0} tokens", t);
			}
			for (Color c = Color.White; c <= Color.Gold; c++)
			{
				for (int cc = 0; cc < 4; cc++)
				{
					desc[i++] = string.Format("Is Token {0} over {1}?", c, cc);
				}
			}
			for (Color c = Color.White; c <= Color.Black; c++)
			{
				for (int cc = 0; cc < 4; cc++)
				{
					desc[i++] = string.Format("Is Gem {0} over {1}?", c, cc);
				}
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			desc[desc.Length - 1] = "Bias";
			return desc;
		}

		public double[] Sense(IGame environment)
		{
			double[] env = new double[dimensions + 1];
			IPlayer player = environment.GetPlayer(this.playerIndex);
			IPlayer opponent = environment.GetPlayer((this.playerIndex + 1) % 2);
			int i = 0;
			// whats my score as a percentage of a win?
			env[i++] = ((double)player.Score / Rules.RequiredPoints);
			// whats my percentile score differential with my opponent?
			env[i++] = (player.Score - opponent.Score) == 0 ? 0 : (double)(player.Score - opponent.Score) / (player.Score + opponent.Score);
			// am i near the token limit?
			for (int t = 0; t < 10; t++)
			{
				env[i++] = player.TokenCount > t ? 1.0 : 0.0;
			}
			for (Color c = Color.White; c <= Color.Gold; c++)
			{
				for (int cc = 0; cc < 4; cc++)
				{
					env[i++] = player.Tokens(c) > cc ? 1.0 : 0.0;
				}
			}
			for (Color c = Color.White; c <= Color.Black; c++)
			{
				for (int cc = 0; cc < 4; cc++)
				{
					env[i++] = player.Gems(c) > cc ? 1.0 : 0.0;
				}
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			env[env.Length - 1] = 1.0;
			return env;
		}
	}

	class GameSensor : ISensor<IGame>
	{
		private const int dimensions = 184;
		private readonly int playerIndex;
		private readonly string[] descriptions;

		public GameSensor(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.descriptions = this.GenerateDescriptions();
		}

		public int DimensionCount
		{
			get { return dimensions; }
		}

		public string[] Descriptions
		{
			get { return this.descriptions; }
		}

		private string[] GenerateDescriptions()
		{
			string[] desc = new string[dimensions + 1];
			int i = 0;
			desc[i++] = "Score / 15";
			desc[i++] = "Relative Score";

			desc[i++] = "Over 2 tokens";
			desc[i++] = "Over 4 tokens";
			desc[i++] = "Over 6 tokens";
			desc[i++] = "Over 8 tokens";
			//for (int c = 0; c < Rules.CardinalColorCount; c++)
			//{
			//	for (int cc = 0; cc < 3; cc++)
			//	{
			//		desc[i++] = string.Format("{0} gems on board > {1}", (Color)c, cc);
			//	}
			//	desc[i++] = ((Color)c).ToString() +" ratio over 3";
			//}
			//for (int c = 0; c < 6; c++)
			//{
			//	for (int cc = 0; cc < 5; cc++)
			//	{
			//		desc[i++] = string.Format("{0} tokens in hand > {1}", (Color)c, cc);
			//	}
			//}
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				desc[i++] = string.Format("Card {0} in hand?", c);
			}
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				desc[i++] = string.Format("Card {0} in market?", c);
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			desc[desc.Length - 1] = "Bias";
			return desc;
		}

		public double[] Sense(IGame environment)
		{
			double[] env = new double[dimensions + 1];
			IPlayer player = environment.GetPlayer(this.playerIndex);
			IPlayer opponent = environment.GetPlayer((this.playerIndex + 1) % 2);
			int i = 0;
			// whats my score as a percentage of a win?
			env[i++] = ((double)player.Score / Rules.RequiredPoints);
			// whats my percentile score differential with my opponent?
			env[i++] = (player.Score - opponent.Score) == 0 ? 0 : (double)(player.Score - opponent.Score) / (player.Score + opponent.Score);
			// am i near the token limit?
			env[i++] = player.TokenCount > 2 ? 1.0 : 0.0;
			env[i++] = player.TokenCount > 4 ? 1.0 : 0.0;
			env[i++] = player.TokenCount > 6 ? 1.0 : 0.0;
			env[i++] = player.TokenCount > 8 ? 1.0 : 0.0;
			// how many gems of each color do i have from buildings?
			//for (int c = 0; c < Rules.CardinalColorCount; c++)
			//{
			//	for (int cc = 0; cc < 3; cc++)
			//	{
			//		env[i++] = player.Gems((Color)c) > cc ? 1.0 : 0.0;
			//	}
			//	env[i++] = player.Gems((Color)c) >= 4 ? player.Gems((Color)c) - 3 / 2.0 : 0.0;
			//}
			//// how many tokens of each color do i have?
			//for (int c = 0; c < 6; c++)
			//{
			//	// TODO: need a way to figure out how many tokens are available for the given setup?
			//	for (int cc = 0; cc < 5; cc++)
			//	{
			//		env[i++] = player.Tokens((Color)c) > cc ? 1.0 : 0.0;
			//	}
			//}
			// what cards are in my hand?
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				env[i++] = player.Hand.Contains(Rules.Cards[c]) ? 1.0 : 0.0;
			}
			// what cards are in the market?
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				env[i++] = environment.Market.Contains(Rules.Cards[c]) ? 1.0 : 0.0;
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			env[env.Length - 1] = 1.0;
			return env;
		}
	}

	class GameSensor5 : ISensor<IGame>
	{
		private const int dimensions = 170;
		private readonly int playerIndex;
		private readonly string[] descriptions;

		public GameSensor5(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.descriptions = this.GenerateDescriptions();
		}

		public int DimensionCount
		{
			get { return dimensions; }
		}

		public string[] Descriptions
		{
			get { return this.descriptions; }
		}

		private string[] GenerateDescriptions()
		{
			string[] desc = new string[dimensions + 1];
			int i = 0;
			for (int d = 0; d < 20; d++)
			{
				desc[i++] = string.Format("Score > {0}", d);
			}
			for (int d = 0; d < 20; d++)
			{
				desc[i++] = string.Format("Opponent score > {0}", d);
			}
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					desc[i++] = string.Format("{0} tokens in supply > {1}", (Color)c, cc);
				}
			}
			for (int t = 0; t < 100; t += 10)
			{
				desc[i++] = string.Format("Turns > {0}?", t);
			}
			for (int m = 0; m < Rules.MarketSize; m++)
			{
				desc[i++] = "Normalized value of market card " + i;
			}
			for (int h = 0; h < Rules.MaxHandSize; h++)
			{
				desc[i++] = "Normalized value of hand card " + i;
			}
			// colors needed for each card in hand/market?
			for (int m = 0; m < Rules.MarketSize; m++)
			{
				for (int c = 0; c < Rules.CardinalColorCount; c++)
				{
					desc[i++] = string.Format("{0} needed to buy card {1} from market", (Color)c, m);
				}
			}
			for (int h = 0; h < Rules.MaxHandSize; h++)
			{
				for (int c = 0; c < Rules.CardinalColorCount; c++)
				{
					desc[i++] = string.Format("{0} needed to buy card {1} from hand", (Color)c, h);
				}
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			desc[desc.Length - 1] = "Bias";
			return desc;
		}

		public double[] Sense(IGame environment)
		{
			double[] env = new double[dimensions + 1];
			IPlayer player = environment.GetPlayer(this.playerIndex);
			IPlayer opponent = environment.GetPlayer((this.playerIndex + 1) % 2);
			int i = 0;
			for (int d = 0; d < 20; d++)
			{
				env[i++] = player.Score > d ? 1.0 : 0.0;
			}
			for (int d = 0; d < 20; d++)
			{
				env[i++] = opponent.Score > d ? 1.0 : 0.0;
			}
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					env[i++] = environment.Supply((Color)c) > cc ? 1.0 : 0.0;
				}
			}
			// how many turns have elapsed?
			for (int t = 0; t < 100; t += 10)
			{
				env[i++] = environment.Turns > t ? 1.0 : 0.0;
			}
			// normalized value of each card in market
			for (int m = 0; m < Rules.MarketSize; m++)
			{
				env[i++] = environment.Market[m].value / 5.0;
			}
			// normalized value of each card in hand
			for (int h = 0; h < Rules.MaxHandSize; h++)
			{
				Card inHand = player.Hand.ElementAtOrDefault(h);
				if (inHand.id == Rules.SentinelCard.id)
				{
					env[i++] = 0.0d;
				}
				else
				{
					env[i++] = player.Hand.ElementAt(h).value / 5.0;
				}
			}
			// colors needed for each card in market?
			for (int m = 0; m < Rules.MarketSize; m++)
			{
				Card card = environment.Market[m];
				int availableGold = player.Tokens(Color.Gold);
				for (int c = 0; c < Rules.CardinalColorCount; c++)
				{
					int cost = card.Cost((Color)c);
					int missingCost = cost - player.BuyingPower[c];
					while (missingCost-- > 0 && availableGold-- > 0) ;
					env[i++] = (double)missingCost / 7.0; //cost;
				}
			}
			// colors needed for each card in hand?
			for (int h = 0; h < Rules.MaxHandSize; h++)
			{
				Card card = player.Hand.ElementAtOrDefault(h);
				int availableGold = player.Tokens(Color.Gold);
				for (int c = 0; c < Rules.CardinalColorCount; c++)
				{
					if (card.id == Rules.SentinelCard.id)
					{
						env[i++] = 1.0d;
						continue;
					}
					int cost = card.Cost((Color)c);
					int missingCost = cost - player.BuyingPower[c];
					while (missingCost-- > 0 && availableGold-- > 0) ;
					env[i++] = (double)missingCost / 7.0; //cost;
				}
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			env[env.Length - 1] = 1.0;
			return env;
		}
	}

	class GameSensor4 : ISensor<IGame>
	{
		private const int dimensions = 338;
		private readonly int playerIndex;
		private readonly string[] descriptions;

		public GameSensor4(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.descriptions = this.GenerateDescriptions();
		}

		public int DimensionCount
		{
			get { return dimensions; }
		}

		public string[] Descriptions
		{
			get { return this.descriptions; }
		}

		private string[] GenerateDescriptions()
		{
			string[] desc = new string[dimensions + 1];
			int i = 0;
			for (int d = 0; d < 15; d++)
			{
				desc[i++] = string.Format("Score > {0}", d);
			}
			for (int d = 0; d < 15; d++)
			{
				desc[i++] = string.Format("Opponent score > {0}", d);
			}
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					desc[i++] = string.Format("{0} tokens in supply > {1}", (Color)c, cc);
				}
			}
			for (int t = 0; t < 100; t += 10)
			{
				desc[i++] = string.Format("Turns > {0}?", t);
			}
			for (int c = 0; c < Rules.CardinalColorCount; c++)
			{
				for (int cc = 0; cc < 3; cc++)
				{
					desc[i++] = string.Format("{0} gems on board > {1}", (Color)c, cc);
				}
				desc[i++] = ((Color)c).ToString() + " ratio over 3";
			}
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					desc[i++] = string.Format("{0} tokens in hand > {1}", (Color)c, cc);
				}
			}
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					desc[i++] = string.Format("{0} opponent tokens in hand > {1}", (Color)c, cc);
				}
			}
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				desc[i++] = string.Format("Card {0} in hand?", c);
			}
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				desc[i++] = string.Format("Card {0} in market?", c);
			}
			for (int n = 1; n < Rules.Nobles.Length; n++)
			{
				desc[i++] = string.Format("Noble {0} available?", n);
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			desc[desc.Length - 1] = "Bias";
			return desc;
		}

		public double[] Sense(IGame environment)
		{
			double[] env = new double[dimensions + 1];
			IPlayer player = environment.GetPlayer(this.playerIndex);
			IPlayer opponent = environment.GetPlayer((this.playerIndex + 1) % 2);
			int i = 0;
			for (int d = 0; d < 15; d++)
			{
				env[i++] = player.Score > d ? 1.0 : 0.0;
			}
			for (int d = 0; d < 15; d++)
			{
				env[i++] = opponent.Score > d ? 1.0 : 0.0;
			}
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					env[i++] = environment.Supply((Color)c) > cc ? 1.0 : 0.0;
				}
			}
			// how many turns have elapsed?
			for (int t = 0; t < 100; t += 10)
			{
				env[i++] = environment.Turns > t ? 1.0 : 0.0;
			}
			// how many gems of each color do i have from buildings?
			for (int c = 0; c < Rules.CardinalColorCount; c++)
			{
				for (int cc = 0; cc < 3; cc++)
				{
					env[i++] = player.Gems((Color)c) > cc ? 1.0 : 0.0;
				}
				env[i++] = player.Gems((Color)c) >= 4 ? player.Gems((Color)c) - 3 / 2.0 : 0.0;
			}
			// how many tokens of each color do i have?
			for (int c = 0; c < 6; c++)
			{
				// TODO: need a way to figure out how many tokens are available for the given setup?
				for (int cc = 0; cc < 5; cc++)
				{
					env[i++] = player.Tokens((Color)c) > cc ? 1.0 : 0.0;
				}
			}
			// how many tokens of each color does my opponent have?
			for (int c = 0; c < 6; c++)
			{
				for (int cc = 0; cc < 5; cc++)
				{
					env[i++] = opponent.Tokens((Color)c) > cc ? 1.0 : 0.0;
				}
			}
			// what cards are in my hand?
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				env[i++] = player.Hand.Contains(Rules.Cards[c]) ? 1.0 : 0.0;
			}
			// what cards are in the market?
			for (int c = 1; c < Rules.Cards.Length; c++)
			{
				env[i++] = environment.Market.Contains(Rules.Cards[c]) ? 1.0 : 0.0;
			}
			// what nobles are available?
			for (int n = 1; n < Rules.Nobles.Length; n++)
			{
				env[i++] = environment.Nobles.Contains(Rules.Nobles[n]) ? 1.0 : 0.0;
			}
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			env[env.Length - 1] = 1.0;
			return env;
		}
	}
}
