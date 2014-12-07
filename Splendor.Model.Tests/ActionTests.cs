using System;
using System.Collections.Generic;
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

		[Fact]
		public void ExecuteNull_Throws()
		{
			TakeTokensAction takeTokensAction = new TakeTokensAction();
			Action a = () => { takeTokensAction.Execute(null); };
			a.ShouldThrow<ArgumentNullException>();
		}

		[Theory]
		[InlineData(0, true)]
		[InlineData(1, true)]
		[InlineData(2, true)]
		[InlineData(3, false)]
		[InlineData(4, false)]
		public void ReserveCardAction_CanExecute_ByCardsInHand(int cardsInHand, bool canExecute)
		{
			IGame game = new TestGame();
			game.CurrentPlayer.Hand.Returns(Enumerable.Range(1, cardsInHand).Select(i => new Card() { id = (byte)i }));
			ReserveCardAction reserveCardAction = new ReserveCardAction(game.Market[0]);
			reserveCardAction.CanExecute(game).Should().Be(canExecute);
		}

		[Theory]
		[InlineData(Phase.Choose, true)]
		[InlineData(Phase.EndTurn, false)]
		[InlineData(Phase.GameOver, false)]
		[InlineData(Phase.NotStarted, false)]
		[InlineData(Phase.Pay, false)]
		public void ReserveCardAction_CanExecute_ByPhase(Phase phase, bool canExecute)
		{
			IGame game = Substitute.ForPartsOf<TestGame>();
			game.CurrentPhase.Returns(phase);
			ReserveCardAction reserveCardAction = new ReserveCardAction(game.Market[0]);
			reserveCardAction.CanExecute(game).Should().Be(canExecute);
		}

		[Theory]
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, true, false, true)]	// can pay from tokens, in market
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, false, true, true)]	// can pay from tokens, in hand
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, false, false, false)]	// can pay from tokens, not available
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, false, false, false)]	// can pay from tokens, not available
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 0 }, true, false, true)]	// can pay from tableau, in market
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 1 }, new int[] { 0, 0, 0, 0, 0, 0 }, true, false, true)]	// can pay with gold, in market
		[InlineData(new int[] { 1, 0, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new int[] { 0, 0, 0, 0, 0, 0 }, true, false, true)]	// can pay with tokens or gold, in market
		[InlineData(new int[] { 3, 1, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new int[] { 2, 0, 0, 0, 0, 0 }, false, true, true)]	// can pay with tokens and tableau, in hand
		[InlineData(new int[] { 4, 1, 0, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new int[] { 2, 0, 0, 0, 0, 0 }, false, true, false)]	// cannot pay with tokens and tableau, in hand
		[InlineData(new int[] { 3, 1, 1, 0, 0 }, new int[] { 1, 0, 0, 0, 0, 1 }, new int[] { 2, 0, 0, 0, 0, 0 }, false, true, false)]	// cannot pay with tokens and tableau, in hand
		public void BuildCardAction_CanExecute(int[] costs, int[] tokens, int[] tableau, bool inMarket, bool inHand, bool expectation)
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
			game.CurrentPhase.Returns(Phase.Choose);
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
			for (int i = 0; i < tableau.Length; i++)
			{
				game.CurrentPlayer.Gems((Color)i).Returns(tableau[i]);
			}
			IAction buildCardAction = new BuildCardAction(card);
			bool result = buildCardAction.CanExecute(game);
			result.Should().Be(expectation);
		}

		[Fact]
		public void BuildCardAction_FromMarket_Execute()
		{
			IGame game = Substitute.ForPartsOf<TestGame>();
			Card card = game.Market[0];
			BuildCardAction buildCardAction = new BuildCardAction(card);
			buildCardAction.Execute(game);
			game.CurrentPlayer.Tableau.Should().OnlyContain(c => c.id == card.id);
			game.Market.Should().NotContain((c) => c.id == card.id);
		}

		[Fact]
		public void BuildCardAction_FromHand_Execute()
		{
			IGame game = Substitute.ForPartsOf<TestGame>();
			Card card = game.Market[0];
			game.CurrentPlayer.MoveCardToHand(card);
			BuildCardAction buildCardAction = new BuildCardAction(card);
			buildCardAction.Execute(game);
			game.CurrentPlayer.Tableau.Should().OnlyContain(c => c.id == card.id);
			game.Market.Should().NotContain((c) => c.id == card.id);
			game.CurrentPlayer.Hand.Should().NotContain((c) => c.id == card.id);
		}

		[Theory]
		[InlineData(new Color[] { Color.Gold })]
		[InlineData(new Color[] { Color.Red, Color.Blue, Color.Green, Color.White })]
		[InlineData(new Color[] { Color.Red, Color.Red, Color.Green })]
		public void TakeTokensAction_ThrowsIfInvalidParameters(Color[] colors)
		{
			var game = Substitute.For<IGame>();
			Action a = () => { TakeTokensAction takeTokensAction = new TakeTokensAction(colors); };
			a.ShouldThrow<NotSupportedException>();
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
			game.CurrentPhase.Returns(Phase.Choose);
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
			IGame game = new Game(Setups.All[0]);
			IAction takeTokensAction = new TakeTokensAction(Color.Blue);
			takeTokensAction.Execute(game);
			game.CurrentPlayer.Tokens(Color.Blue).Should().Be(1);
		}

		[Theory]
		[InlineData(false, Phase.EndTurn, new Color[] { Color.Green }, 0, 0, 0, 0, 0, 0)]
		[InlineData(false, Phase.EndTurn, new Color[] { Color.Green }, 0, 5, 5, 0, 0, 0)]
		[InlineData(false, Phase.EndTurn, new Color[] { Color.Green }, 6, 5, 0, 0, 0, 0)]
		[InlineData(false, Phase.EndTurn, new Color[] { Color.Green }, 0, 5, 4, 1, 0, 0)]
		[InlineData(true, Phase.EndTurn, new Color[] { Color.Green }, 0, 5, 5, 1, 0, 0)]
		[InlineData(false, Phase.Pay, new Color[] { Color.Green, Color.Green }, 0, 0, 1, 0, 0, 0)]
		public void ReplaceTokensAction_CanExecute(bool expectation, Phase phase, Color[] colors, int whiteTokens, int blueTokens, int greenTokens, int redTokens, int blackTokens, int goldTokens)
		{
			IGame game = Substitute.For<IGame>();
			game.CurrentPhase.Returns(phase);
			IPlayer player = Substitute.For<IPlayer>();
			player.Tokens(Color.White).Returns(whiteTokens);
			player.Tokens(Color.Blue).Returns(blueTokens);
			player.Tokens(Color.Green).Returns(greenTokens);
			player.Tokens(Color.Red).Returns(redTokens);
			player.Tokens(Color.Black).Returns(blackTokens);
			player.Tokens(Color.Gold).Returns(goldTokens);
			game.CurrentPlayer.Returns(player);
			ReplaceTokensAction replaceTokensAction = new ReplaceTokensAction(colors);
			bool result = replaceTokensAction.CanExecute(game);
			result.Should().Be(expectation);
		}

		[Theory]
		[InlineData(new int[] { 0, 0, 0, 0, 0, 0 }, new Color[] { }, new int[] { 4, 4, 4, 4, 4, 5 })]
		[InlineData(new int[] { 1, 0, 0, 0, 0, 0 }, new Color[] { }, new int[] { 4, 4, 4, 4, 4, 5 })]
		[InlineData(new int[] { 1, 0, 0, 0, 0, 0 }, new Color[] { Color.White }, new int[] { 5, 4, 4, 4, 4, 5 })]
		[InlineData(new int[] { 1, 0, 0, 0, 0, 0 }, new Color[] { Color.White, Color.Green, Color.White }, new int[] { 6, 4, 5, 4, 4, 5 })]
		public void ReplaceTokensAction_Execute(int[] initial, Color[] colors, int[] expected)
		{
			IGame game = new TestGame();
			IPlayer player = game.CurrentPlayer;
			for (Color color = Color.White; color <= Color.Gold; color++)
			{
				player.Tokens(color).Returns(initial[(int)color]);
			}
			ReplaceTokensAction action = new ReplaceTokensAction(colors);
			action.Execute(game);
			for (Color color = Color.White; color <= Color.Gold; color++)
			{
				game.Supply(color).Should().Be(expected[(int)color]);
			}
		}

		internal class TestPlayer : IPlayer
		{
			private readonly IPlayer player;

			public TestPlayer(IPlayer player)
			{
				this.player = player;
			}

			public virtual int Score
			{
				get
				{
					return this.player.Score;
				}
			}

			public virtual int Gems(Color color)
			{
				return this.player.Gems(color);
			}

			public virtual int Tokens(Color color)
			{
				return this.player.Tokens(color);
			}

			public virtual IEnumerable<Card> Hand
			{
				get { return this.player.Hand; }
			}

			public virtual IEnumerable<Card> Tableau
			{
				get { return this.player.Tableau; }
			}

			public virtual void GainToken(Color color)
			{
				this.player.GainToken(color);
			}

			public virtual void SpendToken(Color color)
			{
				this.player.SpendToken(color);
			}

			public virtual void MoveCardToTableau(Card card)
			{
				this.player.MoveCardToTableau(card);
			}

			public virtual void MoveCardToHand(Card card)
			{
				this.player.MoveCardToHand(card);
			}
		}

		internal class TestGame : IGame
		{
			private readonly IGame game;
			private IPlayer currentPlayer;

			public TestGame()
			{
				this.game = new Game(Setups.All[0]);
			}

			public virtual int Supply(Color color)
			{
				return this.game.Supply(color);
			}

			public virtual Card[] Market
			{
				get { return this.game.Market; }
			}

			public virtual Noble[] Nobles
			{
				get { return this.game.Nobles; }
			}

			public virtual IEnumerable<IAction> AvailableActions
			{
				get { return this.game.AvailableActions; }
			}

			public virtual int CurrentPlayerIndex
			{
				get { return this.game.CurrentPlayerIndex; }
			}

			public virtual Phase CurrentPhase
			{
				get { return this.game.CurrentPhase; }
			}

			public virtual IPlayer CurrentPlayer
			{
				get
				{
					if (this.currentPlayer == null)
					{
						this.currentPlayer = this.GetPlayer(this.CurrentPlayerIndex);
					}
					return this.currentPlayer;
				}
			}

			public virtual IPlayer GetPlayer(int playerIndex)
			{
				return Substitute.ForPartsOf<TestPlayer>(this.game.GetPlayer(playerIndex));
			}

			public virtual void Step(IChooser chooser)
			{
				this.game.Step(chooser);
			}

			public virtual void NextPhase()
			{
				this.game.NextPhase();
			}
		}
	}
}
