using System.Diagnostics;
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

		public Falling(int type, int x, int y, int r)
		{
			Type = type;
			X = x;
			Y = y;
			R = r;

		}



		public void Init(int? newtype, bool isHold, EnvironmentModeEnum environmentMode)
		{
			Locking = 0;
			ForceLock = false;
			LockResets = 0;
			Floored = false;

			if (newtype == null)
			{
				if (environmentMode == EnvironmentModeEnum.Limited)
				{
					if (EnvironmentInstance._gameData.Next.Count != 0)
					{
						Type = EnvironmentInstance._gameData.Next[0];
						EnvironmentInstance._gameData.Next.RemoveAt(0);
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

			if (!IsLegalAtPos(Type, X, Y, R, GameDataInstance.Field))
			{
				Debug.WriteLine("dead");
			}


			EnvironmentInstance.CallOnPieceCreated();



		}

		public Environment EnvironmentInstance { get; }
		public GameData GameDataInstance { get; }
		public int Type { get; set; }
		public int X { get; set; }
		public int Aox { get; set; }
		public double Y { get; set; }
		public int Aoy { get; set; }
		public int R { get; set; }
		public int Irs { get; }
		public int Ihs { get; }
		public int SafeLock { get; set; }
		public bool ForceLock { get; set; }
		public bool Sleep { get; set; }
		public bool DeepSleep { get; }
		public string Last { get; set; }
		public string LastRotation { get; set; }
		public string? SpinType { get; set; }
		public int LastKick { get; set; }
		public bool Clamped { get; set; }
		public int LockResets { get; set; }
		public double Locking { get; set; }
		public bool Floored { get; set; }
		public double HighestY { get; set; }
	}

	public static class FallingEx
	{
		public static Falling Clone(this Falling falling)
		{
			var value = new Falling(falling.Type, falling.X, (int)falling.Y, falling.R);
			return value;
		}
	}
}
