namespace Splendor.ViewModel
{
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Data;

	public sealed class CostZeroToCollapsedConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, string language)
		{
			return ((int)value) == 0 ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, string language)
		{
			throw new System.NotImplementedException();
		}
	}
}
