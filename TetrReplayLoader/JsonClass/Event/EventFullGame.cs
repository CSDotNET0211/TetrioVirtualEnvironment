using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass.Event
{
	public class EventFullGame:Event
	{
		public EventFullGame? data { get; set; } = null;
	}
    public class EventFullGameData
    {
        public Handling? handling { get; set; } = null;



    }
}
