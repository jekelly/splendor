using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	partial class Game : IGame
	{
		private readonly IRandomizer randomizer;
		private readonly GameState gameState;
		private BitArray availableMoves;

		public Game(Setup setup, IRandomizer randomizer = null)
		{
			if(randomizer == null)
			{
				randomizer = new Randomizer();
			}
			this.randomizer = randomizer;
			this.gameState = new GameState(setup, this.randomizer);
			this.availableMoves = new BitArray(Rules.Moves.Length);
		}

		public IEnumerable<Move> AvailableMoves
		{
			get
			{
				int moveIndex = 0;
				int[] currentPlayerTokens = this.gameState.tokens[this.CurrentPlayerIndex];
				int[] supplyTokens = this.gameState.tokens[GameState.SupplyIndex];
				// generate permutations of 3 different
				for (int i = 0; i < 5; i++)
				{
					for (int j = i + 1; j < 5; j++)
					{
						for (int k = j + 1; k < 5; k++)
						{
							this.availableMoves[moveIndex++] = supplyTokens[i] > 0 && supplyTokens[j] > 0 && supplyTokens[k] > 0;
						}
					}
				}
				// generate permutations of 2 identical
				for (int i = 0; i < 5; i++)
				{
					this.availableMoves[moveIndex++] = supplyTokens[i] >= 4;
				}
				// generate all token return moves
				for (int i = 0; i < 5; i++)
				{
					this.availableMoves[moveIndex++] = currentPlayerTokens.Sum() > 10;
				}
				// generate all build actions
				int[] buyingPower = CalculateBuyingPower(this.gameState, this.CurrentPlayerIndex);
				for (int i = 0; i < Rules.Cards.Length; i++)
				{
					Card card = Rules.Cards[i];
					bool available = this.gameState.market.Contains(i) || this.gameState.hands[this.CurrentPlayerIndex].Contains(i);
					bool buildable = card.CanBuy(buyingPower);
					this.availableMoves[moveIndex++] = available && buildable;
				}
				// generate all reserve actions
				for (int i = 0; i < Rules.Cards.Length; i++)
				{
					this.availableMoves[moveIndex++] = this.gameState.market.Contains(i);
				}
				return Rules.Moves.Where((m, i) => this.availableMoves[i]);
			}
		}

		private static int[] CalculateBuyingPower(GameState gameState, int playerIndex)
		{
			int[] power = new int[6];
			for (int i = 0; i < 5; i++)
			{
				Color c = (Color)i;
				power[i] = gameState.tokens[playerIndex][i];
				int cards = 0;
				for(int j = 0; j < gameState.tableauSize[playerIndex]; j++)
				{
					if ((Color)Rules.Cards[gameState.tableau[playerIndex][j]].gives == c)
					{
						cards++;
					}
				}
				power[i] += cards;
			}
			power[(int)Color.Gold] = gameState.tokens[playerIndex][(int)Color.Gold];
			return power;
		}


		//public Game(Game game)
		//{
		//	this.gameState = game.gameState.Clone();
		//}

		public IList<IAction> Actions
		{
			get
			{
				return this.gameState.actions.Take(this.gameState.actionsSize).Select(actionIndex => Rules.Actions[actionIndex]).ToList();
			}
		}

		public int Supply(Color color)
		{
			return this.gameState.tokens[GameState.SupplyIndex][(int)color];
		}

		public Card[] Market
		{
			get { return this.gameState.market.Select(i => Rules.Cards[i]).ToArray(); }
		}

		public Noble[] Nobles
		{
			get { throw new NotImplementedException(); }
		}

		public int CurrentPlayerIndex
		{
			get { return this.gameState.currentPlayer; }
		}

		public IPlayer CurrentPlayer
		{
			get { return this.GetPlayer(this.CurrentPlayerIndex); }
		}

		public IPlayer GetPlayer(int playerIndex)
		{
			return new Player(this.gameState, playerIndex);
		}

		public void SpendToken(int playerIndex, Color color)
		{
			this.gameState.SpendToken(playerIndex, color);
		}

		public void GainToken(int playerIndex, Color color)
		{
			this.gameState.TakeToken(playerIndex, color);
		}

		public void MoveCardToTableau(int playerIndex, Card card)
		{
			this.gameState.MoveCardToTableau(playerIndex, card);
		}

		public void MoveCardToHand(int playerIndex, Card card)
		{
			this.gameState.MoveCardToHand(playerIndex, card);
			if(this.gameState.CanTakeToken(Color.Gold))
			{
				this.gameState.TakeToken(playerIndex, Color.Gold);
			}
		}
	}
}
