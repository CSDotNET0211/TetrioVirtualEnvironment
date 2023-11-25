using TetrEnvironment.Constants;
using TetrEnvironment.Struct;

namespace TetrioVirtualEnvironment.Constants;

public class CornerTable
{
	public static Vector2[][] GetTable(Tetromino.MinoType type)
	{
		int index;
		switch (type)
		{
			case Tetromino.MinoType.Z:
			case Tetromino.MinoType.L:
				index = (int)type;
				break;

			case Tetromino.MinoType.S:
				index = (int)type - 1;
				break;

			case Tetromino.MinoType.J:
			case Tetromino.MinoType.T:
				index = (int)type - 2;
				break;

			default: throw new Exception("Unknown type: " + type ?? "null");
		}

		return _table[index];
	}

	/// <summary>
	/// This is used to judge spin bonus.
	/// Basically, used as Tspin.
	/// AllSpin uses all data.
	/// </summary>
	private static readonly Vector2[][][] _table =
	{
		//Z
		new[]
		{
			new[]
			{
				new Vector2(-2, -1), new Vector2(1, -1), new Vector2(2, 0), new Vector2(-1, 0)
			},
			new[]
			{
				new Vector2(0, -1), new Vector2(1, -2), new Vector2(0, 2), new Vector2(1, 2)
			},
			new[]
			{
				new Vector2(-2, 0), new Vector2(1, 0), new Vector2(2, 1), new Vector2(-1, 1)
			},
			new[]
			{
				new Vector2(-1, -1), new Vector2(0, -2), new Vector2(0, 1), new Vector2(-1, 2)
			},
		},

		//L
		new[]
		{
			new[]
			{
				new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, 1)
			},
			new[]
			{
				new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(-1, 1)
			},
			new[]
			{
				new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(0, 1)
			},
			new[]
			{
				new Vector2(-1, 0), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1)
			},
		},


		//S
		new[]
		{
			new[]
			{
				new Vector2(-1, -1), new Vector2(2, -1), new Vector2(1, 0), new Vector2(-2, 0)
			},
			new[]
			{
				new Vector2(0, -2), new Vector2(1, -1), new Vector2(1, 2), new Vector2(0, 1)
			},
			new[]
			{
				new Vector2(-1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(-2, 1)
			},
			new[]
			{
				new Vector2(-1, -2), new Vector2(0, -1), new Vector2(-1, 1), new Vector2(0, 2)
			},
		},

		//J
		new[]
		{
			new[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
			new[] { new Vector2(-1, -1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(-1, 1) },
			new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(0, 1), new Vector2(-1, 1) },
			new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 0) },
		},

		//T
		new[]
		{
			new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
			new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
			new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
			new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
		},
	};

	internal static readonly Vector2[][] ADDITIONAL_TABLE =
	{
		new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
		new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
		new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
		new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
	};
}