using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	partial class GameState
	{
		class Game : IGame
		{
			private readonly GameState gameState;

			public Game(GameState gameState)
			{
				this.gameState = gameState;
			}

			public IList<IAction> Actions
			{
				get
				{
					return this.gameState.currentActions.Take(this.gameState.actionCount).ToList();
				}
			}

			public int[] Supply
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

			public int CurrentPlayer
			{
				get { return this.gameState.currentPlayer; }
			}

			public void GainToken(int playerIndex, Color color)
			{
				this.gameState.tokens[playerIndex][(int)color]++;
				this.Supply[(int)color]--;
			}
		}
	}
}
