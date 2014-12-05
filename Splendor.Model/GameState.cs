using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Model
{
	class Move 
	{
		
	}

	class TakeTokensMove : Move
	{
		public Color[] Tokens { get; private set; }

		public TakeTokensMove(params Color[] tokens)
		{
			this.Tokens = tokens;
		}
	}

	class BuildCardMove : Move
	{
		public int Id { get; private set; }

		public BuildCardMove(int id)
		{
			this.Id = id;
		}
	}

	class ReserveCardMove : Move
	{
		public int Id { get; private set; }

		public ReserveCardMove(int id)
		{
			this.Id = id;
		}
	}

	class ReturnTokenMove : Move
	{
		public Color Token { get; private set; }

		public ReturnTokenMove(Color token)
		{
			this.Token = token;
		}
	}

	public static class Moves
	{
	}

	partial class Game
	{
		protected sealed class GameState
		{
			public const int SupplyIndex = 4;

			public int currentPlayer;
			public readonly int numPlayers;

			public readonly int[][] tokens;

			public readonly Deck[] decks;

			public readonly int[] market;

			public readonly int[][] hands;
			public readonly int[] handSize;

			public readonly int[][] tableau;
			public readonly int[] tableauSize;

			public readonly int[] actions;
			public int actionsSize;
			
			public void ShuffleDecks(IRandomizer randomizer)
			{
				for (int tier = 0; tier < this.decks.Length; tier++)
				{
					this.decks[tier].Shuffle(randomizer);
				}
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
				for (int i = 0; i < this.numPlayers; i++)
				{
					this.hands[i] = new int[Rules.MaxHandSize];
				}
				this.handSize = new int[this.numPlayers];
				this.tableau = new int[this.numPlayers][];
				for (int i = 0; i < this.numPlayers; i++)
				{
					this.tableau[i] = new int[Rules.MaxTableauSize];
				}
				this.tableauSize = new int[this.numPlayers];
				// determine starting player
				this.currentPlayer = randomizer.Next(setup.playerCount);
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
				if (this.tokens[5].Sum() != Rules.GoldCount)
				{
					return false;
				}
				// Ensure no player has more than 3 cards in hand

				// Ensure tableau has 12 cards

				// and that there are exactly 4 in each tier

				return true;
			}

			public void SpendToken(int playerIndex, Color color)
			{
				this.tokens[playerIndex][(int)color]--;
				this.tokens[GameState.SupplyIndex][(int)color]++;
			}

			internal bool CanTakeToken(Color color)
			{
				return this.tokens[GameState.SupplyIndex][(int)color] > 0;
			}

			public void TakeToken(int playerIndex, Color color)
			{
				this.tokens[playerIndex][(int)color]++;
				this.tokens[GameState.SupplyIndex][(int)color]--;
			}

			public void MoveCardToTableau(int playerIndex, Card card)
			{
				int cardId = Array.IndexOf(Rules.Cards, card);
				int tableauIndex = this.tableauSize[playerIndex];
				var hand = this.hands[playerIndex];
				var handSize = this.handSize[playerIndex];
				// Add card to tableau
				this.tableau[playerIndex][tableauIndex] = cardId;
				this.tableauSize[playerIndex]++;
				// check hand
				for (int i = 0; i < handSize; i++)
				{
					if (hand[i] == cardId)
					{
						// remove card from hand
						hand[i] = Rules.SentinelCard.id;
						this.handSize[playerIndex]--;
						return;
					}
				}
				// if not hand, must be from market
				for (int i = 0; i < this.market.Length; i++)
				{
					if (this.market[i] == cardId)
					{
						// replace card in market
						this.market[i] = this.decks[card.tier].Draw().id;
						return;
					}
				}
			}

			public void MoveCardToHand(int playerIndex, Card card)
			{
				int cardId = Array.IndexOf(Rules.Cards, card);
				int handIndex = this.handSize[playerIndex];
				if (handIndex >= Rules.MaxHandSize)
				{
					throw new InvalidOperationException("Too many cards in hand to add another.");
				}
				var hand = this.hands[playerIndex];
				var handSize = this.handSize[playerIndex];
				// Add card to hand
				this.hands[playerIndex][handIndex] = cardId;
				this.handSize[playerIndex]++;
				// remove from market
				for (int i = 0; i < this.market.Length; i++)
				{
					if (this.market[i] == cardId)
					{
						// replace card in market
						this.market[i] = this.decks[card.tier].Draw().id;
						return;
					}
				}
			}
		}
	}
}