namespace Splendor.ViewModel
{
	using System;
	using System.Linq;
	using System.Windows.Input;
	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Command;
	using GalaSoft.MvvmLight.Ioc;
	using Splendor.Model;

	public class TokenCounterViewModel : ViewModelBase
	{
		private readonly int playerIndex;
		private readonly Func<int> getCount;
		private readonly CommandService commandService;
		private static int selectionIndex = -1;
		private readonly static Color[] selectedTokens;
		private readonly RelayCommand selectCommand;

		public ICommand SelectCommand { get { return this.selectCommand; } }

		public int PlayerIndex { get { return this.playerIndex; } }

		public int Count { get; private set; }

		public Color Color { get; private set; }

		public void Refresh()
		{
			// Filter out Gold as it is used as the sentinel for the selection array.
			int selected = (this.Color == Model.Color.Gold) ? 0 : selectedTokens.Count(st => st == this.Color);
			this.Count = this.getCount() - selected;
			this.selectCommand.RaiseCanExecuteChanged();
			this.RaisePropertyChanged("Count");
		}

		static TokenCounterViewModel()
		{
			selectedTokens = Enumerable.Repeat(Color.Gold, 3).ToArray();
			selectionIndex = -1;
		}

		public TokenCounterViewModel(int playerIndex, Color color, Func<int> getCount)
		{
			this.playerIndex = playerIndex;
			this.Color = color;
			this.getCount = getCount;
			//eventService.TokenTaken += this.OnTokenChange;
			//eventService.TokenReturned += this.OnTokenChange;
			this.commandService = SimpleIoc.Default.GetInstance<CommandService>();
			this.selectCommand = new RelayCommand(this.Select, this.CanSelect);

			this.Refresh();
		}

		private bool CanSelect()
		{
			if (this.Color == Color.Gold)
			{
				return false;
			}
			// TODO: need a better way to deal with this for cases where token supply is empty
			//if(this.Count == 0)
			//{
			//	return false;
			//}
			if (selectionIndex >= 2)
			{
				return false;
			}
			if (selectionIndex == -1)
			{
				return true;
			}
			if (selectionIndex == 0 && selectedTokens[0] == this.Color)
			{
				return this.Count > 1;
			}
			return selectedTokens[0] != this.Color && selectedTokens[1] != this.Color && selectedTokens[0] != selectedTokens[1];
		}

		private void Select()
		{
			selectedTokens[++selectionIndex] = this.Color;
			if (selectionIndex == 2 || selectionIndex == 1 && selectedTokens[0] == selectedTokens[1])
			{
				Array.Sort(selectedTokens);
				this.commandService.TakeTokensCommand.Execute(selectedTokens);
				selectedTokens[0] = Model.Color.Gold;
				selectedTokens[1] = Model.Color.Gold;
				selectedTokens[2] = Model.Color.Gold;
				selectionIndex = -1;
			}
			this.Refresh();
		}

		private void OnTokenChange(object sender, TokenEventArgs e)
		{
			if (e.Color == this.Color)
			{
				this.Refresh();
			}
		}
	}
}
