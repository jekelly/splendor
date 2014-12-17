namespace Splendor.Model
{
	using System;
	using System.Linq;

	class ReplaceTokenAction : IAction
	{
		private Color color;

		public ReplaceTokenAction(Color color)
		{
			this.color = color;
		}

		public bool CanExecute(IGame game)
		{
			Phase currentPhase = game.CurrentPhase;
			if(currentPhase == Phase.Pay)
			{
				return game.Debt(this.color) > 0 && game.CurrentPlayer.Tokens(this.color) > 0;
			}
			else if (currentPhase == Phase.EndTurn)
			{
				return game.CurrentPlayer.TokenCount > 10 && game.CurrentPlayer.Tokens(this.color) > 0;
			}
			throw new InvalidOperationException();
		}

		public void Execute(IGame game)
		{
			IPlayer currentPlayer = game.CurrentPlayer;
			if (game.CurrentPhase == Phase.Pay)
			{
				currentPlayer.SpendToken(this.color);
			}
			else
			{
				currentPlayer.ReturnToken(this.color);
			}
			game.EventSink.OnTokenReturned(currentPlayer, this.color);
		}

		public override string ToString()
		{
			return string.Format("Replace {0}", this.color);	
		}
	}
}
