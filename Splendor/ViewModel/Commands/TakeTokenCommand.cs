namespace Splendor.ViewModel
{
	using System;
	using Splendor.Model;

	class TakeTokenCommand : ActionCommand<Color[]>
	{
		public TakeTokenCommand(CommandService commandService)
			: base(commandService)
		{ }

		protected override IAction GetActionFromParameter(Color[] parameter)
		{
			if (parameter[0] == parameter[1])
			{
				return Rules.TakeActions[10 + (int)parameter[0]];
			}
			// assume parameters come in ordered
			int a = (int)parameter[0]; int b = (int)parameter[1]; int c = (int)parameter[2];
			if (a == 0 && b == 1 && c == 2) return Rules.TakeActions[0];
			else if (a == 0 && b == 1 && c == 3) return Rules.TakeActions[1];
			else if (a == 0 && b == 1 && c == 4) return Rules.TakeActions[2];
			else if (a == 0 && b == 2 && c == 3) return Rules.TakeActions[3];
			else if (a == 0 && b == 2 && c == 4) return Rules.TakeActions[4];
			else if (a == 0 && b == 3 && c == 4) return Rules.TakeActions[5];
			else if (a == 1 && b == 2 && c == 3) return Rules.TakeActions[6];
			else if (a == 1 && b == 2 && c == 4) return Rules.TakeActions[7];
			else if (a == 1 && b == 3 && c == 4) return Rules.TakeActions[8];
			else if (a == 2 && b == 3 && c == 4) return Rules.TakeActions[9];
			throw new InvalidOperationException();
		}
	}
}
