namespace Splendor.View
{
	using System;
	using Microsoft.Xaml.Interactivity;
	using Windows.Foundation;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Media;
	using Windows.UI.Xaml.Media.Animation;

	public sealed class ScaleAction : DependencyObject, Microsoft.Xaml.Interactivity.IAction
	{
		public object Execute(object sender, object parameter)
		{
			DependencyObject associatedObject = ((IBehavior)sender).AssociatedObject;
			var x = new DoubleAnimation()
			{
				AutoReverse = true,
				Duration = TimeSpan.FromMilliseconds(200),
				To = 1.2,
			};
			Storyboard.SetTargetProperty(x, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
			Storyboard.SetTarget(x, associatedObject);

			var y = new DoubleAnimation()
			{
				AutoReverse = true,
				Duration = TimeSpan.FromMilliseconds(200),
				To = 1.2,
			};
			Storyboard.SetTargetProperty(y, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
			Storyboard.SetTarget(y, associatedObject);
			associatedObject.SetValue(UIElement.RenderTransformProperty, new CompositeTransform());
			associatedObject.SetValue(UIElement.RenderTransformOriginProperty, new Point(0.5, 0.5));
			var sb = new Storyboard();
			sb.Children.Add(x);
			sb.Children.Add(y);
			sb.Begin();
			return null;
		}
	}
}
