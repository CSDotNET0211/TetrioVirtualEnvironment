using System.Numerics;
using Vector2 = TetrEnvironment.Struct.Vector2;

namespace TetrEnvironment.Constants;

//TODO:オブジェクト指向使っていい感じにする
public static class Kickset
{
	internal static readonly Vector2[]? ADDITIONAL_OFFSETS = null;

	internal static readonly Vector2[] ADDITIONAL_OFFSETS_EMPTY =
	{
		Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero
	};

	internal static readonly int[]? SPAWN_ROTATION = null;

	/// <summary>
	/// Kickset of SRS+
	/// </summary>
	internal static readonly Dictionary<int, Vector2[]> SRSPLUS = new()
	{
		{
			01,
			new[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2), }
		},
		{
			10,
			new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2), }
		},
		{
			12,
			new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2), }
		},
		{
			21,
			new[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2), }
		},
		{
			23,
			new[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2), }
		},
		{
			32,
			new[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2), }
		},
		{
			30,
			new[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2), }
		},
		{
			03,
			new[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2), }
		},
		{
			02,
			new[]
			{
				new Vector2(0, -1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(1, 0),
				new Vector2(-1, 0),
			}
		},
		{
			13,
			new[]
			{
				new Vector2(1, 0), new Vector2(1, -2), new Vector2(1, -1), new Vector2(0, -2),
				new Vector2(0, -1),
			}
		},
		{
			20,
			new[]
			{
				new Vector2(0, 1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(1, 0),
			}
		},
		{
			31,
			new[]
			{
				new Vector2(-1, 0), new Vector2(-1, -2), new Vector2(-1, -1), new Vector2(0, -2),
				new Vector2(0, -1),
			}
		}
	};

	/// <summary>
	/// Kickset of SRS+ I-piece
	/// </summary>
	internal static readonly Dictionary<int, Vector2[]> SRSPLUS_I = new()
	{
		{ 01, new[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(-2, 1), new Vector2(1, -2), } },
		{ 10, new[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, 2), new Vector2(2, -1), } },
		{ 12, new[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(2, 1), } },
		{ 21, new[] { new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, -1), new Vector2(1, 2), } },
		{ 23, new[] { new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2), } },
		{ 32, new[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, -2), new Vector2(-2, 1), } },
		{ 30, new[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1), } },
		{ 03, new[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(2, 1), new Vector2(-1, -2), } },
		{ 02, new[] { new Vector2(0, -1) } },
		{ 13, new[] { new Vector2(1, 0) } },
		{ 20, new[] { new Vector2(0, 1) } },
		{ 31, new[] { new Vector2(-1, 0) } },
	};
}