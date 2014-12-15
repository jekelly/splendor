namespace Splendor.Model
{
	using System.Collections.Generic;

	public interface IPlayer
	{
		int Index { get; }
		int Score { get; }
		int Gems(Color color);
		int GemCount { get; }
		int Tokens(Color color);
		int TokenCount { get; }
		IEnumerable<Card> Hand { get; }
		IEnumerable<Card> Tableau { get; }
		IEnumerable<Noble> Nobles { get; }
		void GainToken(Color color);
		void SpendToken(Color color);
		void MoveCardToTableau(Card card);
		void MoveCardToHand(Card card);
		void GainNoble(Noble noble);

		void ReturnToken(Color color);
	}
}
