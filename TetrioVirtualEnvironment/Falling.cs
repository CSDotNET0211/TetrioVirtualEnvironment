using System.Diagnostics;
using TetrioVirtualEnvironment.Constants;
using Environment = TetrioVirtualEnvironment.Env.Environment;

namespace TetrioVirtualEnvironment
{
	public class Falling
	{
		public enum SpinTypeKind
		{
			Null,
			Normal,
			Mini
		}

		public enum LastKind
		{
			None,
			Rotate,
			Move,
			Fall
		}

		public enum LastRotationKind
		{
			None,
			Right,
			Left,
			Vertical,
			Horizontal,
		}


		public Falling(Env.Environment environment)
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
			Type = Tetrimino.MinoType.Empty;
			HighestY = 14;
			Last = LastKind.None;
			LastKick = 0;
			LastRotation = LastRotationKind.None;
			Irs = 0;
			Ihs = 0;
			Aox = 0;
			Aoy = 0;

			_environment = environment;
		}

		
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newType"></param>
		/// <param name="isHold"></param>
		/// <param name="environmentMode"></param>
		public void Init(Tetrimino.MinoType? newType, bool isHold, Env.Environment.NextGenerateKind environmentMode)
		{
			Locking = 0;
			ForceLock = false;
			LockResets = 0;
			Floored = false;

			if (newType == null || newType == Tetrimino.MinoType.Empty)
			{
				if (environmentMode == Env.Environment.NextGenerateKind.Array)
				{
					if (_environment.GameData.Next.Count != 0)
					{
						Type = _environment.GameData.Next.Dequeue();
					}
					else
					{
						Type = Tetrimino.MinoType.Empty;
						return;
					}
				}
				else
					Type = _environment.RefreshNext(_environment.GameData.Next, false);
			}
			else
				Type = (Tetrimino.MinoType)newType;

			Aox = 0;
			Aoy = 0;
			X = 4;
			Y = 20 - 2.04 + 1;
			HighestY = 20 - 2;
			R = 0;

			SpinType = SpinTypeKind.Null;
			Sleep = false;
			Last = LastKind.None;
			_environment.GameData.FallingRotations = 0;
			_environment.GameData.TotalRotations = 0;

			/* in tetrio.js, normal tetromino creating has undefined argument, first hold is unll so easy to judge.
			 * t.holdlocked = void 0 !== s
			 * but in this code, null means normal tetromino creating. to solve this problem, I approached to use 2nd argument is hold or not.
			 */


			if (isHold)
				_environment.GameData.HoldLocked = true;
			else
				_environment.GameData.HoldLocked = newType != null;

			if (Clamped && _environment.GameData.Handling.DCD > 0)
			{
				_environment.GameData.LDas = Math.Min(_environment.GameData.LDas,
					_environment.GameData.Handling.DAS - _environment.GameData.Handling.DCD);
				_environment.GameData.LDasIter = _environment.GameData.Handling.ARR;
				_environment.GameData.RDas = Math.Min(_environment.GameData.RDas,
					_environment.GameData.Handling.DAS - _environment.GameData.Handling.DCD);
				_environment.GameData.RDasIter = _environment.GameData.Handling.ARR;
			}

			Clamped = false;

			if (!Environment. IsLegalAtPos(Type, X, Y, R, _environment.GameData.Board) ||
			    (!_environment.GameData.Options.NoLockout && HighestYisOver20()))
			{
				_environment.IsDead = true;
				Debug.WriteLine("dead");
			}


			_environment.CallOnPieceCreated();
		}

		public bool HighestYisOver20()
		{
			var positions = Tetrimino.SHAPES[(int)Type][R];
			var diff = Tetrimino.DIFFS[(int)Type];

			foreach (var position in positions)
			{
				if (position.y + Y - diff.y > 20)
					return true;
			}

			return false;
		}

		private readonly Environment _environment;
		public void ForceMoveTetriminoPos(MinoPosition pos)
		{
			if (pos.type != Tetrimino.MinoType.Empty)
				Type = pos.type;

			if (pos.x != -1)
				X = pos.x;

			if (pos.y != -1)
				Y = pos.y;

			if (pos.r != -1)
				R = pos.r;
		}

		public Tetrimino.MinoType Type { get; private set; }
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

		public LastKind Last { get; internal set; }

		public LastRotationKind LastRotation { get; internal set; }
		public SpinTypeKind SpinType { get; internal set; }
		public int LastKick { get; internal set; }
		public bool Clamped { get; internal set; }
		public int LockResets { get; internal set; }
		public double Locking { get; internal set; }
		public bool Floored { get; internal set; }
		public double HighestY { get; internal set; }
	}
}