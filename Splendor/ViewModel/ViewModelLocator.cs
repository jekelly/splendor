namespace Splendor.ViewModel
{
	using GalaSoft.MvvmLight.Ioc;
	using Splendor.ViewModel;

	public class ViewModelLocator
	{
		static ViewModelLocator()
		{
			SimpleIoc.Default.Register<AnimationService>();
			SimpleIoc.Default.Register<CommandService>();
			SimpleIoc.Default.Register<GameService>();
			SimpleIoc.Default.Register<EventService>();
			SimpleIoc.Default.Register<GameViewModel>();
		}

		public ViewModelLocator()
		{
		}

		public GameViewModel GameViewModel { get { return SimpleIoc.Default.GetInstance<GameViewModel>(); } }

		public CommandService CommandService { get { return SimpleIoc.Default.GetInstance<CommandService>(); } }

		public EventService EventService { get { return SimpleIoc.Default.GetInstance<EventService>(); } }
	}
}
