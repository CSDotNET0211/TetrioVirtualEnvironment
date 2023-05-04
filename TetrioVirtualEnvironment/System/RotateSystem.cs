using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TetrioVirtualEnvironment.Environment;
using TetrioVirtualEnvironment;

namespace TetrioVirtualEnvironment.System
{
	public class RotateSystem
	{


		internal static void RotatePiece(int rotation, GameData gameData)
		{
			var nowminoRotation = gameData.Falling.R;
			var nowminoNewminoRotation = nowminoRotation.ToString() + rotation.ToString();
			var oValue = 0;
			var lastRotation = "none";

			if (rotation < nowminoRotation)
			{
				oValue = 1;
				lastRotation = "right";
			}
			else
			{
				oValue = -1;
				lastRotation = "left";
			}

			if (rotation == 0 && nowminoRotation == 3)
			{
				oValue = 1;
				lastRotation = "right";
			}

			if (rotation == 3 && nowminoRotation == 3)
			{
				oValue = -1;
				lastRotation = "left";
			}

			if (rotation == 2 && nowminoRotation == 0)
				lastRotation = "vertical";
			if (rotation == 0 && nowminoRotation == 2)
				lastRotation = "vertical";
			if (rotation == 3 && nowminoRotation == 1)
				lastRotation = "horizontal";
			if (rotation == 1 && nowminoRotation == 3)
				lastRotation = "horizontal";

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
				gameData.Falling.Last = "rotate";
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

			if (gameData.Falling.Type == (int)MinoKind.O)
				return;

			var kicktable = Environment.ConstData.KICKSET_SRSPLUS[nowminoNewminoRotation];

			if (gameData.Falling.Type == (int)MinoKind.I)
				kicktable = Environment.ConstData.KICKSET_SRSPLUSI[nowminoNewminoRotation];

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
					gameData.Falling.Last = "rotate";
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
