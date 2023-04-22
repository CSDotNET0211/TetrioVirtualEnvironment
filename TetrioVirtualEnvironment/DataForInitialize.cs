using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoader.JsonClass;

namespace TetrioVirtualEnvironment
{
	public class DataForInitialize
	{
		public DataForInitialize()
		{
			Field = null;
			Hold = null;
			Next = null;
			Current = null;
			Garbages = null;
		}

		public DataForInitialize(int[] field, int hold, int current, int[] next, IgeData[]? garbages)
		{
			Field = field;
			Hold = hold;
			Current = current;
			Next = next;
			Garbages = new List<IgeData>(garbages);
		}

		public int[]? Field { get; }
		public int? Hold { get; }
		public int? Current { get; }
		public int[]? Next { get; }
		public List<IgeData?> Garbages { get; }

	}
}
