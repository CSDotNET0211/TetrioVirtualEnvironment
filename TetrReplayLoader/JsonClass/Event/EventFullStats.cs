namespace TetrReplayLoader.JsonClass.Event
{
	public class EventFullStats : Event
	{
		public EventFullStats(EventFullStatsData data) : base(null, null, null)
		{
			this.data = data;
		}

		public new EventFullStatsData data { get; set; }

	}

	public class EventFullStatsData
	{
		public double? seed { get; set; }
		public int? lines { get; set; }
		public int? level_lines { get; set; }
		public int? level_lines_needed { get; set; }
		public int? inputs { get; set; }
		public int? holds { get; set; }
		public int? score { get; set; }
		public int? zenlevel { get; set; }
		public int? zenprogress { get; set; }
		public int? level { get; set; }
		public int? combo { get; set; }
		public int? currentcombopower { get; set; }
		public int? topcombo { get; set; }
		public int? btb { get; set; }
		public int? topbtb { get; set; }
		public int? tspins { get; set; }
		public int? piecesplaced { get; set; }
		public int? kills { get; set; }
	}
}
