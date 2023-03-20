
namespace TetrReplayLoader.JsonClass.Event
{
    public class EventFull
    {

        public bool? successful { get; set; } = null;
        public object? gameoverreason { get; set; } = null;
        public int? fire { get; set; } = null;

        public EventFullReplay? replay { get; set; } = null;
        public EventFullSource? source { get; set; } = null;
        public EventFullOptions? options { get; set; } = null;
        public EventFullStats? stats { get; set; } = null;
        public EventFullGame? game { get; set; } = null;
    }
}
