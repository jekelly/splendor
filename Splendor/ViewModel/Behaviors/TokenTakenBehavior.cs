namespace Splendor.ViewModel
{
	using System;
	using GalaSoft.MvvmLight.Ioc;
	using Microsoft.Xaml.Interactivity;
	using Splendor.Model;
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

	public sealed class TokenGainedTrigger : EventServiceTrigger
	{
		protected override void BindEventService(EventService eventService)
		{
			eventService.TokenTaken += this.OnTokenTaken;
		}

		private void OnTokenTaken(object sender, TokenEventArgs e)
		{
			if (e.Color == this.Color && e.PlayerIndex == this.PlayerIndex)
			{
				Interaction.ExecuteActions(this, this.Actions, null);
			}
		}
	}

	public sealed class CardBuiltTrigger : EventServiceTrigger
	{
		protected override void BindEventService(EventService eventService)
		{
			eventService.CardBuilt += this.OnCardBuilt;
		}

		private void OnCardBuilt(object sender, CardEventArgs e)
		{
			if (e.Card.Gives == this.Color && e.Player.Index == this.PlayerIndex)
			{
				Interaction.ExecuteActions(this, this.Actions, null);
			}
		}
	}

	public abstract class EventServiceTrigger : DependencyObject, IBehavior
	{
		private readonly ActionCollection actions;
		private readonly EventService eventService;
		private DependencyObject associatedObject;

		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(EventServiceTrigger), new PropertyMetadata(Color.Gold));
		public static readonly DependencyProperty PlayerIndexProperty = DependencyProperty.Register("PlayerIndex", typeof(int), typeof(EventServiceTrigger), new PropertyMetadata(-1));

		public Color Color
		{
			get { return (Color)this.GetValue(ColorProperty); }
			set { this.SetValue(ColorProperty, value); }
		}

		public int PlayerIndex
		{
			get { return (int)this.GetValue(PlayerIndexProperty); }
			set { this.SetValue(PlayerIndexProperty, value); }
		}

		public ActionCollection Actions { get { return this.actions; } }

		public EventServiceTrigger()
		{
			this.actions = new ActionCollection();
			this.eventService = SimpleIoc.Default.GetInstance<EventService>();
		}

		public DependencyObject AssociatedObject
		{
			get { return this.associatedObject; }
		}

		public void Attach(DependencyObject associatedObject)
		{
			this.associatedObject = associatedObject;
			this.BindEventService(this.eventService);
		}

		protected abstract void BindEventService(EventService eventService);

		public void Detach()
		{
			this.associatedObject = null;
		}
	}
}
