namespace Splendor.Model.AI
{
	using System.Collections.Generic;
	using System.Linq;

	public class ColorPreferenceChooser : IChooser
	{
		private const double TokenValue = 1.0;
		private const double GemValue = 10.0;

		private readonly int playerIndex;

		private Color favoriteColor;

		public ColorPreferenceChooser(int playerIndex)
		{
			this.playerIndex = playerIndex;
			this.favoriteColor = Color.Gold;
		}

		public IAction Choose(IGame state)
		{
			if (this.favoriteColor == Color.Gold)
			{
				this.favoriteColor = DetermineBestColor(state);
			}

			var actions = state.AvailableActions.ToArray();
			if (actions.Length == 0)
			{
				return null;
			}
			if (actions.Length == 1)
			{
				return actions[0];
			}

			double best = double.MinValue;
			int bestIndex = int.MinValue;
			int preCountTokens = state.GetPlayer(this.playerIndex).Tokens(this.favoriteColor);
			int preCountGems = state.GetPlayer(this.playerIndex).Gems(this.favoriteColor);
			for (int i = 0; i < actions.Length; i++)
			{
				var action = actions[i];
				var clone = state.Clone();
				clone.Step(action);
				int postCountTokens = clone.GetPlayer(this.playerIndex).Tokens(this.favoriteColor);
				int postCountGems = clone.GetPlayer(this.playerIndex).Gems(this.favoriteColor);
				double v = (postCountTokens - preCountTokens) * TokenValue + (postCountGems - preCountGems) * GemValue;
				if (v > best)
				{
					best = v;
					bestIndex = i;
				}
			}
			return actions[bestIndex];
		}

		private Color DetermineBestColor(IGame state)
		{
			double[] colorValues = new double[5];
			foreach (Card card in state.Market)
			{
				if (card.value > 0)
				{
					colorValues[(int)Color.White] += (double)card.value / (card.costWhite);
					colorValues[(int)Color.Blue] += (double)card.value / (card.costBlue);
					colorValues[(int)Color.Green] += (double)card.value / (card.costGreen);
					colorValues[(int)Color.Red] += (double)card.value / (card.costRed);
					colorValues[(int)Color.Black] += (double)card.value / (card.costBlack);
				}
			}
			foreach (Noble noble in state.Nobles)
			{
				for (int i = 0; i < 5; i++)
				{
					colorValues[i] += 3.0d / noble.requires[i];
				}
			}
			return colorValues.Select((v, i) => new { Value = v, Color = (Color)i }).OrderByDescending(a => a.Value).Select(v => v.Color).First();
		}

		public void PostGame(int winner, IEventSink eventSink, List<IGame>[] history)
		{
			this.favoriteColor = Color.Gold;
		}
	}
}
