namespace Splendor.ViewModel
{
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
				await this.InvokeAsync(e);
			}
		}
	}
}
