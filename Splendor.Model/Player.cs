using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
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
	}
}
