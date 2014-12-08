namespace Splendor.Model
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

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
			get { return this.gameState.nobleVisiting.Where(nv => nv == GameState.SupplyIndex).Select((n, i) => Rules.Nobles[this.gameState.nobles[i]]).ToArray(); }
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
					this.gameState.currentPhase = Phase.NobleVisit;
					break;
				case Phase.NobleVisit:
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
