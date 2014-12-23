namespace Splendor.Model
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	public partial class Game : IGame
	{
		private const int MaxTurns = 500;
		private readonly IRandomizer randomizer;
		private readonly GameState gameState;
		private readonly IEventSink eventSink;
		private readonly IPlayer[] players;
		// caches
		private Noble[] nobles;
		private Card[] market;

		private Game(Game otherGame)
		{
			// TODO: clone randomizer?
			this.randomizer = new Randomizer();
			this.eventSink = NullEventSink.Instance;
			this.gameState = otherGame.gameState.Clone();
			this.players = new IPlayer[otherGame.Players.Length];
			for (int i = 0; i < this.players.Length; i++)
			{
				this.players[i] = new Player(this, i);
			}
		}

		public Game(Setup setup, IRandomizer randomizer = null, IEventSink eventSink = null)
		{
			if (randomizer == null)
			{
				randomizer = new Randomizer();
			}
			this.randomizer = randomizer;
			this.gameState = new GameState(setup, this.randomizer);
			this.eventSink = eventSink ?? NullEventSink.Instance;
			this.players = new IPlayer[setup.playerCount];
			for (int i = 0; i < this.players.Length; i++)
			{
				this.players[i] = new Player(this, i);
			}
		}

		public int Turns { get { return this.gameState.turn; } }

		public IEventSink EventSink { get { return this.eventSink; } }

		public void Step(IAction action)
		{
			if (action != null)
			{
				Debug.Assert(this.AvailableActions.Contains(action));
				action.Execute(this);
			}
			this.NextPhase();
		}

		class ActionCollection : IEnumerable<IAction>
		{

			public IEnumerator<IAction> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<IAction> AvailableActions
		{
			get
			{
				IEnumerable<IAction> candidates = null;
				switch (this.CurrentPhase)
				{
					case Phase.Choose:
						candidates = new MultiArray<IAction>(
							new ReferenceArray<IAction>(Rules.BuildActions, this.gameState.market, this.gameState.hands[this.CurrentPlayerIndex]),
							new ReferenceArray<IAction>(Rules.ReserveActions, this.gameState.market),
							new ReferenceArray<IAction>(Rules.TakeActions));
						break;
					case Phase.Pay:
					case Phase.EndTurn:
						candidates = Rules.ReplaceActions;
						break;
					case Phase.NobleVisit:
						candidates = Rules.NobleActions;
						break;
					case Phase.GameOver:
					case Phase.NotStarted:
					default:
						candidates = Enumerable.Empty<IAction>();
						break;
				}
				List<IAction> actions = new List<IAction>();
				foreach (IAction candidate in candidates)
				{
					if (candidate.CanExecute(this))
					{
						actions.Add(candidate);
					}
				}
				return actions;
			}
		}

		public int Supply(Color color)
		{
			return this.gameState.tokens[GameState.SupplyIndex][(int)color];
		}

		public int Debt(Color color)
		{
			if (color == Color.Gold)
			{
				return this.gameState.debt.Sum();
			}
			return this.gameState.debt[(int)color];
		}

		public Card[] Market
		{
			get
			{
				if (this.market == null)
				{
					this.market = this.gameState.market.Select(i => Rules.Cards[i]).ToArray();
				}
				return this.market;
			}
		}

		public Noble[] Nobles
		{
			get
			{
				if (this.nobles == null)
				{
					List<Noble> nobles = new List<Noble>();
					for(int i = 0; i < this.gameState.nobles.Length; i++)
					{
						if (this.gameState.nobleVisiting[i] == GameState.SupplyIndex)
						{
							nobles.Add(Rules.Nobles[this.gameState.nobles[i]]);
						}
					}
					this.nobles = nobles.ToArray();
				}
				return this.nobles;
			}
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

		public IPlayer[] Players
		{
			get { return this.players; }
		}

		public Phase CurrentPhase
		{
			get { return this.gameState.currentPhase; }
		}

		public IPlayer GetPlayer(int playerIndex)
		{
			return this.players[playerIndex];
		}

		public IGame Clone()
		{
			return new Game(this);
		}

		public void NextPhase()
		{
			switch (this.gameState.currentPhase)
			{
				case Phase.Choose:
					this.gameState.currentPhase = Phase.Pay;
					break;
				case Phase.Pay:
					if (this.gameState.debt.Take(Rules.CardinalColorCount).Sum() == this.gameState.debt[(int)Color.Gold])
					{
						for (int i = 0; i < 6; i++)
						{
							this.gameState.debt[i] = 0;
						}
						this.gameState.currentPhase = Phase.NobleVisit;
					}
					break;
				case Phase.NobleVisit:
					this.gameState.currentPhase = Phase.EndTurn;
					break;
				case Phase.EndTurn:
					if (this.CurrentPlayer.TokenCount <= Rules.MaxTokensHeld)
					{
						this.EventSink.SummarizeGame(this);
						if (this.gameState.lastPlayerIndex == -1)
						{
							if (this.CurrentPlayer.Score >= Rules.RequiredPoints)
							{
								this.gameState.lastPlayerIndex = this.CurrentPlayerIndex;
							}
						}
						this.gameState.currentPhase = Phase.Choose;
						this.CurrentPlayerIndex = (this.CurrentPlayerIndex + 1) % this.gameState.numPlayers;
						this.gameState.turn++;
						if (this.CurrentPlayerIndex == this.gameState.lastPlayerIndex || this.gameState.turn > MaxTurns)
						{
							this.gameState.currentPhase = Phase.GameOver;
							this.CurrentPlayerIndex = -1;
						}
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
