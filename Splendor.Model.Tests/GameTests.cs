using System;
using NSubstitute;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;

namespace Splendor.Model.Tests
{
	public class GameTests
	{
		private static readonly IRandomizer randomizer = new Randomizer(0);

		[Theory]
		[InlineData(2, Color.Black, 4)]
		[InlineData(3, Color.Black, 5)]
		[InlineData(4, Color.Black, 6)]
		[InlineData(2, Color.Gold, 5)]
		[InlineData(3, Color.Gold, 5)]
		[InlineData(4, Color.Gold, 5)]
		public void Setup_SetsCorrentNumberOfTokens(int numPlayers, Color color, int expectedTokenCount)
		{
			IGame game = Game(numPlayers);
			game.Supply(color).Should().Be(expectedTokenCount);
		}

		[Fact]
		public void Setup_PopulatesMarket()
		{
			IGame game = Game();
			game.Market.Should().NotContain(Rules.SentinelCard);
			game.Market.Should().HaveCount(Rules.MarketSize);
		}

		[Fact]
		public void Actions_GeneratedProperly()
		{
			Rules.Actions.Should().HaveCount(453);
		}
		
		[Fact]
		public void InitialGameState_CalculatesAvailableActions()
		{
			IGame g = Game();
			var results = g.AvailableActions;
			// 3 unique: 5 choose 3 = 10
			// 2 same: 5 colors
			// 12 cards in market to reserve: 12
			// = 27 choices on first turn
			results.Should().HaveCount(27);
		}

		private static IGame Game(int numPlayers = 2, IRandomizer randomizer = null)
		{
			return new Game(Setups.All[numPlayers - 2], randomizer);
		}
	}
}
