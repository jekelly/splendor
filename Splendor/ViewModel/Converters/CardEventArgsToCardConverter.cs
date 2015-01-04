namespace Splendor.ViewModel
{
	using Windows.UI.Xaml.Data;

	public sealed class CardEventArgsToCardConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, string language)
		{
			return ((CardEventArgs)value).Card;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, string language)
		{
			throw new System.NotImplementedException();
		}
	}
}
