namespace Splendor.Model
{
	using System;
	using System.Linq;

	class ReferenceArray<T>
	{
		private readonly int length;
		private readonly int[][] indices;
		private readonly T[] array;

		public ReferenceArray(T[] array, params int[][] indices)
		{
			this.array = array;
			if (!indices.Any())
			{
				indices = new int[1][];
				indices[0] = Enumerable.Range(0, array.Length).ToArray();
			}
			this.indices = indices;
			this.length = this.indices.Sum(i => i.Length);
		}

		public int Length
		{
			get { return this.length; }
		}

		public T this[int index]
		{
			get
			{
				for (int i = 0; i < this.indices.Length; i++)
				{
					if (index < this.indices[i].Length)
					{
						return this.array[this.indices[i][index]];
					}
					index -= this.indices[i].Length;
				}
				throw new IndexOutOfRangeException();
			}
		}
	}
}
