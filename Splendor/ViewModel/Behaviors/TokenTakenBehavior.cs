﻿namespace Splendor.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using GalaSoft.MvvmLight.Ioc;
	using Microsoft.Xaml.Interactivity;
	using Splendor.Model;
	using Windows.Foundation;
	using Windows.UI.Core;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;
	using Windows.UI.Xaml.Media;
	using Windows.UI.Xaml.Media.Animation;
	using Windows.UI.Xaml.Media.Imaging;

	public class AnimationService
	{
		private readonly Dictionary<object, UIElement> sources;
		public AnimationService()
		{
			this.sources = new Dictionary<object, UIElement>();
		}

		internal void Register(object identifier, UIElement dependencyObject)
		{
			if (identifier == null) return;
			this.sources.Add(identifier, dependencyObject);
		}

		internal void Unregister(object identifier)
		{
			if (identifier == null) return;
			this.sources.Remove(identifier);
		}

		internal UIElement GetSource(object identifier)
		{
			return this.sources[identifier];
		}
	}

	public sealed class AnimationSourceBehavior : DependencyObject, Microsoft.Xaml.Interactivity.IBehavior
	{
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

		public static readonly DependencyProperty IdentifierProperty = DependencyProperty.Register("Identifier", typeof(object), typeof(AnimationSourceBehavior), new PropertyMetadata(null, OnIdentiferChanged));

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
	}

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

		private async void OnTokenTaken(object sender, TokenEventArgs e)
		{
			if (e.Color == this.Color && e.PlayerIndex == this.PlayerIndex)
			{
				await this.InvokeAsync(null);
			}
		}
	}

	public sealed class CardBuiltTrigger : EventServiceTrigger
	{
		protected override void BindEventService(EventService eventService)
		{
			eventService.CardBuilt += this.OnCardBuilt;
		}

		private async void OnCardBuilt(object sender, CardEventArgs e)
		{
			if (e.Card.Gives == this.Color && e.Player.Index == this.PlayerIndex)
			{
				await this.InvokeAsync(null);
			}
		}
	}

	public abstract class EventServiceTrigger : DependencyObject, IBehavior
	{
		private readonly EventService eventService;
		private DependencyObject associatedObject;

		public ActionCollection Actions
		{
			get { return (ActionCollection)GetValue(ActionsProperty); }
			set { SetValue(ActionsProperty, value); }
		}

		public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(EventServiceTrigger), new PropertyMetadata(null));
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

		private readonly CoreDispatcher uiDispatcher;
		public EventServiceTrigger()
		{
			this.Actions = new ActionCollection();
			this.eventService = SimpleIoc.Default.GetInstance<EventService>();
			this.uiDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
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

		protected async Task InvokeAsync(object parameter)
		{
			List<Task> tasks = new List<Task>();
			await this.uiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					foreach(Microsoft.Xaml.Interactivity.IAction action in this.Actions)
					{
						var result = action.Execute(this, parameter);
						if (result is Task) 
						{
							tasks.Add((Task)result); 
						}
					}
				});
			await Task.WhenAll(tasks.ToArray());
		}
	}
}
