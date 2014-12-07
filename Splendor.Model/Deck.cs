using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	public class Deck
	{
		private int[] indices;
		private Card[] cards;
		private int startIndex;
		private int size;
		private int head;

		public Deck(Card[] cards, int start, int size)
		{
			if (cards == null)
			{
				throw new ArgumentNullException("cards");
			}
			this.cards = cards;
			if (start < 0 ||
				start > this.cards.Length ||
				size < 0 ||
				start + size > this.cards.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.startIndex = start;
			this.size = size;
			this.indices = Enumerable.Range(this.startIndex, size).ToArray();
			this.head = 0;
		}

		public Card Draw()
		{
			if (this.head >= this.size)
			{
				return Rules.SentinelCard;
				//throw new InvalidOperationException();
			}
			return this[this.head++];
		}

		public Card this[int index]
		{
			get
			{
				if (index >= this.size || index < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.cards[this.indices[index]];
			}
		}

		public void Shuffle(IRandomizer randomizer)
		{
			if (this.head != 0)
			{
				throw new InvalidOperationException("Cannot shuffle a deck once it has been drawn from.");
			}
			for (int i = this.size - 1; i >= 0; i--)
			{
				int ind = randomizer.Next(i);
				int swap = this.indices[i];
				this.indices[i] = this.indices[ind];
				this.indices[ind] = swap;
			}
		}
	}
}
