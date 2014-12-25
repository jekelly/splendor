namespace Splendor.ViewModel
{
	using System;
	using Splendor.Model;

	public sealed class TokenEventArgs : EventArgs
	{
		public int PlayerIndex { get; private set; }

		public Color Color { get; private set; }

		public TokenEventArgs(Color color, int playerIndex)
		{
			this.Color = color;
			this.PlayerIndex = playerIndex;
		}
	}
}
