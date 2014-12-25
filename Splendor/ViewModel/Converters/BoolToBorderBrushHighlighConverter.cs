namespace Splendor.ViewModel
{
	using System;
	using Windows.UI;
	using Windows.UI.Xaml.Data;
	using Windows.UI.Xaml.Media;

	class BoolToBorderBrushHighlightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if((bool)value)
			{
				return new SolidColorBrush(Colors.Gold);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
