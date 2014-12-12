namespace Splendor.ViewModel
{
	using System.Collections.Generic;
	using Splendor.Model;
	using Windows.UI.Xaml.Data;

	public sealed class CardToCostsConverter : IValueConverter
	{
		class ColorCost
		{
			public Color Color { get; set; }
			public int Cost { get; set; }
		}

		public object Convert(object value, System.Type targetType, object parameter, string language)
		{
			Card card = (Card)value;
			var costs = new List<ColorCost>(5);
			costs.Add(new ColorCost() { Color = Color.White, Cost = card.costWhite });
			costs.Add(new ColorCost() { Color = Color.Blue, Cost = card.costBlue });
			costs.Add(new ColorCost() { Color = Color.Green, Cost = card.costGreen });
			costs.Add(new ColorCost() { Color = Color.Red, Cost = card.costRed });
			costs.Add(new ColorCost() { Color = Color.Black, Cost = card.costBlack });
			return costs;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, string language)
		{
			throw new System.NotImplementedException();
		}
	}
}
