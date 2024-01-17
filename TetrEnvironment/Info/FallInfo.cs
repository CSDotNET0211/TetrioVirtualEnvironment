using TetrEnvironment.Constants;
using TetrioVirtualEnvironment.Constants;
using TetrLoader.Enum;

namespace TetrEnvironment.Info;

public class FallInfo
{
	private Environment _manager;
	private readonly double TERA10 = Math.Pow(10, 13);

	public FallInfo(Environment manager)
	{
		_manager = manager;
	}


	public void FallEvent(int? value, double subFrameDiff = 1)
	{
		if (_manager.GameData.Falling.SafeLock > 0)
			_manager.GameData.Falling.SafeLock--;

		if (_manager.GameData.Falling.Sleep || _manager.GameData.Falling.DeepSleep)
			return;

		var subfFrameGravity = _manager.GameData.Gravity * subFrameDiff;

		if (_manager.GameData.SoftDrop)
		{
			if (_manager.GameData.Handling.SDF == (_manager.GameData.Options.Version >= 15 ? 41 : 21))
				subfFrameGravity = (_manager.GameData.Options.Version >= 15 ? 400 : 20) * subFrameDiff;
			else
			{
				subfFrameGravity *= _manager.GameData.Handling.SDF;
				subfFrameGravity = Math.Max(subfFrameGravity,
					_manager.GameData.Options.Version >= 13 ? 0.05 * _manager.GameData.Handling.SDF : 0.42);
			}
		}

		if (value != null)
			subfFrameGravity = (int)value;

		if (!_manager.GameData.Options.InfiniteMovement &&
		    _manager.GameData.Falling.LockResets >= (int)_manager.GameData.Options.LockResets &&
		    !_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type, _manager.GameData.Falling.X,
			    _manager.GameData.Falling.Y + 1, _manager.GameData.Falling.R))
		{
			subfFrameGravity = 20;
			_manager.GameData.Falling.ForceLock = true;
		}


		if (!_manager.GameData.Options.InfiniteMovement &&
		    _manager.GameData.FallingRotations > (int)_manager.GameData.Options.LockResets + 15)
		{
			subfFrameGravity += 0.5 * subFrameDiff *
			                    (_manager.GameData.FallingRotations -
			                     ((int)_manager.GameData.Options.LockResets + 15));
		}

		double constSubFrameGravity = subfFrameGravity;

