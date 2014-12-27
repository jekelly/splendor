namespace Splendor.View
{
	using System;
	using System.Threading.Tasks;
	using GalaSoft.MvvmLight.Ioc;
	using Microsoft.Xaml.Interactivity;
	using Windows.Foundation;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;
	using Windows.UI.Xaml.Media;
	using Windows.UI.Xaml.Media.Animation;
	using Windows.UI.Xaml.Media.Imaging;

	public sealed class TranslateAndScaleAction : DependencyObject, Microsoft.Xaml.Interactivity.IAction
	{
		private readonly string guid;
		private readonly AnimationService animationService;

		public object SourceId
		{
			get { return (object)GetValue(SourceIdProperty); }
			set { SetValue(SourceIdProperty, value); }
		}

		public static readonly DependencyProperty SourceIdProperty = DependencyProperty.Register("SourceId", typeof(object), typeof(TranslateAndScaleAction), new PropertyMetadata(null));

		public TranslateAndScaleAction()
		{
			this.guid = Guid.NewGuid().ToString();
			this.animationService = SimpleIoc.Default.GetInstance<AnimationService>();
		}

		private static Panel FindVisualRootPanel(DependencyObject obj)
		{
			Panel panel = null;
			while (obj != null)
			{
				panel = obj as Panel ?? panel;
				obj = VisualTreeHelper.GetParent(obj);
			}
			return panel;
		}

		private Canvas GetAnimationLayer(DependencyObject obj)
		{
			Panel p = FindVisualRootPanel(obj);
			foreach (var child in p.Children)
			{
				if (((FrameworkElement)child).Name == this.guid)
				{
					return child as Canvas;
				}
			}
			Canvas c = new Canvas();
			c.Name = this.guid;
			p.Children.Add(c);
			return c;
		}

		public object Execute(object sender, object parameter)
		{
			return this.ExecuteAsync(sender, parameter);
		}

		private async Task ExecuteAsync(object sender, object parameter)
		{
			if (this.SourceId == null)
			{
				return;
			}
			var sourceObj = this.animationService.GetSource(this.SourceId);
			var target = ((IBehavior)sender).AssociatedObject as UIElement;
			RenderTargetBitmap rtb = new RenderTargetBitmap();
			await rtb.RenderAsync(sourceObj);
			Image i = new Image();
			Canvas canvas = this.GetAnimationLayer(sourceObj);
			i.Source = rtb;
			i.RenderTransform = new CompositeTransform();

			GeneralTransform t = sourceObj.TransformToVisual(Window.Current.Content);
			var startOffset = t.TransformPoint(new Point(0, 0));
			Canvas.SetLeft(i, startOffset.X);
			Canvas.SetTop(i, startOffset.Y);
			canvas.Children.Add(i);

			// figure out translation
			Point targetTranslation = target.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
			double scaleX = target.RenderSize.Width / sourceObj.RenderSize.Width;
			double scaleY = target.RenderSize.Height / sourceObj.RenderSize.Height;
			Storyboard sb = new Storyboard();
			TimeSpan time = TimeSpan.FromMilliseconds(500);
			DoubleAnimation xa = new DoubleAnimation()
			{
				Duration = time,
				To = targetTranslation.X,
			};
			Storyboard.SetTargetProperty(xa, "(Canvas.Left)");
			Storyboard.SetTarget(xa, i);
			DoubleAnimation ya = new DoubleAnimation()
			{
				Duration = time,
				To = targetTranslation.Y,
			};
			Storyboard.SetTargetProperty(ya, "(Canvas.Top)");
			Storyboard.SetTarget(ya, i);
			DoubleAnimation xs = new DoubleAnimation()
			{
				Duration = time,
				To = scaleX,
			};
			Storyboard.SetTargetProperty(xs, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
			Storyboard.SetTarget(xs, i);
			DoubleAnimation ys = new DoubleAnimation()
			{
				Duration = time,
				To = scaleY,
			};
			Storyboard.SetTargetProperty(ys, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
			Storyboard.SetTarget(ys, i);


			sb.Children.Add(xa);
			sb.Children.Add(ya);
			sb.Children.Add(xs);
			sb.Children.Add(ys);
			sb.Begin();
			sb.Completed += (o, e) => { canvas.Children.Remove(i); };
		}
	}
}
