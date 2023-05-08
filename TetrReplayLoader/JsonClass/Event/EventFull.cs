
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventFull : Event
	{

		public EventFullData? data { get; set; } = null;
	}

	public class EventFullData
	{

		public bool? successful { get; set; } = null;
		public object? gameoverreason { get; set; } = null;
		public int? fire { get; set; } = null;

		public EventFullReplayData? replay { get; set; } = null;
		public EventFullSourceData? source { get; set; } = null;
		public EventFullOptionsData? options { get; set; } = null;
		public EventFullStatsData? stats { get; set; } = null;
		public EventFullGameData? game { get; set; } = null;
	}
}
