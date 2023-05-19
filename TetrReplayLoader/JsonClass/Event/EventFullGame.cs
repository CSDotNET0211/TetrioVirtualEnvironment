using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass.Event
{
	public class EventFullGame : Event
	{
		public EventFullGame(EventFullGame data) : base(null, null, null)
		{
			this.data = data;
		}

		public new EventFullGame data { get; set; }
	}
	public class EventFullGameData
	{
		public Handling? handling { get; set; } = null;



	}
}
