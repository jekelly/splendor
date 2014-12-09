namespace Splendor.Model
{
	using System.Linq;

	class ReplaceTokensAction : IAction
	{
		private readonly Color[] colors;

		public ReplaceTokensAction(params Color[] colors)
		{
			this.colors = colors;
		}

		public bool CanExecute(IGame game)
		{
			Phase currentPhase = game.CurrentPhase;
			if(currentPhase == Phase.EndTurn || currentPhase == Phase.Pay)
			{
				IPlayer currentPlayer = game.CurrentPlayer;
				if(currentPhase == Phase.EndTurn)
				{
					int tokens = 0;
					for (Color color = Color.White; color != Color.Gold; color++)
					{
						tokens += currentPlayer.Tokens(color);
					}
					if (tokens <= 10)
					{
						return false;
					}
				}
				int[] numColor = new int[6];
				numColor = this.colors.Aggregate(numColor, (nc, color) =>
				{
					nc[(int)color]++;
					return nc;
				});
				for (int i = 0; i < 6; i++)
				{
					if (numColor[i] > currentPlayer.Tokens((Color)i))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public void Execute(IGame game)
		{
			IPlayer currentPlayer = game.CurrentPlayer;
			foreach (Color color in this.colors)
			{
				currentPlayer.SpendToken(color);
			}
			game.EventSink.OnTokensReturned(currentPlayer, this.colors);
		}
	}
}
