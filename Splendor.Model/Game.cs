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
}
