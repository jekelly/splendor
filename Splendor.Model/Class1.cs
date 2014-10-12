using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Model
{
	public enum Location
	{
		Box,
		Deck,
		Table,
		Hand,
		Play,
	}

	public class State
	{
		public Location Location;
		public int Index;
	}

	public class Noble
	{
		public byte id;
		public byte requiresWhite;
		public byte requiresBlue;
		public byte requiresGreen;
		public byte requiresRed;
		public byte requiresBlack;
		public byte value;
	}

	//abstract class Action
	//{
	//	public abstract void Do(GameState state);
	//	public abstract void Undo(GameState state);
	//}

	//class SetupAction : Action
	//{

	//}

	public class Deck
	{
		private int[] indices;
		private Card[] cards;
		private int startIndex;
		private int size;
		private int head;

		public Deck(Card[] cards, int start, int size)
		{
			if(cards == null)
			{
				throw new ArgumentNullException("cards");
			}
			this.cards = cards;
			if(start < 0 || 
				start > this.cards.Length ||
				size < 0 ||
				start + size > this.cards.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.startIndex = start;
			this.size = size;
			this.indices = Enumerable.Range(this.startIndex, size).ToArray();
			this.head = 0;
		}

		public Card Draw()
		{
			if(this.head >= this.size)
			{
				throw new InvalidOperationException();
			}
			return this[this.head++];
		}

		public Card this[int index]
		{
			get
			{
				if (index >= this.size || index < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.cards[this.indices[index]];
			}
		}

		public void Shuffle(IRandomizer randomizer)
		{
			for (int i = this.size - 1; i >= 0; i--)
			{
				int ind = randomizer.Next(i);
				int swap = this.indices[i];
				this.indices[i] = this.indices[ind];
				this.indices[ind] = swap;
			}
		}
	}

	public interface IRandomizer
	{
		int Next(int max);
	}

	public class Randomizer : IRandomizer
	{
		private Random random;

		public Randomizer()
		{
			this.random = new Random();
		}

		public int Next(int max)
		{
			return this.random.Next(max);
		}
	}

	class GameController
	{
		
	}
	
	interface IPlayer
	{
		int[] Tokens { get;  }
		Card[] Hand { get; }
		Card[] Tableau { get; }
	}

	interface IGame
	{
		int[] Tokens { get; }
		Card[] Market { get; }
		Noble[] Nobles { get; }
	}

	partial class GameState
	{
		class Player : IPlayer
		{
			private readonly GameState gameState;
			private readonly int playerIndex;

			public Player(GameState gameState, int playerIndex)
			{
				this.gameState = gameState;
				this.playerIndex = playerIndex;
			}

			public int[] Tokens
			{
				get { return this.gameState.tokens[this.playerIndex]; }
			}

			public Card[] Hand
			{
				get { return this.gameState.hands[this.playerIndex].Select(i => Rules.Cards[i]).ToArray(); }
			}

			public Card[] Tableau
			{
				get { return this.gameState.tableau[this.playerIndex].Select(i => Rules.Cards[i]).ToArray(); }
			}
		}

		class Game : IGame
		{
			private readonly GameState gameState;

			public Game(GameState gameState)
			{
				this.gameState = gameState;
			}

			public int[] Tokens
			{
				get { return this.gameState.tokens[SupplyIndex]; }
			}

			public Card[] Market
			{
				get { return this.gameState.market.Select(i => Rules.Cards[i]).ToArray(); }
			}

			public Noble[] Nobles
			{
				get { throw new NotImplementedException(); }
			}
		}
	}

	enum Action
	{
		TakeWhite,
		TakeBlue,
		TakeGreen,
		TakeRed,
		TakeBlack,
	}

	public partial class GameState
	{
		private const int SupplyIndex = 4;

		private readonly IRandomizer randomizer;
		private readonly int playerCount;
		private readonly int[][] tokens;
		private readonly Deck[] decks;
		private readonly int[] market;
		private readonly int[][] hands;
		private readonly int[][] tableau;

		private readonly Action[] actions;
		private int actionCount;

		private int currentPlayer;

		public void ShuffleDecks()
		{
			for (int tier = 0; tier < this.decks.Length; tier++)
			{
				this.decks[tier].Shuffle(this.randomizer);
			}
		}

		public IEnumerable<Action> GetActions()
		{
			List<Action> availableActions = new List<Action>();
			if (this.actionCount= 0)
			{
				// take any token
				yield return Action.TakeWhite;
				yield return Action.TakeBlue;
				yield return Action.TakeGreen;
				yield return Action.TakeRed;
				yield return Action.TakeBlack;
				// take a card
				// build any card from table or hand
			}
			else if (actionCount == 1)
			{
				Action firstAction = this.actions[0];
				if (firstAction == Action.TakeWhite)
				{

				}
			}
		}

		public void Setup(Setup setup)
		{
			// shuffle decks separately
			this.ShuffleDecks();
			// reveal top four from each deck
			for (int i = 0; i < this.market.Length; i++)
			{
				int tier = (i / Rules.tiers);
				this.market[i] = decks[tier].Draw().id;
			}
			// shuffle and reveal nobles
			// populate tokens
			this.tokens[SupplyIndex][(int)Color.White] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Blue] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Green] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Red] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Black] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Gold] = Rules.goldCount;
			// determine starting player
			this.currentPlayer = this.randomizer.Next(setup.playerCount);
		}

		public GameState(int numPlayers, IRandomizer randomizer = null)
		{
			if(randomizer == null)
			{
				randomizer = new Randomizer();
			}
			this.randomizer = randomizer;
			this.tokens = new int[numPlayers][];
			for (int i = 0; i < numPlayers; i++)
			{
				this.tokens[i] = new int[6];
			}
			this.decks = new Deck[Rules.tiers];
			for (int i = 0; i < this.decks.Length; i++)
			{
				var cards = Rules.Cards;
				var first = cards.First(card => card.tier == i);
				var count = cards.Count(card => card.tier == i);
				this.decks[i] = new Deck(Rules.Cards, first.id, count);
			}
		}

		public bool IsValid()
		{
			// Ensure all tokens are accounted for
			int tokenCount = Rules.Setups[0].tokenCount;
			for (Color c = Color.White; c < Color.Gold; c++)
			{
				int sum = this.tokens[(int)c].Sum();
				if (sum != tokenCount)
				{
					return false;
				}
			}
			if (this.tokens[5].Sum() != Rules.goldCount)
			{
				return false;
			}
			// Ensure no player has more than 3 cards in hand

			// Ensure tableau has 12 cards

			// and that there are exactly 4 in each tier

			return true;
		}
	}

	public enum Color : byte
	{
		White,
		Blue,
		Green,
		Red,
		Black,
		Gold,
	}


	public class Setup
	{
		public int playerCount;
		public int tokenCount;
		public int nobleCount;
	}

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
	}

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
