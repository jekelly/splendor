namespace Splendor.ViewModel
{
	using GalaSoft.MvvmLight.Ioc;
	using Splendor.ViewModel;

	public class ViewModelLocator
	{
		static ViewModelLocator()
		{
			SimpleIoc.Default.Register<GameService>();
			SimpleIoc.Default.Register<EventService>();
			SimpleIoc.Default.Register<GameViewModel>();
		}

		public ViewModelLocator()
		{
		}

		public GameViewModel GameViewModel { get { return SimpleIoc.Default.GetInstance<GameViewModel>(); } }
	}
}
