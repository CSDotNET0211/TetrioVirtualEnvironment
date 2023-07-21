namespace TetrioVirtualEnvironment.Constants;

public abstract class Tetrimino
{
	public enum MinoType : byte
	{
		Z,
		L,
		O,
		S,
		I,
		J,
		T,
		Garbage,
		Empty
	}

	/// <summary>
	/// for next generating
	/// </summary>
	internal static readonly MinoType[] MINOTYPES =
		{ MinoType.Z, MinoType.L, MinoType.O, MinoType.S, MinoType.I, MinoType.J, MinoType.T, };

	/// <summary>
	/// Tetrimino Array
	/// </summary>
	public static readonly Vector2[][][] SHAPES =
	{
		//Z
		new[]
		{
			new[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)
			},
			new[]
			{
				new Vector2(2, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2)
			},
			new[]
			{
				new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2)
			},
			new[]
			{
				new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 2)
			},
		},

		//L
		new[]
		{
			new[]
			{
				new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1)
			},
			new[]
			{
				new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2)
			},
			new[]
			{
				new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2)
			},
			new[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2)
			},
		},

		//O
		new[]
		{
			new[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
			},
			new[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
			},
			new[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
			},
			new[]
			{
				new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
			},
		},

		//S
		new[]
		{
			new[]
			{
				new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1)
			},
			new[]
			{
				new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2)
			},
			new[]
			{
				new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2), new Vector2(1, 2)
			},
			new[]
			{
				new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2)
			},
		},

		//I
		new[]
		{
			new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(3, 1) },
			new[] { new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2), new Vector2(2, 3) },
			new[] { new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2), new Vector2(3, 2) },
			new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(1, 3) },
		},


		//J
		new[]
		{
			new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) },
			new[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(1, 2) },
			new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2) },
			new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 2), new Vector2(1, 2) },
		},

		//T
		new[]
		{
			new[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) },
			new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) },
			new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) },
			new[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) },
		},
	};

	/// <summary>
	/// Diff Position for Fix Tetrimino Position
	/// </summary>
	public static readonly Vector2[] DIFFS =
		{ new(1, 1), new(1, 1), new(0, 1), new(1, 1), new(1, 1), new(1, 1), new(1, 1) };
}