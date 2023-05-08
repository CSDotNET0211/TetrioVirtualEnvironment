
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventTargets : Event
	{
		public EventTargetsData? data { get; set; } = null;

	}

	public class EventTargetsData
    {
        public string? id { get; set; } = null;
        public int? frame { get; set; } = null;
        public string? type { get; set; } = null;
        public List<string>? data { get; set; } = null;
    }
}
