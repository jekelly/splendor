﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Model
{
	internal class TakeTokenAction : IAction
	{
		private readonly Color color;
		public TakeTokenAction(Color color)
		{
			this.color = color;
		}

		public bool CanExecute(IGame game)
		{
			if (game == null)
			{
				return false;
			}
			bool isFirstAction = game.Actions.Count == 0;
			bool isSecondAction = game.Actions.Count == 1;
			int count = game.Supply[(int)this.color];
			TakeTokenAction firstAction = isSecondAction ? game.Actions[0] as TakeTokenAction : null;
			// if this is the first action, the only requirement is
			// that there are tokens left to take.
			return (isFirstAction && count > 0) ||
				// if this is the second action, it either needs to be a
				// different color or we need to have at least 3 left (4 normally)
				(isSecondAction && firstAction != null && (firstAction.color != this.color || count >= 3));
		}

		public void Execute(IGame game)
		{
			if (game == null)
			{
				throw new ArgumentNullException("game");
			}
			int playerIndex = game.CurrentPlayer;
			game.GainToken(playerIndex, this.color);
		}
	}
}