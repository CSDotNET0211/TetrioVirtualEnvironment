using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.Constants;
using TetrReplayLoader.JsonClass;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment
{
	public class DataForInitialize
	{
		public DataForInitialize()
		{
			Board = null;
			Hold = Tetrimino.MinoType.Empty;
			Next = null;
			Current = Tetrimino.MinoType.Empty;
			Garbages = null;
		}

		public DataForInitialize(Tetrimino.MinoType[] board, Tetrimino.MinoType hold, Tetrimino.MinoType current, Tetrimino.MinoType[] next, IgeData[] garbages)
		{
			Board = board;
			Hold = hold;
			Current = current;
			Next = next;
			Garbages = new List<IgeData?>(garbages);
		}

		public Tetrimino.MinoType[] Board { get; }
		public Tetrimino.MinoType Hold { get; }
		public Tetrimino.MinoType Current { get; }
		public Tetrimino.MinoType[] Next { get; }
		public List<IgeData?>? Garbages { get; }

	}
}
