using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	interface IPlayer
	{
		int Tokens(Color color);
		IEnumerable<Card> Hand { get; }
		IEnumerable<Card> Tableau { get; }
		void GainToken(Color color);
		void SpendToken(Color color);
		void MoveCardToTableau(Card card);
		void MoveCardToHand(Card card);
	}
}
