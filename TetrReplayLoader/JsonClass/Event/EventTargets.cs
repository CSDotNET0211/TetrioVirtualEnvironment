
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventTargets : Event
	{
		public EventTargets(int? id, int frame, string type, EventTargetsData data) : base(id, frame, type)
		{
			this.data = data;
		}

		public new EventTargetsData? data { get; set; }

	}

	public class EventTargetsData
	{
		public string? id { get; set; } = null;
		public int? frame { get; set; } = null;
		public string? type { get; set; } = null;
		public List<string>? data { get; set; } = null;
	}
}
