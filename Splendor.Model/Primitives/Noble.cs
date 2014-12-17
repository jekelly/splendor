namespace Splendor.Model
{
	using System.Linq;

	public class Noble
	{
		public byte id;
		public byte[] requires;
		public byte value;

		public override string ToString()
		{
			return string.Format("N-[{0}]", string.Join(" ", this.requires.Select((r, i) => r + Colors.Short((byte)i).ToString())));
		}
	}
}
