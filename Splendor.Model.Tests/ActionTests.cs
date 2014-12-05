using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Xunit.Extensions;

namespace Splendor.Model.Tests
{
	public class ActionTests
	{
		[Fact]
		public void CanExecuteNull_ReturnsFalse()
		{
			TakeTokensAction takeTokensAction = new TakeTokensAction(Color.Green);
			takeTokensAction.CanExecute(null);
		}

		[Theory]
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new Color[0], true, false, true)]								// can pay from tokens, in market
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new Color[0], false, true, true)]								// can pay from tokens, in hand
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new Color[0], false, false, false)]							// can pay from tokens, not available
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new Color[0], false, false, false)]							// can pay from tokens, not available
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new Color[] { Color.White }, true, false, true)]				// can pay from tableau, in market
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 1 }, new Color[0], true, false, true)]								// can pay with gold, in market
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new Color[0], true, false, true)]								// can pay with tokens or gold, in market
		[InlineData(new int[] { 3, 1, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new Color[] { Color.White, Color.White }, false, true, true)]	// can pay with tokens and tableau, in hand
		[InlineData(new int[] { 4, 1, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new Color[] { Color.White, Color.White }, false, true, false)]	// cannot pay with tokens and tableau, in hand
		[InlineData(new int[] { 3, 1, 1, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new Color[] { Color.White, Color.White }, false, true, false)]	// cannot pay with tokens and tableau, in hand
		public void BuildCardAction_CanExecute(int[] costs, int[] tokens, Color[] tableau, bool inMarket, bool inHand, bool expectation)
		{
			Card card = new Card()
			{
				costWhite = (byte)costs[0],
				costBlue = (byte)costs[1],
				costGreen = (byte)costs[2],
				costRed = (byte)costs[3],
				costBlack = (byte)costs[4],
			};
			IGame game = Substitute.For<IGame>();
			for (int i = 0; i < tokens.Length; i++)
			{
				game.CurrentPlayer.Tokens((Color)i).Returns(tokens[i]);
			}
			if (inMarket)
			{
				game.Market.Returns(new Card[] { card });
			}
			else if (inHand)
			{
				game.CurrentPlayer.Hand.Returns(new Card[] { card });
			}
			game.CurrentPlayer.Tableau.Returns(tableau.Select(c => new Card() { gives = (byte)c }));
			IAction buildCardAction = new BuildCardAction(card);
			bool result = buildCardAction.CanExecute(game);
			result.Should().Be(expectation);
		}

		[Theory]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 7, 7, 7 }, true)]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 1, 1, 1 }, true)]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 1, 2, 3 }, true)]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 0, 0, 0 }, false)]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 0, 1, 1 }, false)]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 1, 0, 1 }, false)]
		[InlineData(new Color[] { Color.Green, Color.Black, Color.Blue }, new int[] { 1, 1, 0 }, false)]
		[InlineData(new Color[] { Color.Green, Color.Green }, new int[] { 4, 4 }, true)]
		[InlineData(new Color[] { Color.Green, Color.Green }, new int[] { 3, 3 }, false)]
		[InlineData(new Color[] { Color.Green, Color.Green }, new int[] { 3, 3 }, false)]
		[InlineData(new Color[] { Color.Green, Color.Green }, new int[] { 0, 0 }, false)]
		public void TakeTokensAction_CanExecute(Color[] colors, int[] supplyCount, bool expectation)
		{
			var game = Substitute.For<IGame>();
			for (int i = 0; i < colors.Length; i++)
			{
				game.Supply(colors[i]).Returns(supplyCount[i]);
			}
			IAction takeTokensAction = new TakeTokensAction(colors);
			bool result = takeTokensAction.CanExecute(game);
			result.Should().Be(expectation);
		}

		[Fact]
		public void TakeTokensAction_Execute_AddsTokenToPlayerSupply()
		{
			IGame game = new Game(Rules.Setups[0]);
			IAction takeTokensAction = new TakeTokensAction(Color.Blue);
			takeTokensAction.Execute(game);
			game.CurrentPlayer.Tokens(Color.Blue).Should().Be(1);
		}

		[Theory]
		[InlineData(false, Color.Green, 0, 0, 0, 0, 0, 0)]
		[InlineData(false, Color.Green, 0, 5, 5, 0, 0, 0)]
		[InlineData(false, Color.Green, 6, 5, 0, 0, 0, 0)]
		[InlineData(false, Color.Green, 0, 5, 4, 1, 0, 0)]
		[InlineData(true, Color.Green, 0, 5, 5, 1, 0, 0)]
		public void ReplaceTokenAction_CanExecute(bool expectation, Color color, int whiteTokens, int blueTokens, int greenTokens, int redTokens, int blackTokens, int goldTokens)
		{
			IGame game = Substitute.For<IGame>();
			IPlayer player = Substitute.For<IPlayer>();
			player.Tokens(Color.White).Returns(whiteTokens);
			player.Tokens(Color.Blue).Returns(blueTokens);
			player.Tokens(Color.Green).Returns(greenTokens);
			player.Tokens(Color.Red).Returns(redTokens);
			player.Tokens(Color.Black).Returns(blackTokens);
			player.Tokens(Color.Gold).Returns(goldTokens);
			game.GetPlayer(0).ReturnsForAnyArgs(player);
			ReplaceTokenAction rpa = new ReplaceTokenAction(color);
			bool result = rpa.CanExecute(game);
			result.Should().Be(expectation);
		}
	}
}
