using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
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

			public int Tokens(Color color)
			{
				return this.gameState.tokens[this.playerIndex][(int)color];
			}

			public IEnumerable<Card> Hand
			{
				get { return this.gameState.hands[this.playerIndex].Where(i => i != Rules.SentinelCard.id).Select(i => Rules.Cards[i]); }
			}

			public IEnumerable<Card> Tableau
			{
				get { return this.gameState.tableau[this.playerIndex].Where(i => i != Rules.SentinelCard.id).Select(i => Rules.Cards[i]); }
			}

			private int[] tokens
			{
				get { return this.gameState.tokens[this.playerIndex]; }
			}

			private int[] supply
			{
				get { return this.gameState.tokens[GameState.SupplyIndex]; }
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
			}

			public void MoveCardToTableau(Card card)
			{
				this.MoveCardToTableau(playerIndex, card);
				this.gameState.TrackDebt(card);
			}

			public void MoveCardToHand(Card card)
			{
				this.MoveCardToHand(playerIndex, card);
				if (this.supply[(int)Color.Gold] > 0)
				{
					this.GainToken(Color.Gold);
				}
			}

			private void MoveCardToTableau(int playerIndex, Card card)
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
			}

			private void MoveCardToHand(int playerIndex, Card card)
			{
				int cardId = card.id;
				if (this.handSize >= Rules.MaxHandSize)
				{
					throw new InvalidOperationException("Too many cards in hand to add another.");
				}
				// Add card to hand
				this.hand[this.handSize] = cardId;
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
			}
		}
	}
}
