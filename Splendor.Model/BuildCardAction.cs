﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	class BuildCardAction : IAction
	{
		private readonly Card card;

		public BuildCardAction(Card card)
		{
			this.card = card;
		}

		public bool CanExecute(IGame game)
		{
			bool available = game.Market.Contains(this.card) || game.CurrentPlayer.Hand.Contains(this.card);
			bool buildable = this.card.CanBuy(this.BuyingPower(game));
			return available && buildable;
		}

		public void Execute(IGame game)
		{
			game.MoveCardToTableau(game.CurrentPlayerIndex, this.card);
		}

		private int[] BuyingPower(IGame game)
		{
			int[] power = new int[6];

			foreach (Card card in game.CurrentPlayer.Tableau)
			{
				power[(int)card.gives]++;
			}
			for (int i = 0; i < 5; i++)
			{
				Color c = (Color)i;
				power[i] += game.CurrentPlayer.Tokens(c);
			}
			power[(int)Color.Gold] = game.CurrentPlayer.Tokens(Color.Gold);
			return power;
		}
	}
}
