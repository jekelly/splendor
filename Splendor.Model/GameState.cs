namespace Splendor.Model
{
	using System;
	using System.Linq;

	public partial class Game
	{
		protected sealed class GameState
		{
			public const int SupplyIndex = 4;

			public int currentPlayer;
			public Phase currentPhase;

			public readonly int numPlayers;

			public readonly int[][] tokens;

			public readonly Deck[] decks;

			public readonly int[] market;

			public readonly int[][] hands;
			public readonly int[] handSize;

			public readonly int[][] tableau;
			public readonly int[] tableauSize;

			// could be calculated, but expensive
			public readonly int[][] gems;

			public readonly int[] nobles;
			public readonly int[] nobleVisiting;

			public readonly int[] debt;

			public int lastPlayerIndex = -1;
			public int turn;

			public void ShuffleDecks(IRandomizer randomizer)
			{
				for (int tier = 0; tier < this.decks.Length; tier++)
				{
					this.decks[tier].Shuffle(randomizer);
				}
			}

			private GameState()
			{
			}

			public GameState(Setup setup, IRandomizer randomizer)
			{
				this.tokens = new int[5][];
				for (int i = 0; i < 5; i++)
				{
					this.tokens[i] = new int[6];
				}
				this.decks = new Deck[Rules.Tiers];
				for (int i = 0; i < this.decks.Length; i++)
				{
					var cards = Rules.Cards;
					var first = cards.First(card => card.tier == i);
					var count = cards.Count(card => card.tier == i);
					this.decks[i] = new Deck(Rules.Cards, first.id, count);
				}
				this.market = new int[Rules.MarketSize];
				// shuffle decks separately?
				this.ShuffleDecks(randomizer);
				// reveal top four from each deck
				for (int i = 0; i < this.market.Length; i++)
				{
					int tier = (i / Rules.CardsPerTier);
					this.market[i] = decks[tier].Draw().id;
				}
				// shuffle and reveal nobles
				var possibleNobles = Rules.Nobles.Length - 1;
				this.nobles = Enumerable.Range(1, possibleNobles).ToArray();
				this.nobleVisiting = new int[setup.nobleCount];
				for (int i = 0; i < setup.nobleCount; i++)
				{
					int index = randomizer.Next(possibleNobles);
					int tmp = this.nobles[possibleNobles - 1];
					this.nobles[possibleNobles - 1] = this.nobles[index];
					this.nobles[index] = tmp;
					possibleNobles--;
					this.nobleVisiting[i] = SupplyIndex;
				}
				this.nobles = this.nobles.Reverse().Take(setup.nobleCount).ToArray();
				// populate tokens
				this.tokens[SupplyIndex][(int)Color.White] = setup.tokenCount;
				this.tokens[SupplyIndex][(int)Color.Blue] = setup.tokenCount;
				this.tokens[SupplyIndex][(int)Color.Green] = setup.tokenCount;
				this.tokens[SupplyIndex][(int)Color.Red] = setup.tokenCount;
				this.tokens[SupplyIndex][(int)Color.Black] = setup.tokenCount;
				this.tokens[SupplyIndex][(int)Color.Gold] = Rules.GoldCount;
				// setup player-specific variables
				this.numPlayers = setup.playerCount;
				this.hands = new int[this.numPlayers][];
				this.gems = new int[this.numPlayers][];
				for (int i = 0; i < this.numPlayers; i++)
				{
					this.hands[i] = new int[Rules.MaxHandSize];
					this.gems[i] = new int[Rules.CardinalColorCount];
				}
				this.handSize = new int[this.numPlayers];
				this.tableau = new int[this.numPlayers][];
				for (int i = 0; i < this.numPlayers; i++)
				{
					this.tableau[i] = new int[Rules.MaxTableauSize];
				}
				this.tableauSize = new int[this.numPlayers];
				// setup game-state variables
				this.debt = new int[6];
				for (int i = 0; i < 6; i++)
				{
					this.debt[i] = 0;
				}
				// determine starting player
				this.currentPlayer = randomizer.Next(setup.playerCount);
				this.currentPhase = Phase.Choose;
				this.turn = 0;
			}

			private static int[] CloneArray(int[] source)
			{
				int length = source.Length;
				int[] dest = new int[length];
				Buffer.BlockCopy(source, 0, dest, 0, length * sizeof(int));
				return dest;
			}

			private static int[][] CloneArrays(int[][] source)
			{
				int[][] dest = new int[source.Length][];
				for (int i = 0; i < dest.Length; i++)
				{
					dest[i] = CloneArray(source[i]);
				}
				return dest;
			}

			private Deck[] CloneDecks(Deck[] decks)
			{
				Deck[] clone = new Deck[decks.Length];
				for (int i = 0; i < clone.Length; i++)
				{
					clone[i] = new Deck(decks[i]);
				}
				return clone;
			}

			private GameState(GameState other)
			{
				this.currentPlayer = other.currentPlayer;
				this.currentPhase = other.currentPhase;
				this.numPlayers = other.numPlayers;
				this.tokens = CloneArrays(other.tokens);
				this.decks = CloneDecks(other.decks);
				this.market = CloneArray(other.market);
				this.hands = CloneArrays(other.hands);
				this.handSize = CloneArray(other.handSize);
				this.tableau = CloneArrays(other.tableau);
				this.tableauSize = CloneArray(other.tableauSize);
				this.gems = CloneArrays(other.gems);
				this.nobles = CloneArray(other.nobles);
				this.nobleVisiting = CloneArray(other.nobleVisiting);
				this.debt = CloneArray(other.debt);
				this.lastPlayerIndex = other.lastPlayerIndex;
				this.turn = other.turn;
			}

			public GameState Clone()
			{
				return new GameState(this);
			}

			public bool IsValid()
			{
				// Ensure all tokens are accounted for
				int tokenCount = Setups.All[0].tokenCount;
				for (Color c = Color.White; c < Color.Gold; c++)
				{
					int sum = this.tokens[(int)c].Sum();
					if (sum != tokenCount)
					{
						return false;
					}
				}
				if (this.tokens[5].Sum() != Rules.GoldCount)
				{
					return false;
				}
				// Ensure no player has more than 3 cards in hand

				// Ensure tableau has 12 cards

				// and that there are exactly 4 in each tier

				return true;
			}
		}
	}
}