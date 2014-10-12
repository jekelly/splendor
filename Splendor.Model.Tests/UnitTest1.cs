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
}
