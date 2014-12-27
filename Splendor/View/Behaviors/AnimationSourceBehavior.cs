namespace Splendor.View
{
	using GalaSoft.MvvmLight.Ioc;
	using Microsoft.Xaml.Interactivity;
	using Windows.UI.Xaml;

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
		}

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
