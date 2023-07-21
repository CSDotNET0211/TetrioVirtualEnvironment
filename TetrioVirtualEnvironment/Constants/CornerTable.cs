namespace TetrioVirtualEnvironment.Constants;

public class CornerTable
{
	/// <summary>
			/// This is used to judge spin bonus.
			/// Basically, used as Tspin.
			/// AllSpin uses all data.
			/// </summary>
			internal static readonly Vector2[][][] TABLE =
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