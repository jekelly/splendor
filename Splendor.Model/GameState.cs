using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Model
{
	partial class Game
	{
		protected sealed class GameState
		{
			public const int SupplyIndex = 4;

			public int currentPlayer;
			public readonly int playerIndex;

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

			public void Setup(Setup setup, IRandomizer randomizer)
			{
				// shuffle decks separately
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
				// determine starting player
				this.currentPlayer = randomizer.Next(setup.playerCount);
			}

			public GameState()
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
				// TODO: how many actions could someone possibly take on a single turn?
				//this.actionCount = 0;
				//this.currentActions = new IAction[6];
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

			public void TakeToken(int playerIndex, Color color)
			{
				this.tokens[playerIndex][(int)color]++;
				this.tokens[GameState.SupplyIndex][(int)color]--;
			}

			public void BuyCard(int playerIndex, Card card)
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
						hand[i] = -1;
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
		}
	}
}