		for (; subfFrameGravity > 0;)
		{
			var yPos = Math.Ceiling(_manager.GameData.Falling.Y);
			if (!FallInner(Math.Min(1, subfFrameGravity), constSubFrameGravity))
			{
				if (value != null)
					_manager.GameData.Falling.ForceLock = true;
				Locking(value != null, subFrameDiff);
				break;
			}

			subfFrameGravity -= Math.Min(1, subfFrameGravity);
			if (yPos != Math.Ceiling(_manager.GameData.Falling.Y))
			{
				_manager.GameData.Falling.Last = Falling.LastKind.Fall;
				if (_manager.GameData.SoftDrop)
				{
					//ScoreAdd
				}
			}
		}
	}

	public bool FallInner(double value, double value2)
	{
		var yPos1 = Math.Floor(TERA10 * _manager.GameData.Falling.Y) /
			TERA10 + value;

		if (yPos1 % 1 == 0)
			yPos1 += 0.00001;

		var yPos2 = Math.Floor(TERA10 * _manager.GameData.Falling.Y) / TERA10 + 1;
		if (yPos2 % 1 == 0)
			yPos2 -= 0.00002;

		if (_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type, _manager.GameData.Falling.X, yPos1,
			    _manager.GameData.Falling.R) &&
		    _manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type, _manager.GameData.Falling.X, yPos2,
			    _manager.GameData.Falling.R))
		{
			var highestY = _manager.GameData.Falling.HighestY;
			yPos2 = _manager.GameData.Falling.Y;

			_manager.GameData.Falling.Y = yPos1;
			_manager.GameData.Falling.HighestY = Math.Ceiling(Math.Max(_manager.GameData.Falling.HighestY, yPos1));
			_manager.GameData.Falling.Floored = false;
			if ((int)Math.Ceiling(yPos1) != (int)Math.Ceiling(yPos2))
			{
			}

			if (yPos1 > highestY || _manager.GameData.Options.InfiniteMovement)
			{
				_manager.GameData.Falling.LockResets = 0;
				_manager.GameData.FallingRotations = 0;
			}

			if (value2 >= int.MaxValue)
			{
				//スコア足す
			}

			return true;
		}

		return false;
	}

	public void Locking(bool value = false, double subframe = 1)
	{
		if (_manager.GameData.Options.Version >= 15)
			_manager.GameData.Falling.Locking += subframe;
		else
			_manager.GameData.Falling.Locking ++;

		if (!_manager.GameData.Falling.Floored)
			_manager.GameData.Falling.Floored = true;


		if (_manager.GameData.Falling.Locking > _manager.GameData.Options.LockTime ||
		    _manager.GameData.Falling.ForceLock ||
		    _manager.GameData.Falling.LockResets > _manager.GameData.Options.LockResets &&
		    !_manager.GameData.Options.InfiniteMovement)
			Lock(value);
	}

	public void Lock(bool emptyDrop = false)
	{
		_manager.GameData.Falling.Sleep = true;

		_manager.BoardInfo.PushActiveToStack();

		if (!emptyDrop && _manager.GameData.Handling.SafeLock != 0)
			_manager.GameData.Handling.SafeLock = 7;

		var lines = _manager.LineInfo.ClearLines(); 
		var are = lines != 0 ? _manager.GameData.Options.LineClearAre : _manager.GameData.Options.Are;
		if (_manager.GameData.Options.GarbageEntry == GarbageEntryType.Delayed)
			are = Math.Max(are, _manager.GameData.LastReceivedCount * _manager.GameData.Options.GarbageAre);


		if (are != 0)
			_manager.WaitingFrameInfo.WaitFrames(are, GarbageInfo.WaitingFrameType.Are, null);
		else
			Next(null);
	}

	//empty means first swap,
	//null means normal,
	//the other means swap 
	public void Next(Tetromino.MinoType? newMino)
	{
		if (_manager.GameData.Falling.DeepSleep)
			return;

		_manager.GameData.Falling.Locking = 0;
		_manager.GameData.Falling.ForceLock = false;
		_manager.GameData.Falling.LockResets = 0;
		_manager.GameData.Falling.Floored = false;

		if (newMino == null || newMino == Tetromino.MinoType.Empty)
			_manager.GameData.Falling.Type = _manager.BagInfo.PullFromBag();
		else
			_manager.GameData.Falling.Type = (Tetromino.MinoType)newMino;

		var type = _manager.GameData.Falling.Type;
		var offset = Kickset.ADDITIONAL_OFFSETS ?? Kickset.ADDITIONAL_OFFSETS_EMPTY;
		int spawnRotation;
		if (Kickset.SPAWN_ROTATION == null)
			spawnRotation = 0;
		else
			spawnRotation = Kickset.SPAWN_ROTATION[(int)type];

		_manager.GameData.Falling.Aox = (int)offset[spawnRotation].x;
		_manager.GameData.Falling.Aoy = (int)offset[spawnRotation].y;
		_manager.GameData.Falling.X = _manager.BoardInfo.Width / 2 - 1 + _manager.GameData.Falling.Aox;
		_manager.GameData.Falling.Y = _manager.BoardInfo.VisibleHeight - 2.04 + _manager.GameData.Falling.Aoy;
		_manager.GameData.Falling.HighestY = _manager.BoardInfo.VisibleHeight - 2;
		_manager.GameData.Falling.R = spawnRotation;
		_manager.GameData.Falling.SpinType = Falling.SpinTypeKind.Null;
		_manager.GameData.Falling.Sleep = false;
		_manager.GameData.Falling.DeepSleep = false;
		_manager.GameData.Falling.Last = Falling.LastKind.None;
		_manager.GameData.FallingRotations = 0;
		_manager.GameData.TotalRotations = 0;

		if (newMino != null && !_manager.GameData.Options.InfiniteHold)
			_manager.GameData.HoldLocked = true;
		else
			_manager.GameData.HoldLocked = false;

		if (_manager.GameData.Falling.Clamped && _manager.GameData.Handling.DCD > 0)
		{
			_manager.GameData.LDas = Math.Min(_manager.GameData.LDas,
				_manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD);
			_manager.GameData.LDasIter = _manager.GameData.Handling.ARR;

			_manager.GameData.RDas = Math.Min(_manager.GameData.RDas,
				_manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD);
			_manager.GameData.RDasIter = _manager.GameData.Handling.ARR;
		}

		_manager.GameData.Falling.Clamped = false;

		if (!_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
			    _manager.GameData.Falling.X,
			    _manager.GameData.Falling.Y,
			    _manager.GameData.Falling.R))
		{
			//dead
			//	throw new Exception("dead");
		}

		if (_manager.GameData.Falling.Ihs)
		{
			if (!_manager.GameData.HoldLocked)
				SwapHold();
		}

		if (_manager.GameData.Falling.Irs != 0)
			_manager.HandleInfo.RotatePiece((spawnRotation + _manager.GameData.Falling.Irs) % 14);

		_manager.GameData.Falling.Irs = 0;
		_manager.GameData.Falling.Ihs = false;
	}

	public void SwapHold()
	{
		var type = _manager.GameData.Falling.Type;
		Next(_manager.GameData.Hold);
		_manager.GameData.Hold = type;
	}

	public Falling.SpinTypeKind IsTspin()
	{
		if (_manager.GameData.Options.SpinBonuses == SpinBonusesType.None)
			return Falling.SpinTypeKind.Null;
		if (_manager.GameData.Options.SpinBonuses == SpinBonusesType.Stupid)
		{
			if (!_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
				    _manager.GameData.Falling.X,
				    _manager.GameData.Falling.Y + 1,
				    _manager.GameData.Falling.R))
				return Falling.SpinTypeKind.Normal;
		}

		if (_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
			    _manager.GameData.Falling.X,
			    _manager.GameData.Falling.Y + 1,
			    _manager.GameData.Falling.R))
		{
			return Falling.SpinTypeKind.Null;
		}

		if (_manager.GameData.Falling.Type != Tetromino.MinoType.T &&
		    _manager.GameData.Options.SpinBonuses != SpinBonusesType.Handheld)
		{
			if (_manager.GameData.Options.SpinBonuses == SpinBonusesType.All &&
			    !(_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
				      _manager.GameData.Falling.X - 1,
				      _manager.GameData.Falling.Y,
				      _manager.GameData.Falling.R) ||
			      _manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
				      _manager.GameData.Falling.X + 1,
				      _manager.GameData.Falling.Y,
				      _manager.GameData.Falling.R) ||
			      _manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
				      _manager.GameData.Falling.X,
				      _manager.GameData.Falling.Y + 1,
				      _manager.GameData.Falling.R) ||
			      _manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
				      _manager.GameData.Falling.X,
				      _manager.GameData.Falling.Y - 1,
				      _manager.GameData.Falling.R)
				    ))
			{
				return Falling.SpinTypeKind.Normal;
			}
			else return Falling.SpinTypeKind.Null;
		}

		if (_manager.GameData.Falling.Last != Falling.LastKind.Rotate)
			return Falling.SpinTypeKind.Null;

		int cornerCount = 0;
		int miniCheckCount = 0;

		for (int i = 0; i < 4; i++)
		{
			//		if (CornerTable.TABLE == null)
			//		break;

			var cornerTable = CornerTable.GetTable(_manager.GameData.Falling.Type);
			if (_manager.BoardInfo.IsOccupied(
				    _manager.GameData.Falling.X + (int)cornerTable[_manager.GameData.Falling.R][i].x,
				    _manager.GameData.Falling.Y + (int)cornerTable[_manager.GameData.Falling.R][i].y))
			{
				cornerCount++;
				//NOTE: ここのTの条件怪しいから、確認
				if (!(_manager.GameData.Falling.Type != Tetromino.MinoType.T ||
				      (_manager.GameData.Falling.R != CornerTable.ADDITIONAL_TABLE[_manager.GameData.Falling.R][i].x &&
				       _manager.GameData.Falling.R != CornerTable.ADDITIONAL_TABLE[_manager.GameData.Falling.R][i].y)))
				{
					miniCheckCount++;
				}
			}
		}

		if (cornerCount < 3)
			return Falling.SpinTypeKind.Null;

		var spinType = Falling.SpinTypeKind.Normal;
		if (_manager.GameData.Falling.Type == Tetromino.MinoType.T && miniCheckCount != 2)
			spinType = Falling.SpinTypeKind.Mini;

		if (_manager.GameData.Falling.LastKick == 4)
			spinType = Falling.SpinTypeKind.Normal;

		return spinType;
	}
}