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
			Board = null;
			Hold = null;
			Next = null;
			Current = null;
			Garbages = null;
		}

		public DataForInitialize(int[] board, int hold, int current, int[] next, IgeData[]? garbages)
		{
			Board = board;
			Hold = hold;
			Current = current;
			Next = next;
			Garbages = new List<IgeData>(garbages);
		}

		public int[]? Board { get; }
		public int? Hold { get; }
		public int? Current { get; }
		public int[]? Next { get; }
		public List<IgeData?> Garbages { get; }

	}
}
