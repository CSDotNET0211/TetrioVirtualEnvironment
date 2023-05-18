
using System.Dynamic;

namespace TetrReplayLoader.JsonClass.Event
{
    public  class Event
    {
        public int id { get; set; }
        public int frame { get; set; } 
        public string type { get; set; } 
        public object data { get; set; } 
    }
}
