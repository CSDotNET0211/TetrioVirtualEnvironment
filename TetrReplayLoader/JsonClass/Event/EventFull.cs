
namespace TetrReplayLoader.JsonClass.Event
{
	public class EventFull : Event
	{
		public EventFull(int? id,int frame,string type, EventFullData data) : base(id,frame,type)
		{
			this.data = data;
		}

		public new EventFullData data { get; set; }
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
