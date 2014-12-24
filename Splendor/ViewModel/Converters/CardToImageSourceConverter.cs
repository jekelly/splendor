namespace Splendor.ViewModel
{
	using System;
	using Splendor.Model;
	using Windows.UI.Xaml.Data;
	using Windows.UI.Xaml.Media;

	public class CardToImageSourceConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			Card card = (Card)value;
			string file = string.Format("ms-appx:///Assets/Themes/Space/Tier{0}/{1}.jpg", card.tier + 1, card.Gives);
			return file;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
