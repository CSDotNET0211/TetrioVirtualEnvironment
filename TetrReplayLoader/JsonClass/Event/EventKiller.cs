
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventKiller:Event
	{
		public EventKillerData? data { get; set; } = null;

	}

	public class EventKillerData
    {
        public string? name { get; set; }
        public string? type { get; set; }
    }

}
