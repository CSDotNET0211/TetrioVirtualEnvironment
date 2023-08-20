using TetrioVirtualEnvironment.Constants;
using TetrLoader.Enum;

namespace TetrioVirtualEnvironment.Env;

public partial class Environment
{
	private Falling.SpinTypeKind IsTspin()
	{
		if (GameData.SpinBonuses == SpinBonusesType.None)
			return Falling.SpinTypeKind.Null;

		if (GameData.SpinBonuses == SpinBonusesType.Stupid)
			throw new NotImplementedException();

		if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, GameData.Falling.Y + 1, GameData.Falling.R,
			    GameData.Board))
			return Falling.SpinTypeKind.Null;

		if (GameData.Falling.Type != Tetrimino.MinoType.T && GameData.SpinBonuses != SpinBonusesType.Handheld)
		{
			if (GameData.SpinBonuses == SpinBonusesType.All)
			{
				if (!(IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X - 1, GameData.Falling.Y,
					      GameData.Falling.R, GameData.Board) ||
				      IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X + 1, GameData.Falling.Y,
					      GameData.Falling.R, GameData.Board) ||
				      IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, GameData.Falling.Y - 1,
					      GameData.Falling.R, GameData.Board) ||
				      IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, GameData.Falling.Y + 1,
					      GameData.Falling.R, GameData.Board)))
					return Falling.SpinTypeKind.Normal;
				else
					return Falling.SpinTypeKind.Null;
			}
			else
				return Falling.SpinTypeKind.Null;
		}

		if (GameData.Falling.Last != Falling.LastKind.Rotate)
			return Falling.SpinTypeKind.Null;

		var cornerCount = 0;
		var a = 0;

		for (int index = 0; index < 4; index++)
		{
			Vector2[][]? minoCorner = null;

			minoCorner = CornerTable.TABLE[(int)GetIndex(GameData.Falling.Type)];

			Tetrimino.MinoType GetIndex(Tetrimino.MinoType type)
			{
				switch (type)
				{
					case Tetrimino.MinoType.Z:
					case Tetrimino.MinoType.L:
						return type;
					case Tetrimino.MinoType.S:
						return type - 1;
					case Tetrimino.MinoType.J:
					case Tetrimino.MinoType.T:
						return type - 2;

					default: throw new Exception("Unknown type: " + type ?? "null");
				}
			}

			if (!IsEmptyPos((int)(GameData.Falling.X + minoCorner[GameData.Falling.R][index].x),
				    (int)(GameData.Falling.Y + minoCorner[GameData.Falling.R][index].y), GameData.Board))
			{
				cornerCount++;

				//AdditinalTableは無理やり追加したものなのでx,yは関係ない
				if (!(GameData.Falling.Type != Tetrimino.MinoType.T ||
				      (GameData.Falling.R != CornerTable.ADDITIONAL_TABLE[GameData.Falling.R][index].x &&
				       GameData.Falling.R != CornerTable.ADDITIONAL_TABLE[GameData.Falling.R][index].y)))
					a++;
			}
		}


		if (cornerCount < 3)
			return Falling.SpinTypeKind.Null;

		var spinType = Falling.SpinTypeKind.Normal;

		if (GameData.Falling.Type == Tetrimino.MinoType.T && a != 2)
			spinType = Falling.SpinTypeKind.Mini;

		if (GameData.Falling.LastKick == 4)
			spinType = Falling.SpinTypeKind.Normal;


		return spinType;
	}


	/// <summary>
	/// Judge position is in the field and selected position is empty.
	/// </summary>
	/// <param name="x">absolute position</param>
	/// <param name="y">absolute position</param>
	/// <param name="field"></param>
	/// <returns>Is empty</returns>
	private static bool IsEmptyPos(int x, int y, IReadOnlyList<Tetrimino.MinoType> field)
	{
		if (!(x is >= 0 and < FIELD_WIDTH &&
		      y is >= 0 and < FIELD_HEIGHT))
			return false;

		return field[x + y * 10] == Tetrimino.MinoType.Empty;
	}

	/// <summary>
	/// Check tetromino is in the field and not being overwraped.
	/// remove this
	/// </summary>
	/// <param name="type"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="rotation"></param>n 
	/// <returns></returns>
	public static bool IsLegalField(Tetrimino.MinoType type, int x, double y, int rotation)
	{
		var positions = Tetrimino.SHAPES[(int)type][rotation];
		var diff = Tetrimino.DIFFS[(int)type];

		foreach (var position in positions)
		{
			if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
			      position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT))
				return false;
		}

		return true;
	}

	/// <summary>
	/// Check x,y is in the field.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static bool IsLegalField(int x, double y)
	{
		return x is >= 0 and < FIELD_WIDTH &&
		       y is >= 0 and < FIELD_HEIGHT;
	}

	/// <summary>
	/// Check tetromino is in the field and not being overwraped.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="rotation"></param>
	/// <param name="field"></param>
	/// <returns></returns>
	public static bool IsLegalAtPos(Tetrimino.MinoType type, int x, double y, int rotation,
		IReadOnlyList<Tetrimino.MinoType> field)
	{
		var positions = Tetrimino.SHAPES[(int)type][rotation];
		var diff = Tetrimino.DIFFS[(int)type];

		foreach (var position in positions)
		{
			if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
			      position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT &&
			      field[(int)position.x + x - (int)diff.x + (int)(position.y + y - (int)diff.y) * 10] ==
			      Tetrimino.MinoType.Empty))
				return false;
		}

		return true;
	}

}