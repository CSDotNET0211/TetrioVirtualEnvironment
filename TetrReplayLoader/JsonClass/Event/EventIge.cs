﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass.Event
{
	public class EventIge:Event
	{
		public EventIgeData? data { get; set; } = null;

	}
	public class EventIgeData
	{
		public int? id { get; set; }
		public int frame { get; set; }
		public string type { get; set; }
		public IgeData data { get; set; }
	}
}
