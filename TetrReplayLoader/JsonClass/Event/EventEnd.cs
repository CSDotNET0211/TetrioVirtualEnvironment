
namespace TetrReplayLoader.JsonClass.Event
{

	public class EventEnd:Event
	{
		public EventEndData? data { get; set; } = null;
	}

	public class EventEndData
    {
        public string? reason { get; set; } = null;
        public Export? export { get; set; } = null;
    }
}
