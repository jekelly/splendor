namespace Splendor.Model
{
	using System;
	using System.Linq;

	public class Deck
	{
		private readonly int[] indices;
		private readonly Card[] cards;
		private readonly int startIndex;
		private readonly int size;
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

		public Deck(Deck other)
		{
			this.indices = new int[other.indices.Length];
			Buffer.BlockCopy(other.indices, 0, this.indices, 0, sizeof(int) * other.indices.Length);
			// NOTE: shallow ref copy here, assumption is cards is static and cannot change!
			this.cards = other.cards;
			this.startIndex = other.startIndex;
			this.size = other.size;
			this.head = other.head;
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
