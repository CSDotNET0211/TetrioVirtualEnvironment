using TetrEnvironment.Constants;

namespace TetrEnvironment;

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

	public bool Sleep { get; internal set; }
	public bool DeepSleep { get; internal set; }
	public bool Hibernated { get; internal set; }
	public double Locking { get; internal set; }
	public int LockResets { get; internal set; }
	public bool ForceLock { get; internal set; }
	public bool Floored { get; internal set; }
	public bool Clamped { get; internal set; }
	public int SafeLock { get; internal set; }
	public int X { get; internal set; }
	public double Y { get; internal set; }
	public int R { get; internal set; }
	public Tetromino.MinoType Type { get; internal set; }
	public double HighestY { get; internal set; }
	public LastKind Last { get; internal set; }
	public int LastKick { get; internal set; }
	public LastRotationKind LastRotation { get; internal set; }
	public int Irs { get; internal set; }
	public bool Ihs { get; internal set; }
	public int Aox { get; internal set; }
	public int Aoy { get; internal set; }
	public int Keys;
	public SpinTypeKind SpinType { get; internal set; }

	public Falling()
	{
		Sleep = true;
		DeepSleep = true;
		Hibernated = false;
		Locking = 0;
		LockResets = 0;
		ForceLock = false;
		Floored = false;
		Clamped = false;
		SafeLock = 0;
		X = 4;
		Y = 14;
		R = 0;
		Type = Tetromino.MinoType.I;
		HighestY = 14;
		Last = LastKind.None;
		LastKick = 0;
		LastRotation = LastRotationKind.None;
		Irs = 0;
		Ihs = false;
		Aox = 0;
		Aoy = 0;
		Keys = 0;
	}
}