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
			GameState state = new GameState(2);
			TakeTokenAction takeTokenAction = new TakeTokenAction(Color.Green);
			takeTokenAction.CanExecute(null);
		}

		[Theory]
		[InlineData(Color.Green, 7, true, new Color[0])]
		[InlineData(Color.Green, 0, false, new Color[0])]
		[InlineData(Color.Green, 1, true, new Color[] { Color.White })]
		[InlineData(Color.Green, 4, true, new Color[] { Color.Green })]
		[InlineData(Color.Green, 2, false, new Color[] { Color.Green })]
		[InlineData(Color.Green, 1, true, new Color[] { Color.White, Color.Blue })]
		[InlineData(Color.Green, 1, false, new Color[] { Color.Blue, Color.Blue })]
		[InlineData(Color.Green, 1, false, new Color[] { Color.White, Color.Green })]
		public void TakeTokenAction_CanExecute(Color color, int startCount, bool expectation, params Color[] previouslyBought)
		{
			IGame game = Substitute.For<IGame>();
			game.Supply(color).Returns(startCount);
			game.Actions.Returns(previouslyBought.Select(pb => (IAction)new TakeTokenAction(pb)).ToList());
			IAction takeTokenAction = new TakeTokenAction(color);
			bool result = takeTokenAction.CanExecute(game);
			result.Should().Be(expectation);
		}

		[Theory]
		[InlineData(false, Color.Green, 0, 0, 0, 0, 0, 0)]
		[InlineData(false, Color.Green, 0, 5, 5, 0, 0, 0)]
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
