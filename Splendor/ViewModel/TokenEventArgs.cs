namespace Splendor.ViewModel
{
	using System;
	using Splendor.Model;

	public sealed class TokenEventArgs : EventArgs
	{
		public Color Color { get; private set; }

		public TokenEventArgs(Color color)
		{
			this.Color = color;
		}
	}
}
