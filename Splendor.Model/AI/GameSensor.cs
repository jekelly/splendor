namespace Splendor.Model.AI
{
	using System;
	using System.Linq;

	class GameSensor3 : ISensor<IGame>
	{
		private const int dimensions = 2;
		private readonly int playerIndex;
		private readonly string[] descriptions;

		public GameSensor3(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.descriptions = this.GenerateDescriptions();
		}

		public int Dimensions
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
			if (i != dimensions)
			{
				throw new IndexOutOfRangeException();
			}
			env[env.Length - 1] = 1.0;
			return env;
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

		public int Dimensions
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

		public int Dimensions
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

		public int Dimensions
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
