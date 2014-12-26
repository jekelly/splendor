using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splendor.Model;
using Windows.UI.Xaml.Data;

namespace Splendor.ViewModel
{
	public class NobleToImageSourceConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			Noble noble = (Noble)value;
			return string.Format("ms-appx:///Assets/Themes/Space/Nobles/noble{0}.jpg", noble.id);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
