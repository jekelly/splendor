namespace Splendor.ViewModel
{
	using Splendor.Model;
	using Windows.UI.Xaml.Data;
	using Windows.UI.Xaml.Media;

	public sealed class ColorToBrushConverter : IValueConverter
	{
		private static readonly Brush WhiteBrush = new SolidColorBrush(Windows.UI.Colors.White);
		private static readonly Brush BlueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
		private static readonly Brush GreenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
		private static readonly Brush RedBrush = new SolidColorBrush(Windows.UI.Colors.Red);
		private static readonly Brush BlackBrush = new SolidColorBrush(Windows.UI.Colors.Black);
		private static readonly Brush GoldBrush = new SolidColorBrush(Windows.UI.Colors.Gold);
		private static readonly Brush DefaultBrush = new SolidColorBrush(Windows.UI.Colors.HotPink);

		public object Convert(object value, System.Type targetType, object parameter, string language)
		{
			Color color = (Color)value;
			switch (color)
			{
				case Color.White:
					return WhiteBrush;
				case Color.Blue:
					return BlueBrush;
				case Color.Green:
					return GreenBrush;
				case Color.Red:
					return RedBrush;
				case Color.Black:
					return BlackBrush;
				case Color.Gold:
					return GoldBrush;
				default:
					return DefaultBrush;
			}
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, string language)
		{
			throw new System.NotImplementedException();
		}
	}
}
