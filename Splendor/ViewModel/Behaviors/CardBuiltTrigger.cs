namespace Splendor.ViewModel
{
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
}
