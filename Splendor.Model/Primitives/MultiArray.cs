namespace Splendor.Model
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	class MultiArray<T> : IEnumerable<T>
	{
		private int length;
		private readonly ReferenceArray<T>[] arrays;

		public MultiArray(params ReferenceArray<T>[] arrays)
		{
			this.length = arrays.Sum(a => a.Length);
			this.arrays = arrays;
		}

		public int Length { get { return this.length; } }

		public T this[int index]
		{
			get
			{
				for (int i = 0; i < this.arrays.Length; i++)
				{
					if (index < this.arrays[i].Length)
					{
						return this.arrays[i][index];
					}
					index -= this.arrays[i].Length;
				}
				throw new IndexOutOfRangeException();
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new MultiArrayEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		class MultiArrayEnumerator : IEnumerator<T>
		{
			private int index;
			private readonly MultiArray<T> array;

			public MultiArrayEnumerator(MultiArray<T> array)
			{
				this.index = -1;
				this.array = array;
			}

			public T Current
			{
				get { return this.array[this.index]; }
			}

			object System.Collections.IEnumerator.Current
			{
				get { return this.Current; }
			}

			public bool MoveNext()
			{
				return ++this.index < this.array.length;
			}

			public void Reset()
			{
				this.index = -1;
			}

			public void Dispose()
			{
			}
		}
	}
}
