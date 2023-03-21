using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public DataForInitialize(int[] field, int hold, int current, int[] next, int[]? garbages)
		{
			Field = field;
			Hold = hold;
			Current = current;
			Next = next;
			Garbages = garbages;
		}

		public int[]? Field { get; }
		public int? Hold { get; }
		public int? Current { get; }
		public int[]? Next { get; }
		public int[]? Garbages { get; }

	}
}
