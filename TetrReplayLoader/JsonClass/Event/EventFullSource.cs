
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventFullSource : Event
	{
		public EventFullSource(EventFullSourceData data) : base(null, null, null)
		{
			this.data = data;
		}

		public new EventFullSourceData data { get; set; }

	}

	public class EventFullSourceData
	{
		public object? advanceFrame { get; set; }
		public object? getFrame { get; set; }
		public object? readyEventQueue { get; set; }
		public object? pull { get; set; }
		public object? nextFrameReady { get; set; }
		public object? fallingBehind { get; set; }
		public object? amountToCatchUp { get; set; }
		public object? behindness { get; set; }
		public object? bind { get; set; }
		public object? unbind { get; set; }
		public object? seek { get; set; }
		public object? finished { get; set; }
		public object? type { get; set; }
		public object? pushIGE { get; set; }
		public object? pushTargets { get; set; }
		public object? unhook { get; set; }
		public object? destroy { get; set; }
		public object? socket { get; set; }
		public object? bindHyperRetry { get; set; }
		public object? bindHyperForfeit { get; set; }

	}

}
