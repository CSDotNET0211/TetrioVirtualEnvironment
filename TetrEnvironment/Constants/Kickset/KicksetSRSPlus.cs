using Vector2 = TetrEnvironment.Struct.Vector2;

namespace TetrEnvironment.Constants.Kickset;

public class KicksetSRSPlus : KicksetBase
{
	public override Vector2[]? ADDITIONAL_OFFSETS { get; protected set; } = null;

	public override Vector2[] ADDITIONAL_OFFSETS_EMPTY { get; protected set; } =
	{
		Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero
	};

	public override int[]? SPAWN_ROTATION { get; protected set; } = null;

	public override Dictionary<int, Vector2[]> KICKSET_TABLE { get; protected set; }
		= new()
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

	public override Dictionary<int, Vector2[]> KICKSET_TABLE_I { get; protected set; }
		= new()
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