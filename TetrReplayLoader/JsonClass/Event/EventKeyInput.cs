using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass.Event
{
	public class EventKeyInput:Event
	{
		public EventKeyInputData? data { get; set; } = null;

	}

	public class EventKeyInputData
    {
        public string key { get; set; }
        public double subframe { get; set; }
        public bool hoisted { get; set; }
    }

}
