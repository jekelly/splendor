
namespace Splendor.ViewModel
{
	using System;
	using GalaSoft.MvvmLight;
	using Splendor.Model;

	public class TokenCounterViewModel : ViewModelBase
	{
		private readonly Func<int> getCount;

		public int Count { get; private set; }

		public void Refresh()
		{
			this.Count = this.getCount();
			this.RaisePropertyChanged("Count");
		}

		public Color Color { get; private set; }

		public TokenCounterViewModel(Color color, Func<int> getCount)
		{
			this.Color = color;
			this.getCount = getCount;
			//eventService.TokenTaken += this.OnTokenChange;
			//eventService.TokenReturned += this.OnTokenChange;
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
