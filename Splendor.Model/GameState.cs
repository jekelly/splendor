using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor.Model
{
	public partial class GameState
	{
		private const int SupplyIndex = 4;

		private readonly IRandomizer randomizer;
		private readonly int playerCount;
		private readonly int[][] tokens;
		private readonly Deck[] decks;
		private readonly int[] market;
		private readonly int[][] hands;
		private readonly int[][] tableau;

		private static readonly IAction[] actions;

		private readonly IAction[] currentActions;
		private int actionCount;

		private int currentPlayer;
		
		static GameState()
		{
			List<IAction> actions = new List<IAction>();

			for (Color color = Color.White; color <= Color.Gold; color++)
			{
				actions.Add(new TakeTokenAction(color));
				actions.Add(new ReplaceTokenAction(color));
			}
			for (int i = 0; i < Rules.Cards.Length; i++)
			{
				actions.Add(new ReserveCardAction(Rules.Cards[i]));
				actions.Add(new BuildCardAction(Rules.Cards[i]));
			}
			GameState.actions = actions.ToArray();
		}
		
		public void ShuffleDecks()
		{
			for (int tier = 0; tier < this.decks.Length; tier++)
			{
				this.decks[tier].Shuffle(this.randomizer);
			}
		}

		public void Setup(Setup setup)
		{
			// shuffle decks separately
			this.ShuffleDecks();
			// reveal top four from each deck
			for (int i = 0; i < this.market.Length; i++)
			{
				int tier = (i / Rules.tiers);
				this.market[i] = decks[tier].Draw().id;
			}
			// shuffle and reveal nobles
			// populate tokens
			this.tokens[SupplyIndex][(int)Color.White] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Blue] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Green] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Red] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Black] = setup.tokenCount;
			this.tokens[SupplyIndex][(int)Color.Gold] = Rules.goldCount;
			// determine starting player
			this.currentPlayer = this.randomizer.Next(setup.playerCount);
		}

		public GameState(int numPlayers, IRandomizer randomizer = null)
		{
			if (randomizer == null)
			{
				randomizer = new Randomizer();
			}
			this.randomizer = randomizer;
			this.tokens = new int[5][];
			for (int i = 0; i < 5; i++)
			{
				this.tokens[i] = new int[6];
			}
			this.decks = new Deck[Rules.tiers];
			for (int i = 0; i < this.decks.Length; i++)
			{
				var cards = Rules.Cards;
				var first = cards.First(card => card.tier == i);
				var count = cards.Count(card => card.tier == i);
				this.decks[i] = new Deck(Rules.Cards, first.id, count);
			}
			// TODO: how many actions could someone possibly take on a single turn?
			this.actionCount = 0;
			this.currentActions = new IAction[6];
		}

		public bool IsValid()
		{
			// Ensure all tokens are accounted for
			int tokenCount = Rules.Setups[0].tokenCount;
			for (Color c = Color.White; c < Color.Gold; c++)
			{
				int sum = this.tokens[(int)c].Sum();
				if (sum != tokenCount)
				{
					return false;
				}
			}
			if (this.tokens[5].Sum() != Rules.goldCount)
			{
				return false;
			}
			// Ensure no player has more than 3 cards in hand

			// Ensure tableau has 12 cards

			// and that there are exactly 4 in each tier

			return true;
		}
	}

	

	

	

	

	

	

	

	

	
}
