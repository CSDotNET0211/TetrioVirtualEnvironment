
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventKiller : Event
	{
		public EventKiller(string type, EventKillerData data) : base(null, null, type)
		{
			this.data = data;
		}

		public new EventKillerData data { get; set; }

	}

	public class EventKillerData
	{
		public string? name { get; set; }
		public string? type { get; set; }
	}

}
