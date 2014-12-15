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
			private readonly int index;
			private readonly Game game;

			// caches
			private IEnumerable<Card> handCache;

			public Player(Game game, int playerIndex)
			{
				this.game = game;
				this.gameState = game.gameState;
				this.index = playerIndex;
			}

			public int Index
			{
				get { return this.index; }
			}

			public int Gems(Color color)
			{
				return this.gems[(int)color];
			}

			public int GemCount { get { return this.gameState.gems[this.index].Sum(); } }

			public int Tokens(Color color)
			{
				return this.gameState.tokens[this.index][(int)color];
			}

			public int TokenCount
			{
				get
				{
					return this.gameState.tokens[this.index].Sum();
				}
			}

			public int Score
			{
				get { return this.Tableau.Sum(card => card.value) + this.Nobles.Sum(noble => noble.value); }
			}

			public IEnumerable<Card> Hand
			{
				get
				{
					if (this.handCache == null)
					{
						this.handCache = this.gameState.hands[this.index].Where(i => i != Rules.SentinelCard.id).Select(i => Rules.Cards[i]);
					}
					return this.handCache;
				}
			}

			public IEnumerable<Card> Tableau
			{
				get { return this.gameState.tableau[this.index].Where(i => i != Rules.SentinelCard.id).Select(i => Rules.Cards[i]); }
			}

			public IEnumerable<Noble> Nobles
			{
				get { return this.gameState.nobleVisiting.Where(nv => nv == this.index).Select((n, i) => Rules.Nobles[this.gameState.nobles[i]]); }
			}

			private int[] gems
			{
				get { return this.gameState.gems[this.index]; }
			}

			private int[] tokens
			{
				get { return this.gameState.tokens[this.index]; }
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
				get { return this.gameState.hands[this.index]; }
			}

			private int handSize
			{
				get { return this.gameState.handSize[this.index]; }
				set { this.gameState.handSize[this.index] = value; }
			}

			private int[] tableau
			{
				get { return this.gameState.tableau[this.index]; }
			}

			private int tableauSize
			{
				get { return this.gameState.tableauSize[this.index]; }
				set { this.gameState.tableauSize[this.index] = value; }
			}

			public void GainToken(Color color)
			{
				this.tokens[(int)color]++;
				this.supply[(int)color]--;
			}

			public void ReturnToken(Color color)
			{
				this.tokens[(int)color]--;
				this.supply[(int)color]++;
			}

			public void SpendToken(Color color)
			{
				this.ReturnToken(color);
				if (color == Color.Gold)
				{
					this.gameState.debt[(int)color]++;
				}
				else
				{
					this.gameState.debt[(int)color] = Math.Max(this.gameState.debt[(int)color] - 1, 0);
				}
			}

			public void GainNoble(Noble noble)
			{
				for (int i = 0; i < this.nobles.Length; i++)
				{
					if (this.nobles[i] == noble.id)
					{
						this.gameState.nobleVisiting[i] = this.index;
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
				bool inHand = false;
				for (int i = 0; i < this.handSize; i++)
				{
					if (this.hand[i] == cardId)
					{
						// remove card from hand
						inHand = true;
						this.hand[i] = Rules.SentinelCard.id;
						this.handSize--;
						// TODO need test for hand cache
						this.handCache = null;
						break;
					}
				}
				if (!inHand)
				{
					// if not hand, must be from market
					int[] market = this.gameState.market;
					Deck deck = this.gameState.decks[card.tier];
					for (int i = 0; i < market.Length; i++)
					{
						if (market[i] == cardId)
						{
							// replace card in market
							market[i] = deck.Draw().id;
							this.game.market = null;
							break;
						}
					}
				}
				// TODO need test for gem update
				// update player gems
				this.gems[card.gives]++;
				// TODO need test for cost
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
				// TODO need test for hand cache
				this.handCache = null;

				int[] market = this.gameState.market;
				Deck deck = this.gameState.decks[card.tier];
				// remove from market
				for (int i = 0; i < market.Length; i++)
				{
					if (market[i] == cardId)
					{
						// replace card in market
						market[i] = deck.Draw().id;
						this.game.market = null;
						break;
					}
				}
			}
		}
	}
}
