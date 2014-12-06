using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace Splendor.Model.Tests
{
	public class PlayerTests
	{
		//[Fact]
		//public void MoveCardToTableau_RemovesCardFromMarket()
		//{
		//	Player player = new 
		//	IGame game = Game();
		//	var card = game.Market[0];
		//	game.MoveCardToTableau(0, card);
		//	game.Market.Should().NotContain(card);
		//}

		//[Fact]
		//public void MoveCardToTableau_AddsCardToTableau()
		//{
		//	IGame game = Game();
		//	var card = game.Market[0];
		//	game.MoveCardToTableau(0, card);
		//	game.GetPlayer(0).Tableau.Should().Contain(card);
		//}

		//[Fact]
		//public void MoveCardToHand_ShouldRemoveCardFromTableau()
		//{
		//	IGame game = Game();
		//	var card = game.Market[0];
		//	game.MoveCardToHand(0, card);
		//	game.Market.Should().NotContain(card);
		//}

		//[Fact]
		//public void MoveCardToHand_AddsCardToHand()
		//{
		//	IGame game = Game();
		//	var card = game.Market[0];
		//	game.MoveCardToHand(0, card);
		//	game.GetPlayer(0).Hand.Should().Contain(card);
		//}

		//[Fact]
		//public void MoveCardToHand_ShouldThrowIfHandFull()
		//{
		//	IGame game = Game();
		//	game.MoveCardToHand(0, game.Market[0]);
		//	game.MoveCardToHand(0, game.Market[0]);
		//	game.MoveCardToHand(0, game.Market[0]);
		//	Action a = () => { game.MoveCardToHand(0, game.Market[0]); };
		//	a.ShouldThrow<InvalidOperationException>();
		//}

		//[Theory]
		//[InlineData(Rules.GoldCount, 1)]
		//[InlineData(0, 0)]
		//public void MoveCardToHand_GainsAGoldIfAvailable(int availableGold, int expectedGold)
		//{
		//	IGame game = Substitute.For<IGame>();
		//	var card = new Card();
		//	game.Market.Returns(new Card[] { card });
		//	game.Supply(Color.Gold).Returns(availableGold);
		//	game.MoveCardToHand(0, card);
		//	game.GetPlayer(0).Tokens(Color.Gold).Should().Be(expectedGold);
		//}

		//[Fact]
		//public void MoveCardToTableau_RemovesCardFromHand()
		//{
		//	IGame game = Game();
		//	var card = game.Market[0];
		//	game.MoveCardToHand(0, card);
		//	game.MoveCardToTableau(0, card);
		//	game.GetPlayer(0).Hand.Should().NotContain(card);
		//	game.GetPlayer(0).Tableau.Should().Contain(card);
		//}
	}
}
