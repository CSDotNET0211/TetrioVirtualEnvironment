using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.Interface;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment.System
{
	public class JudgeSystem : ISystem
	{
		internal static string? IsTspin(GameData gameData)
		{
			if (gameData.SpinBonuses == "none")
				return null;

			if (gameData.SpinBonuses == "stupid")
				throw new NotImplementedException();

			if (IsLegalAtPos((MinoKind)gameData.Falling.Type, gameData.Falling.X, gameData.Falling.Y + 1, gameData.Falling.R, gameData.Board))
				return null;

			if (gameData.Falling.Type != MinoKind.T && gameData.SpinBonuses != "handheld")
			{
				if (gameData.SpinBonuses == "all")
				{
					if (!(IsLegalAtPos((MinoKind)gameData.Falling.Type, gameData.Falling.X - 1, gameData.Falling.Y, gameData.Falling.R, gameData.Board) ||
					   IsLegalAtPos((MinoKind)gameData.Falling.Type, gameData.Falling.X + 1, gameData.Falling.Y, gameData.Falling.R, gameData.Board) ||
					   IsLegalAtPos((MinoKind)gameData.Falling.Type, gameData.Falling.X, gameData.Falling.Y - 1, gameData.Falling.R, gameData.Board) ||
					   IsLegalAtPos((MinoKind)gameData.Falling.Type, gameData.Falling.X, gameData.Falling.Y + 1, gameData.Falling.R, gameData.Board)))
						return "normal";
					else
						return null;
				}
				else
					return null;

			}

			if (gameData.Falling.Last != "rotate")
				return null;

			var cornerCount = 0;
			var a = 0;

			for (int index = 0; index < 4; index++)
			{
				Vector2[][]? minoCorner = null;

				minoCorner = ConstData.CORNER_TABLE[(int)GetIndex((MinoKind)gameData.Falling.Type)];

				MinoKind GetIndex(MinoKind type)
				{
					switch (type)
					{
						case MinoKind.Z:
						case MinoKind.L:
							return type;
						case MinoKind.S:
							return type - 1;
						case MinoKind.J:
						case MinoKind.T:
							return type - 2;

						default: throw new Exception("Unknown type: " + type ?? "null");

					}
				}

				if (!IsEmptyPos((int)(gameData.Falling.X + minoCorner[gameData.Falling.R][index].x),
					(int)(gameData.Falling.Y + minoCorner[gameData.Falling.R][index].y), gameData.Board))
				{
					cornerCount++;

					//AdditinalTableは無理やり追加したものなのでx,yは関係ない
					if (!(gameData.Falling.Type != MinoKind.T ||
						(gameData.Falling.R != ConstData.CORNER_ADDITIONAL_TABLE[gameData.Falling.R][index].x &&
						gameData.Falling.R != ConstData.CORNER_ADDITIONAL_TABLE[gameData.Falling.R][index].y)))
						a++;
				}

			}


			if (cornerCount < 3)
				return null;

			var spinType = "normal";

			if (gameData.Falling.Type == MinoKind.T && a != 2)
				spinType = "mini";

			if (gameData.Falling.LastKick == 4)
				spinType = "normal";


			return spinType;

		}


		/// <summary>
		/// Judge position is in the field and selected position is empty.
		/// </summary>
		/// <param name="x">absolute position</param>
		/// <param name="y">absolute position</param>
		/// <param name="field"></param>
		/// <returns>Is empty</returns>
		internal static bool IsEmptyPos(int x, int y, IReadOnlyList<MinoKind> field)
		{
			if (!(x is >= 0 and < FIELD_WIDTH &&
				  y is >= 0 and < FIELD_HEIGHT))
				return false;

			return field[x + y * 10] == MinoKind.Empty;
		}

		/// <summary>
		/// Check tetromino is in the field and not being overwraped.
		/// remove this
		/// </summary>
		/// <param name="type"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public static bool IsLegalField(int type, int x, double y, int rotation)
		{
			var positions = ConstData.TETRIMINOS_SHAPES[type][rotation];
			var diff = ConstData.TETRIMINO_DIFFS[type];

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
		public static bool IsLegalAtPos(MinoKind type, int x, double y, int rotation, IReadOnlyList<MinoKind> field)
		{
			var positions = ConstData.TETRIMINOS_SHAPES[(int)type][rotation];
			var diff = ConstData.TETRIMINO_DIFFS[(int)type];

			foreach (var position in positions)
			{
				if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
				  position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT &&
					 field[(int)position.x + x - (int)diff.x + (int)(position.y + y - (int)diff.y) * 10] == MinoKind.Empty))
					return false;
			}

			return true;
		}


		internal static void IsBoardEmpty(GameData gameData)
		{
			int emptyLineCount = 0;
			for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
			{
				if (emptyLineCount >= 2)
					break;

				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (gameData.Board[x + y * 10] != MinoKind.Empty)
						return;

				}

				emptyLineCount++;
			}

			//PC
			GarbageSystem.FightLines((int)(ConstValue.Garbage.ALL_CLEAR * gameData.Options.GarbageMultiplier), gameData);
		}

	}
}
