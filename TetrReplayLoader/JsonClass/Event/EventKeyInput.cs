using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass.Event
{
	public class EventKeyInput : Event
	{
		public EventKeyInput(int? id, int frame, string type, EventKeyInputData data) : base(id, frame, type)
		{
			this.data = data;
		}

		public new EventKeyInputData data { get; set; } 

	}

	public class EventKeyInputData
	{
		public string key { get; set; }
		public double subframe { get; set; }
		public bool hoisted { get; set; }
	}

}
