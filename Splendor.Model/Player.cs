namespace Splendor.Model
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public partial class Game
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

			public int Gems(Color color)
			{
				return this.Tableau.Where(card => card.gives == (byte)color).Count();
			}

			public int Tokens(Color color)
			{
				return this.gameState.tokens[this.playerIndex][(int)color];
			}

			public int Score
			{
				get { return this.Tableau.Sum(card => card.value) + this.Nobles.Sum(noble => noble.value); }
			}

			public IEnumerable<Card> Hand
			{
				get { return this.gameState.hands[this.playerIndex].Where(i => i != Rules.SentinelCard.id).Select(i => Rules.Cards[i]); }
			}

			public IEnumerable<Card> Tableau
			{
				get { return this.gameState.tableau[this.playerIndex].Where(i => i != Rules.SentinelCard.id).Select(i => Rules.Cards[i]); }
			}

			public IEnumerable<Noble> Nobles
			{
				get { return this.gameState.nobleVisiting.Where(nv => nv == this.playerIndex).Select((n, i) => Rules.Nobles[this.gameState.nobles[i]]); }
			}

			private int[] tokens
			{
				get { return this.gameState.tokens[this.playerIndex]; }
			}

			private int[] supply
			{
				get { return this.gameState.tokens[GameState.SupplyIndex]; }
			}

			private int[] nobles
			{
				get { return this.gameState.nobles; }
			}

			private int[] hand
			{
				get { return this.gameState.hands[this.playerIndex]; }
			}

			private int handSize
			{
				get { return this.gameState.handSize[this.playerIndex]; }
				set { this.gameState.handSize[this.playerIndex] = value; }
			}

			private int[] tableau
			{
				get { return this.gameState.tableau[this.playerIndex]; }
			}

			private int tableauSize
			{
				get { return this.gameState.tableauSize[this.playerIndex]; }
				set { this.gameState.tableauSize[this.playerIndex] = value; }
			}

			public void GainToken(Color color)
			{
				this.tokens[(int)color]++;
				this.supply[(int)color]--;
			}

			public void SpendToken(Color color)
			{
				this.tokens[(int)color]--;
				this.supply[(int)color]++;
				this.gameState.debt[(int)color] = Math.Max(this.gameState.debt[(int)color] - 1, 0);
			}

			public void GainNoble(Noble noble)
			{
				for (int i = 0; i < this.nobles.Length; i++)
				{
					if (this.nobles[i] == noble.id)
					{
						this.gameState.nobleVisiting[i] = this.playerIndex;
						return;
					}
				}
				throw new InvalidOperationException("Tried to gain a noble that isn't part of the game.");
			}

			public void MoveCardToTableau(Card card)
			{
				int cardId = card.id;
				// Add card to tableau
				this.tableau[this.tableauSize] = cardId;
				this.tableauSize++;
				// check hand
				for (int i = 0; i < this.handSize; i++)
				{
					if (this.hand[i] == cardId)
					{
						// remove card from hand
						this.hand[i] = Rules.SentinelCard.id;
						this.handSize--;
						return;
					}
				}
				// if not hand, must be from market
				int[] market = this.gameState.market;
				Deck deck = this.gameState.decks[card.tier];
				for (int i = 0; i < market.Length; i++)
				{
					if (market[i] == cardId)
					{
						// replace card in market
						market[i] = deck.Draw().id;
						return;
					}
				}
				// calculate cost of card relative to player assets
				this.gameState.debt[(int)Color.White] = Math.Max(0, card.costWhite - this.Gems(Color.White));
				this.gameState.debt[(int)Color.Black] = Math.Max(0, card.costBlack - this.Gems(Color.Black));
				this.gameState.debt[(int)Color.Blue] = Math.Max(0, card.costBlue - this.Gems(Color.Blue));
				this.gameState.debt[(int)Color.Green] = Math.Max(0, card.costGreen - this.Gems(Color.Green));
				this.gameState.debt[(int)Color.Red] = Math.Max(0, card.costRed - this.Gems(Color.Red));
			}

			public void MoveCardToHand(Card card)
			{
				int cardId = card.id;
				if (this.handSize >= Rules.MaxHandSize)
				{
					throw new InvalidOperationException("Too many cards in hand to add another.");
				}
				// Add card to hand at first available index
				for (int i = 0; i < Rules.MaxHandSize; i++)
				{
					if (this.hand[i] == Rules.SentinelCard.id)
					{
						this.hand[i] = cardId;
						break;
					}
				}
				this.handSize++;
				int[] market = this.gameState.market;
				Deck deck = this.gameState.decks[card.tier];
				// remove from market
				for (int i = 0; i < market.Length; i++)
				{
					if (market[i] == cardId)
					{
						// replace card in market
						market[i] = deck.Draw().id;
						return;
					}
				}
				if (this.supply[(int)Color.Gold] > 0)
				{
					this.GainToken(Color.Gold);
				}
			}
		}
	}
}
