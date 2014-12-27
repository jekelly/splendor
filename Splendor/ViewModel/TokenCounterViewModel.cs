namespace Splendor.ViewModel
{
	using System;
	using System.Linq;
	using System.Windows.Input;
	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Command;
	using GalaSoft.MvvmLight.Ioc;
	using Splendor.Model;

	class TokenSelectionViewModel : ViewModelBase, IRefreshable
	{
		private int selectionIndex = -1;
		private readonly Func<bool> canTakeTokens;
		private readonly CommandService commandService;
		private readonly Color[] selectedTokens;
		private readonly RelayCommand<TokenCounterViewModel> selectCommand;

		public int SelectionCount(Color color)
		{
			if(color == Color.Gold)
			{
				return 0;
			}
			return this.selectedTokens.Count(c => c == color);
		}

		public ICommand SelectCommand { get { return this.selectCommand; } }

		public TokenSelectionViewModel(Func<bool> canTakeTokens, CommandService commandService)
		{
			this.commandService = commandService;
			this.canTakeTokens = canTakeTokens;
			this.selectedTokens = Enumerable.Repeat(Color.Gold, 3).ToArray();
			this.selectionIndex = -1;
			this.selectCommand = new RelayCommand<TokenCounterViewModel>(this.Select, this.CanSelect);
			this.commandService.RegisterCommand(this);
		}

		private bool CanSelect(TokenCounterViewModel tokens)
		{
			if (!this.canTakeTokens())
			{
				return false;
			}
			if (tokens.Color == Color.Gold)
			{
				return false;
			}
			// TODO: need a better way to deal with this for cases where token supply is empty
			//if(this.Count == 0)
			//{
			//	return false;
			//}
			if (this.selectionIndex >= 2)
			{
				return false;
			}
			if (this.selectionIndex == -1)
			{
				return true;
			}
			if (this.selectionIndex == 0 && this.selectedTokens[0] == tokens.Color)
			{
				return tokens.Count > 3;
			}
			return selectedTokens[0] != tokens.Color && 
				this.selectedTokens[1] != tokens.Color &&
				this.selectedTokens[0] != this.selectedTokens[1];
		}

		private void Select(TokenCounterViewModel tokens)
		{
			this.selectedTokens[++this.selectionIndex] = tokens.Color;
			if (this.selectionIndex == 2 || this.selectionIndex == 1 && this.selectedTokens[0] == this.selectedTokens[1])
			{
				Array.Sort(this.selectedTokens);
				this.commandService.TakeTokensCommand.Execute(this.selectedTokens);
				this.selectedTokens[0] = Model.Color.Gold;
				this.selectedTokens[1] = Model.Color.Gold;
				this.selectedTokens[2] = Model.Color.Gold;
				this.selectionIndex = -1;
			}
			tokens.Refresh();
			this.Refresh();
		}

		public void Refresh()
		{
			this.selectCommand.RaiseCanExecuteChanged();
		}
	}

	public class TokenCounterViewModel : ViewModelBase
	{
		private readonly int playerIndex;
		private readonly Func<int> getCount;
		private readonly CommandService commandService;

		public int PlayerIndex { get { return this.playerIndex; } }

		public int Count { get; private set; }
		public Color Color { get; private set; }

		public void Refresh()
		{
			this.Count = this.getCount();
			this.RaisePropertyChanged("Count");
		}

		public TokenCounterViewModel(int playerIndex, Color color, Func<int> getCount)
		{
			this.playerIndex = playerIndex;
			this.Color = color;
			this.getCount = getCount;

			this.commandService = SimpleIoc.Default.GetInstance<CommandService>();

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
