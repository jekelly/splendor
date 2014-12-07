using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public interface IChooser
	{
		IAction Choose(IEnumerable<IAction> actions);
	}

	class Controller
	{
		private IGame game;
		private readonly IChooser[] choosers;

		public void RunGame()
		{
			while (game.CurrentPhase != Phase.GameOver)
			{
				game.Step(this.choosers[this.game.CurrentPlayerIndex]);
			}
		}
	}

	public enum Phase
	{
		NotStarted,
		Choose,
		Pay,
		EndTurn,
		GameOver,
	}

	public partial class Game : IGame
	{
		private readonly IRandomizer randomizer;
		private readonly GameState gameState;

		public Game(Setup setup, IRandomizer randomizer = null)
		{
			if (randomizer == null)
			{
				randomizer = new Randomizer();
			}
			this.randomizer = randomizer;
			this.gameState = new GameState(setup, this.randomizer);
		}

		public void Step(IChooser chooser)
		{
			var actions = this.AvailableActions;
			if (actions.Any())
			{
				var action = chooser.Choose(actions);
				action.Execute(this);
			}
			this.NextPhase();
		}

		public IEnumerable<IAction> AvailableActions
		{
			get
			{
				for (int i = 0; i < Rules.Actions.Length; i++)
				{
					IAction action = Rules.Actions[i];
					if (action.CanExecute(this))
					{
						yield return action;
					}
				}
				//int moveIndex = 0;
				//int[] currentPlayerTokens = this.gameState.tokens[this.CurrentPlayerIndex];
				//int[] supplyTokens = this.gameState.tokens[GameState.SupplyIndex];
				//// generate permutations of 3 different
				//for (int i = 0; i < 5; i++)
				//{
				//	for (int j = i + 1; j < 5; j++)
				//	{
				//		for (int k = j + 1; k < 5; k++)
				//		{
				//			this.availableMoves[moveIndex++] = supplyTokens[i] > 0 && supplyTokens[j] > 0 && supplyTokens[k] > 0;
				//		}
				//	}
				//}
				//// generate permutations of 2 identical
				//for (int i = 0; i < 5; i++)
				//{
				//	this.availableMoves[moveIndex++] = supplyTokens[i] >= 4;
				//}
				//// generate all token return moves
				//for (int i = 0; i < 5; i++)
				//{
				//	this.availableMoves[moveIndex++] = currentPlayerTokens.Sum() > 10;
				//}
				//// generate all build actions
				//int[] buyingPower = CalculateBuyingPower(this.gameState, this.CurrentPlayerIndex);
				//for (int i = 0; i < Rules.Cards.Length; i++)
				//{
				//	Card card = Rules.Cards[i];
				//	bool available = this.gameState.market.Contains(i) || this.gameState.hands[this.CurrentPlayerIndex].Contains(i);
				//	bool buildable = card.CanBuy(buyingPower);
				//	this.availableMoves[moveIndex++] = available && buildable;
				//}
				//// generate all reserve actions
				//for (int i = 0; i < Rules.Cards.Length; i++)
				//{
				//	this.availableMoves[moveIndex++] = this.gameState.market.Contains(i);
				//}
				//return Rules.Moves.Where((m, i) => this.availableMoves[i]);
			}
		}

		//public Game(Game game)
		//{
		//	this.gameState = game.gameState.Clone();
		//}

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
			private set { this.gameState.currentPlayer = value; }
		}

		public IPlayer CurrentPlayer
		{
			get { return this.GetPlayer(this.CurrentPlayerIndex); }
		}

		public Phase CurrentPhase
		{
			get { return this.gameState.currentPhase; }
		}

		public IPlayer GetPlayer(int playerIndex)
		{
			return new Player(this.gameState, playerIndex);
		}

		public void NextPhase()
		{
			switch (this.gameState.currentPhase)
			{
				case Phase.Choose:
					this.gameState.currentPhase = Phase.Pay;
					break;
				case Phase.Pay:
					Debug.Assert(this.gameState.debt.Sum() == 0);
					this.gameState.currentPhase = Phase.EndTurn;
					break;
				case Phase.EndTurn:
					if (this.gameState.lastPlayerIndex == -1)
					{
						if (this.CurrentPlayer.Score >= Rules.RequiredPoints)
						{
							this.gameState.lastPlayerIndex = this.CurrentPlayerIndex;
						}
					}
					this.gameState.currentPhase = Phase.Choose;
					this.CurrentPlayerIndex = (this.CurrentPlayerIndex + 1) % this.gameState.numPlayers;
					if (this.CurrentPlayerIndex == this.gameState.lastPlayerIndex)
					{
						this.gameState.currentPhase = Phase.GameOver;
						this.CurrentPlayerIndex = -1;
					}

					break;
				case Phase.GameOver:
					break;
				case Phase.NotStarted:
				default:
					throw new InvalidOperationException();
			}
		}

	}
}
