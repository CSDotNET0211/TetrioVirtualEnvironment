
using System.Dynamic;

namespace TetrReplayLoader.JsonClass.Event
{
    public  class Event
    {
        public int? id { get; set; } = null;
        public int? frame { get; set; } = null;
        public string? type { get; set; } = null;
        public object? data { get; set; } = null;
    }
}
