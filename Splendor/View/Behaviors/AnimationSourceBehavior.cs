namespace Splendor.View
{
	using System.Collections.Specialized;
	using GalaSoft.MvvmLight.Ioc;
	using Microsoft.Xaml.Interactivity;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;
	using Windows.UI.Xaml.Media;

	public sealed class AnimationSourceBehavior : DependencyObject, IBehavior
	{
		public static readonly DependencyProperty IdentifierProperty = DependencyProperty.Register("Identifier", typeof(object), typeof(AnimationSourceBehavior), new PropertyMetadata(null, OnIdentiferChanged));

		private readonly AnimationService animationService;

		public object Identifier
		{
			get { return (object)GetValue(IdentifierProperty); }
			set
			{
				if (this.Identifier != value)
				{
					SetValue(IdentifierProperty, value);
				}
			}
		}

		public AnimationSourceBehavior()
		{
			this.animationService = SimpleIoc.Default.GetInstance<AnimationService>();
		}

		public DependencyObject AssociatedObject
		{
			get;
			private set;
		}

		public void Attach(DependencyObject associatedObject)
		{
			this.AssociatedObject = associatedObject;
			this.animationService.Register(this.Identifier, (UIElement)this.AssociatedObject);
			//((FrameworkElement)associatedObject).Loaded += (o, e) =>
			//	{
			//		// see if we're coming from a notifying container, and hook up an unregistration notice if so
			//		DependencyObject root = associatedObject;
			//		while (root != null)
			//		{
			//			ItemsControl ic = root as ItemsControl;
			//			if (ic != null)
			//			{
			//				INotifyCollectionChanged ncc = ic.ItemsSource as INotifyCollectionChanged;
			//				if (ncc != null)
			//				{
			//					ncc.CollectionChanged += ncc_CollectionChanged;
			//				}
			//				break;
			//			}
			//			root = VisualTreeHelper.GetParent(root);
			//		}
			//	};
		}

		//void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//}

		public void Detach()
		{
			this.animationService.Unregister(this.Identifier);
			this.AssociatedObject = null;
		}

		private static void OnIdentiferChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AnimationSourceBehavior behavior = (AnimationSourceBehavior)d;

			if (e.OldValue != null)
			{
				behavior.animationService.Unregister(e.OldValue);
			}
			if (e.NewValue != null && behavior.AssociatedObject != null)
			{
				behavior.animationService.Register(e.NewValue, (UIElement)behavior.AssociatedObject);
			}
		}
	}
}
