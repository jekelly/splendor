namespace Splendor.View
{
	using System.Collections.Generic;
	using Windows.UI.Xaml;

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
			this.sources[identifier] = dependencyObject;
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
}
