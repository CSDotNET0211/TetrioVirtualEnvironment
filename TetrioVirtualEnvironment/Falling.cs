using System.Diagnostics;
using TetrioVirtualEnvironment;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment
{
	public class Falling
	{
		public Falling(Environment environment, GameData gameData)
		{
			Sleep = true;
			DeepSleep = false;
			Locking = 0;
			LockResets = 0;
			ForceLock = false;
			Floored = false;
			Clamped = false;
			SafeLock = 0;
			X = 4;
			Y = 14;
			R = 0;
			Type = -1;
			HighestY = 14;
			Last = null;
			LastKick = 0;
			LastRotation = "none";
			Irs = 0;
			Ihs = 0;
			Aox = 0;
			Aoy = 0;

			EnvironmentInstance = environment;
			GameDataInstance = gameData;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="newtype"></param>
		/// <param name="isHold"></param>
		/// <param name="environmentMode"></param>
		public void Init(int? newtype, bool isHold, NextGenerateKind environmentMode)
		{
			Locking = 0;
			ForceLock = false;
			LockResets = 0;
			Floored = false;

			if (newtype == null)
			{
				if (environmentMode == NextGenerateKind.Array)
				{
					if (EnvironmentInstance.GameData.Next.Count != 0)
					{
						Type = EnvironmentInstance.GameData.Next[0];
						EnvironmentInstance.GameData.Next.RemoveAt(0);
					}
					else
					{
						Type = (int)MinoKind.Empty;
						return;
					}


				}
				else
					Type = EnvironmentInstance.RefreshNext(GameDataInstance.Next, false);
			}
			else
				Type = (int)newtype;

			Aox = 0;
			Aoy = 0;
			X = 4;
			Y = 20 - 2.04 + 1;
			HighestY = 20 - 2;
			R = 0;

			SpinType = null;
			Sleep = false;
			Last = null;
			GameDataInstance.FallingRotations = 0;
			GameDataInstance.TotalRotations = 0;

			/* in tetrio.js, normal tetromino creating has undefined argument, first hold is unll so easy to judge.
			 * t.holdlocked = void 0 !== s
			 * but in this code, null means normal tetromino creating. to solve this problem, I approached to use 2nd argument is hold or not.
			 */


			if (isHold)
				GameDataInstance.HoldLocked = true;
			else
				GameDataInstance.HoldLocked = newtype != null;

			if (Clamped && GameDataInstance.Handling.DCD > 0)
			{
				GameDataInstance.LDas = Math.Min(GameDataInstance.LDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
				GameDataInstance.LDasIter = GameDataInstance.Handling.ARR;
				GameDataInstance.RDas = Math.Min(GameDataInstance.RDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
				GameDataInstance.RDasIter = GameDataInstance.Handling.ARR;
			}

			Clamped = false;

			if (!IsLegalAtPos(Type, X, Y, R, GameDataInstance.Board))
			{
				EnvironmentInstance.IsDead = true;
				Debug.WriteLine("dead");
			}


			EnvironmentInstance.CallOnPieceCreated();



		}

		public void ForceMoveTetriminoPos(MinoPosition pos)
		{
			if (pos.type != -1)
				Type = pos.type;

			if (pos.x != -1)
				X = pos.x;

			if (pos.y != -1)
				Y = pos.y;

			if (pos.r != -1)
				R = pos.r;
		}

		//DOTO:アクセスすべてできないようにするべき、ゲッターかなんか用意
		public Environment EnvironmentInstance { get; }
		public GameData GameDataInstance { get; }
		public int Type { get; private set; }
		public int X { get; internal set; }
		public int Aox { get; internal set; }
		public double Y { get; internal set; }
		public int Aoy { get; internal set; }
		public int R { get; internal set; }
		public int Irs { get; }
		public int Ihs { get; }
		public int SafeLock { get; internal set; }
		public bool ForceLock { get; internal set; }
		public bool Sleep { get; internal set; }
		public bool DeepSleep { get; }
		/// <summary>
		/// Last move
		/// </summary>
		public string Last { get; internal set; }
		public string LastRotation { get; internal set; }
		public string? SpinType { get; internal set; }
		public int LastKick { get; internal set; }
		public bool Clamped { get; internal set; }
		public int LockResets { get; internal set; }
		public double Locking { get; internal set; }
		public bool Floored { get; internal set; }
		public double HighestY { get; internal set; }
	}


}