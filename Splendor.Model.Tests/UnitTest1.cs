using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Splendor.Model.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			GameState gs = new GameState(2);
			gs.ShuffleDecks();
		}
	}

	[TestClass]
	public class ActionTests
	{
		[TestMethod]
		public void CanExecuteNull_ReturnsFalse()
		{
			GameState state = new GameState(2);
			GameState.TakeTokenAction takeTokenAction = new GameState.TakeTokenAction(Color.Green);
			takeTokenAction.CanExecute(null);
		}

		[TestMethod]
		public void CanExecuteSingle_ReturnsTrue()
		{
			GameState state = new GameState(2);
			IGame game = state.AsGame();
			game.Tokens[(int)Color.Green] = 1;
			GameState.TakeTokenAction takeTokenAction = new GameState.TakeTokenAction(Color.Green);
			bool result = takeTokenAction.CanExecute(state);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void CanExecuteEmpty_ReturnsFalse()
		{
			GameState state = new GameState(2);
			IGame game = state.AsGame();
			game.Tokens[(int)Color.Green] = 0;
			GameState.TakeTokenAction takeTokenAction = new GameState.TakeTokenAction(Color.Green);
			bool result = takeTokenAction.CanExecute(state);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void CanExecute_TakeTwo_ReturnsTrue()
		{
			GameState state = new GameState(2);
			IGame game = state.AsGame();
			game.Tokens[(int)Color.Green] = 4;
			GameState.TakeTokenAction takeTokenAction = new GameState.TakeTokenAction(Color.Green);
			takeTokenAction.Execute(state);
			bool result = takeTokenAction.CanExecute(state);
			Assert.IsTrue(result);
		}
	}
}
