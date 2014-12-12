namespace Splendor.ViewModel
{
	using System;
	using Splendor.Model;

	public sealed class CardEventArgs : EventArgs
	{
		public IPlayer Player { get; private set; }
		public Card Card { get; private set; }

		public CardEventArgs(IPlayer player, Card card)
		{
			this.Player = player;
			this.Card = card;
		}
	}
}
