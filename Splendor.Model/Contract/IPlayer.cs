namespace Splendor.Model
{
	using System.Collections.Generic;

	public interface IPlayer
	{
		int Score { get; }
		int Gems(Color color);
		int Tokens(Color color);
		IEnumerable<Card> Hand { get; }
		IEnumerable<Card> Tableau { get; }
		IEnumerable<Noble> Nobles { get; }
		void GainToken(Color color);
		void SpendToken(Color color);
		void MoveCardToTableau(Card card);
		void MoveCardToHand(Card card);
		void GainNoble(Noble noble);
	}
}
