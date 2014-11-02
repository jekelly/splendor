﻿using System;
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

		public Game(int numPlayers = 2, IRandomizer randomizer = null)
		{
			if(randomizer == null)
			{
				randomizer = new Randomizer();
			}
			this.randomizer = randomizer;
			this.gameState = new GameState();
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

		public int CurrentPlayer
		{
			get { return this.gameState.currentPlayer; }
		}

		public IPlayer GetPlayer(int playerIndex)
		{
			return new Player(this.gameState, playerIndex);
		}

		public void Setup(Setup setup)
		{
			this.gameState.Setup(setup, this.randomizer);
		}

		public void SpendToken(int playerIndex, Color color)
		{
			this.gameState.SpendToken(playerIndex, color);
		}

		public void GainToken(int playerIndex, Color color)
		{
			this.gameState.TakeToken(playerIndex, color);
		}

		public void BuyCard(int playerIndex, Card card)
		{
			this.gameState.BuyCard(playerIndex, card);
		}
	}
}
