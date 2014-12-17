namespace Splendor.Model
{
	using System.Linq;

	class NobleVisitAction : IAction
	{
		private readonly Noble noble;

		public NobleVisitAction(Noble noble)
		{
			this.noble = noble;
		}

		public bool CanExecute(IGame game)
		{
			if (game.CurrentPhase != Phase.NobleVisit)
			{
				return false;
			}
			if (!game.Nobles.Contains(this.noble))
			{
				return false;
			}
			IPlayer currentPlayer = game.CurrentPlayer;
			for (Color color = Color.White; color <= Color.Black; color++)
			{
				if (currentPlayer.Gems(color) < this.noble.requires[(int)color])
				{
					return false;
				}
			}
			return true;
		}

		public void Execute(IGame game)
		{
			game.CurrentPlayer.GainNoble(this.noble);
			game.EventSink.OnNobleVisit(game.CurrentPlayer, this.noble);
		}

		public override string ToString()
		{
			return string.Format("Noble {0} visits.", this.noble);
		}
	}
}
