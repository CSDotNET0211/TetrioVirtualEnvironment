using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TetrioVirtualEnvironment.Environment;
using TetrioVirtualEnvironment;
using TetrioVirtualEnvironment.Constants;

namespace TetrioVirtualEnvironment
{
	public partial class Environment
	{


		private  void RotatePiece(int rotation)
		{
			var currentMinoRotation = GameData.Falling.R;
			var currentMinoNewRotation = currentMinoRotation*10 + rotation;
			var oValue = 0;
			Falling.LastRotationKind lastRotation =Falling.LastRotationKind.None;

			if (rotation < currentMinoRotation)
			{
				oValue = 1;
				lastRotation =Falling.LastRotationKind.Right;
			}
			else
			{
				oValue = -1;
				lastRotation = Falling.LastRotationKind.Left;
			}

			if (rotation == 0 && currentMinoRotation == 3)
			{
				oValue = 1;
				lastRotation =Falling.LastRotationKind.Right;
			}

			if (rotation == 3 && currentMinoRotation == 3)
			{
				oValue = -1;
				lastRotation =Falling.LastRotationKind.Left;
			}

			if (rotation == 2 && currentMinoRotation == 0)
				lastRotation = Falling.LastRotationKind.Vertical;
			if (rotation == 0 && currentMinoRotation == 2)
				lastRotation = Falling.LastRotationKind.Vertical;
			if (rotation == 3 && currentMinoRotation == 1)
				lastRotation = Falling.LastRotationKind.Horizontal;
			if (rotation == 1 && currentMinoRotation == 3)
				lastRotation = Falling.LastRotationKind.Horizontal;

			if (IsLegalAtPos(GameData.Falling.Type,
				GameData.Falling.X - GameData.Falling.Aox,
				GameData.Falling.Y - GameData.Falling.Aoy, rotation,
				GameData.Board))
			{
				GameData.Falling.X -= GameData.Falling.Aox;
				GameData.Falling.Y -= GameData.Falling.Aoy;
				GameData.Falling.Aox = 0;
				GameData.Falling.Aoy = 0;
				GameData.Falling.R = rotation;
				GameData.Falling.Last = Falling.LastKind.Rotate;
				GameData.Falling.LastRotation = lastRotation;
				GameData.Falling.LastKick = 0;
				GameData.Falling.SpinType = IsTspin();
				GameData.FallingRotations++;
				GameData.TotalRotations++;

				if (GameData.Falling.Clamped && GameData.Handling.DCD > 0)
				{
					GameData.LDas = Math.Min(GameData.LDas, GameData.Handling.DAS - GameData.Handling.DCD);
					GameData.LDasIter = GameData.Handling.ARR;
					GameData.RDas = Math.Min(GameData.RDas, GameData.Handling.DAS - GameData.Handling.DCD);
					GameData.RDasIter = GameData.Handling.ARR;
				}

				if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
					GameData.Falling.Locking = 0;


				return;
			}

			if (GameData.Falling.Type == Tetrimino.MinoType.O)
				return;

			var kicktable = KickTable.SRSPLUS[currentMinoNewRotation];

			if (GameData.Falling.Type == Tetrimino.MinoType.I)
				kicktable = KickTable.SRSPLUS_I[currentMinoNewRotation];

			for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
			{
				var kicktableTest = kicktable[kicktableIndex];
				var newMinoYPos = (int)(GameData.Falling.Y) + 0.1 +
					kicktableTest.y - GameData.Falling.Aoy;


				if (!GameData.Options.InfiniteMovement &&
					GameData.TotalRotations > (int)GameData.Options.LockResets + 15)
				{
					newMinoYPos = GameData.Falling.Y + kicktableTest.y - GameData.Falling.Aoy;
				}

				if (IsLegalAtPos(GameData.Falling.Type,
					GameData.Falling.X + (int)kicktableTest.x - GameData.Falling.Aox,
					newMinoYPos, rotation, GameData.Board))
				{

					GameData.Falling.X += (int)kicktableTest.x - GameData.Falling.Aox;
					GameData.Falling.Y = newMinoYPos;
					GameData.Falling.Aox = 0;
					GameData.Falling.Aoy = 0;
					GameData.Falling.R = rotation;
					GameData.Falling.SpinType = IsTspin();
					GameData.Falling.LastKick = kicktableIndex + 1;
					GameData.Falling.Last = Falling.LastKind.Rotate;
					GameData.FallingRotations++;
					GameData.TotalRotations++;

					if (GameData.Falling.Clamped && GameData.Handling.DCD > 0)
					{
						GameData.LDas = Math.Min(GameData.LDas, GameData.Handling.DAS - GameData.Handling.DCD);
						GameData.LDasIter = GameData.Handling.ARR;
						GameData.RDas = Math.Min(GameData.RDas, GameData.Handling.DAS - GameData.Handling.DCD);
						GameData.RDasIter = GameData.Handling.ARR;
					}

					if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
						GameData.Falling.Locking = 0;


					return;
				}
			}
		}

	}
}
