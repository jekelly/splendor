namespace Splendor.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using GalaSoft.MvvmLight.Ioc;
	using Microsoft.Xaml.Interactivity;
	using Splendor.Model;
	using Windows.UI.Core;
	using Windows.UI.Xaml;

	public abstract class EventServiceTrigger : DependencyObject, IBehavior
	{
		public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(EventServiceTrigger), new PropertyMetadata(null));
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(EventServiceTrigger), new PropertyMetadata(Color.Gold));
		public static readonly DependencyProperty PlayerIndexProperty = DependencyProperty.Register("PlayerIndex", typeof(int), typeof(EventServiceTrigger), new PropertyMetadata(-1));

		private readonly EventService eventService;
		private readonly CoreDispatcher uiDispatcher;
		private DependencyObject associatedObject;

		public ActionCollection Actions
		{
			get { return (ActionCollection)GetValue(ActionsProperty); }
			set { SetValue(ActionsProperty, value); }
		}

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

		public void Detach()
		{
			this.associatedObject = null;
		}
	
		protected abstract void BindEventService(EventService eventService);

		protected async Task InvokeAsync(object parameter)
		{
			List<Task> tasks = new List<Task>();
			await this.uiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				foreach (Microsoft.Xaml.Interactivity.IAction action in this.Actions)
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
