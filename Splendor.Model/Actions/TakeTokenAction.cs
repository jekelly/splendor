namespace Splendor.Model
{
	using System;
	using System.Linq;

	internal class TakeTokensAction : IAction
	{
		private readonly Color[] colors;
		public TakeTokensAction(params Color[] colors)
		{
			this.colors = colors;
			if(this.colors.Any(color => color == Color.Gold))
			{
				throw new NotSupportedException("Cannot take a Gold token with the TakeTokensAction.");
			}
			if(this.colors.Length > 3)
			{
				throw new NotSupportedException("Can only take 3 tokens at a time at most.");
			}
			if (this.colors.Length == 3 && (this.colors[0] == this.colors[1] || this.colors[0] == this.colors[2] || this.colors[1] == this.colors[2]))
			{
				throw new NotSupportedException("Cannot take three tokens unless they are all a different color.");
			}
		}

		public bool CanExecute(IGame game)
		{
			if (game == null)
			{
				return false;
			}
			if (game.CurrentPhase != Phase.Choose)
			{
				return false;
			}
			bool sameColor = this.colors.Length > 1 && this.colors[0] == this.colors[1];
			if (sameColor)
			{
				return game.Supply(this.colors[0]) >= 4;
			}
			return this.colors.All(color => game.Supply(color) > 0);
		}

		public void Execute(IGame game)
		{
			if (game == null)
			{
				throw new ArgumentNullException("game");
			}
			IPlayer currentPlayer = game.CurrentPlayer;
			foreach (Color color in this.colors)
			{
				currentPlayer.GainToken(color);
			}
		}
	}
}
