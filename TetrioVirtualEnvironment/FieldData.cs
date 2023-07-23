using TetrioVirtualEnvironment.Constants;
using TetrLoader.JsonClass;

namespace TetrioVirtualEnvironment
{
	public class FieldData
	{
		public FieldData()
		{
			Board = null;
			Hold = Tetrimino.MinoType.Empty;
			Next = null;
			Current = Tetrimino.MinoType.Empty;
			Garbages = null;
		}

		public FieldData(Tetrimino.MinoType[] board, Tetrimino.MinoType hold, Tetrimino.MinoType current, Tetrimino.MinoType[] next, IgeData[] garbages)
		{
			Board = board;
			Hold = hold;
			Current = current;
			Next = next;
			Garbages = new List<IgeData?>(garbages);
		}

		public Tetrimino.MinoType[]? Board { get; }
		public Tetrimino.MinoType Hold { get; }
		public Tetrimino.MinoType Current { get; }
		public Tetrimino.MinoType[]? Next { get; }
		public List<IgeData?>? Garbages { get; }

	}
}
