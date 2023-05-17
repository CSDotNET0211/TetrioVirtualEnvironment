using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoader.JsonClass;
using static TetrioVirtualEnvironment.Environment;

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

		public DataForInitialize(MinoKind[] board, MinoKind hold, MinoKind current, MinoKind[] next, IgeData[] garbages)
		{
			Board = board;
			Hold = hold;
			Current = current;
			Next = next;
			Garbages = new List<IgeData?>(garbages);
		}

		public MinoKind[]? Board { get; }
		public MinoKind? Hold { get; }
		public MinoKind? Current { get; }
		public MinoKind[]? Next { get; }
		public List<IgeData?>? Garbages { get; }

	}
}
