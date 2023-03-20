using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass.Event
{
    public class EventKeyInput
    {
        public string key { get; set; }
        public double subframe { get; set; }
        public bool hoisted { get; set; }
    }

}
