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
	}
}
