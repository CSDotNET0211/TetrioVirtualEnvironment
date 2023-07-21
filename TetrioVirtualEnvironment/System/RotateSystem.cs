using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TetrioVirtualEnvironment.Environment;
using TetrioVirtualEnvironment;
using TetrioVirtualEnvironment.Constants;

namespace TetrioVirtualEnvironment.System
{
	public class RotateSystem
	{


		internal static void RotatePiece(int rotation, GameData gameData)
		{
			var currentMinoRotation = gameData.Falling.R;
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

			if (JudgeSystem.IsLegalAtPos(gameData.Falling.Type,
				gameData.Falling.X - gameData.Falling.Aox,
				gameData.Falling.Y - gameData.Falling.Aoy, rotation,
				gameData.Board))
			{
				gameData.Falling.X -= gameData.Falling.Aox;
				gameData.Falling.Y -= gameData.Falling.Aoy;
				gameData.Falling.Aox = 0;
				gameData.Falling.Aoy = 0;
				gameData.Falling.R = rotation;
				gameData.Falling.Last = Falling.LastKind.Rotate;
				gameData.Falling.LastRotation = lastRotation;
				gameData.Falling.LastKick = 0;
				gameData.Falling.SpinType = JudgeSystem.IsTspin(gameData);
				gameData.FallingRotations++;
				gameData.TotalRotations++;

				if (gameData.Falling.Clamped && gameData.Handling.DCD > 0)
				{
					gameData.LDas = Math.Min(gameData.LDas, gameData.Handling.DAS - gameData.Handling.DCD);
					gameData.LDasIter = gameData.Handling.ARR;
					gameData.RDas = Math.Min(gameData.RDas, gameData.Handling.DAS - gameData.Handling.DCD);
					gameData.RDasIter = gameData.Handling.ARR;
				}

				if (++gameData.Falling.LockResets < 15 || gameData.Options.InfiniteMovement)
					gameData.Falling.Locking = 0;


				//落下ミノ更新フラグ true
				return;
			}

			if (gameData.Falling.Type == Tetrimino.MinoType.O)
				return;

			var kicktable = KickTable.SRSPLUS[currentMinoNewRotation];

			if (gameData.Falling.Type == Tetrimino.MinoType.I)
				kicktable = KickTable.SRSPLUS_I[currentMinoNewRotation];

			for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
			{
				var kicktableTest = kicktable[kicktableIndex];
				var newMinoYPos = (int)(gameData.Falling.Y) + 0.1 +
					kicktableTest.y - gameData.Falling.Aoy;


				if (!gameData.Options.InfiniteMovement &&
					gameData.TotalRotations > (int)gameData.Options.LockResets + 15)
				{
					newMinoYPos = gameData.Falling.Y + kicktableTest.y - gameData.Falling.Aoy;
				}

				if (JudgeSystem.IsLegalAtPos(gameData.Falling.Type,
					gameData.Falling.X + (int)kicktableTest.x - gameData.Falling.Aox,
					newMinoYPos, rotation, gameData.Board))
				{

					gameData.Falling.X += (int)kicktableTest.x - gameData.Falling.Aox;
					gameData.Falling.Y = newMinoYPos;
					gameData.Falling.Aox = 0;
					gameData.Falling.Aoy = 0;
					gameData.Falling.R = rotation;
					gameData.Falling.SpinType = JudgeSystem.IsTspin(gameData);
					gameData.Falling.LastKick = kicktableIndex + 1;
					gameData.Falling.Last = Falling.LastKind.Rotate;
					gameData.FallingRotations++;
					gameData.TotalRotations++;

					if (gameData.Falling.Clamped && gameData.Handling.DCD > 0)
					{
						gameData.LDas = Math.Min(gameData.LDas, gameData.Handling.DAS - gameData.Handling.DCD);
						gameData.LDasIter = gameData.Handling.ARR;
						gameData.RDas = Math.Min(gameData.RDas, gameData.Handling.DAS - gameData.Handling.DCD);
						gameData.RDasIter = gameData.Handling.ARR;
					}

					if (++gameData.Falling.LockResets < 15 || gameData.Options.InfiniteMovement)
						gameData.Falling.Locking = 0;


					return;
				}
			}
		}

	}
}
