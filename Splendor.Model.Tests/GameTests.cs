using System;
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
		public void MoveCardToTableau_RemovesCardFromMarket()
		{
			IGame game = Game();
			var card = game.Market[0];
			game.MoveCardToTableau(0, card);
			game.Market.Should().NotContain(card);
		}

		[Fact]
		public void MoveCardToTableau_AddsCardToTableau()
		{
			IGame game = Game();
			var card = game.Market[0];
			game.MoveCardToTableau(0, card);
			game.GetPlayer(0).Tableau.Should().Contain(card);
		}

		[Fact]
		public void MoveCardToHand_ShouldRemoveCardFromTableau()
		{
			IGame game = Game();
			var card = game.Market[0];
			game.MoveCardToHand(0, card);
			game.Market.Should().NotContain(card);
		}

		[Fact]
		public void MoveCardToHand_AddsCardToHand()
		{
			IGame game = Game();
			var card = game.Market[0];
			game.MoveCardToHand(0, card);
			game.GetPlayer(0).Hand.Should().Contain(card);
		}

		[Fact]
		public void MoveCardToHand_ShouldThrowIfHandFull()
		{
			IGame game = Game();
			game.MoveCardToHand(0, game.Market[0]);
			game.MoveCardToHand(0, game.Market[0]);
			game.MoveCardToHand(0, game.Market[0]);
			Action a = () => { game.MoveCardToHand(0, game.Market[0]); };
			a.ShouldThrow<InvalidOperationException>();
		}

		[Theory]
		[InlineData(Rules.GoldCount, 1)]
		[InlineData(0, 0)]
		public void MoveCardToHand_GainsAGoldIfAvailable(int availableGold, int expectedGold)
		{
			IGame game = Game();
			var card = game.Market[0];
			int count = Rules.GoldCount - availableGold;
			for (int i = 0; i < count; i++)
			{
				game.MoveCardToHand(1, card);
				game.MoveCardToTableau(1, card);
			}
			game.MoveCardToHand(0, card);
			game.GetPlayer(0).Tokens(Color.Gold).Should().Be(expectedGold);
		}

		[Fact]
		public void MoveCardToTableau_RemovesCardFromHand()
		{
			IGame game = Game();
			var card = game.Market[0];
			game.MoveCardToHand(0, card);
			game.MoveCardToTableau(0, card);
			game.GetPlayer(0).Hand.Should().NotContain(card);
			game.GetPlayer(0).Tableau.Should().Contain(card);
		}

		private static IGame Game(int numPlayers = 2, IRandomizer randomizer = null)
		{
			return new Game(Rules.Setups[numPlayers - 2], randomizer);
		}
	}
}
