using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass
{
	public class GarbageData
	{
		public GarbageData Clone()
		{
			GarbageData newdata = new GarbageData()
			{
				iid = iid,
				type = type,
				amt = amt,
				ackiid = ackiid,
				x = x,
				y = y,
				column = column
			};

			return newdata;
		}
		public int iid { get; set; }
		public string type { get; set; }
		public int amt { get; set; }
		public int ackiid { get; set; }
		public int x { get; set; }
		public int y { get; set; }
		public int column { get; set; }

	}

}
