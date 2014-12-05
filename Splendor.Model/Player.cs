using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	partial class Game
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
				get { return this.gameState.hands[this.playerIndex].Select(i => Rules.Cards[i]); }
			}

			public IEnumerable<Card> Tableau
			{
				get { return this.gameState.tableau[this.playerIndex].Select(i => Rules.Cards[i]); }
			}

			public int[] BuyingPower
			{
				get
				{
					int[] power = new int[6];
					for (int i = 0; i < 5; i++)
					{
						Color c = (Color)i;
						power[i] = this.gameState.tokens[playerIndex][i];
						int cards = 0;
						for (int j = 0; j < this.gameState.tableauSize[playerIndex]; j++)
						{
							if ((Color)Rules.Cards[this.gameState.tableau[playerIndex][j]].gives == c)
							{
								cards++;
							}
						}
						power[i] += cards;
					}
					power[(int)Color.Gold] = this.gameState.tokens[playerIndex][(int)Color.Gold];
					return power;
				}
			}
		}
	}
}
