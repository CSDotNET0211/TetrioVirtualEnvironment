using TetrEnvironment.Constants;
using TetrLoader.Enum;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment.Info;

public class HandleInfo
{
	private Environment _manager;

	public HandleInfo(Environment manager)
	{
		_manager = manager;
	}

	public void KeyDown(EventKeyInputData @event)	
	{
		if ((int)@event.key < 8)
			_manager.PressingKeys[(int)@event.key] = true;
		
		if (@event.subframe > _manager.GameData.SubFrame)
		{
			_manager.HandleInfo.ProcessShift(false, @event.subframe - _manager.GameData.SubFrame);
			_manager.FallInfo.FallEvent(null, @event.subframe - _manager.GameData.SubFrame);
			_manager.GameData.SubFrame = @event.subframe;
		}

		if (@event.key == KeyType.MoveLeft)
		{
			_manager.GameData.LShift = true;
			_manager.GameData.LastShift = -1;
			_manager.GameData.LDas =
				@event.hoisted ? _manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD : 0;
			if (_manager.GameData.Options.Version >= 12)
				_manager.GameData.LDasIter = _manager.GameData.Handling.ARR;

			ProcessLShift(true, _manager.GameData.Options.Version >= 15 ? 0 : 1);
			return;
		}

		if (@event.key == KeyType.MoveRight)
		{
			_manager.GameData.RShift = true;
			_manager.GameData.LastShift = 1;
			_manager.GameData.RDas =
				@event.hoisted ? _manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD : 0;
			if (_manager.GameData.Options.Version >= 12)
				_manager.GameData.RDasIter = _manager.GameData.Handling.ARR;

			ProcessRShift(true, _manager.GameData.Options.Version >= 15 ? 0 : 1);
			return;
		}

		if (@event.key == KeyType.SoftDrop)
		{
			_manager.GameData.SoftDrop = true;
			return;
		}

		if (!_manager.GameData.Falling.DeepSleep)
		{
			if (_manager.GameData.Falling.Sleep)
			{
				if (@event.key == KeyType.RotateCCW)
				{
					var e = _manager.GameData.Falling.Irs - 1;
					if (e < 0)
						e = 3;
					_manager.GameData.Falling.Irs = e;
				}

				if (@event.key == KeyType.RotateCW)
				{
					var e = _manager.GameData.Falling.Irs + 1;
					if (e > 3)
						e = 0;
					_manager.GameData.Falling.Irs = e;
				}

				if (@event.key == KeyType.Rotate180)
				{
					var e = _manager.GameData.Falling.Irs + 2;
					if (e > 3)
						e -= 4;
					_manager.GameData.Falling.Irs = e;
				}

				if (@event.key == KeyType.Hold)
				{
					_manager.GameData.Falling.Ihs = true;
				}
			}
			else
			{
				if (@event.key == KeyType.RotateCCW)
				{
					var e = _manager.GameData.Falling.R - 1;
					if (e < 0)
						e = 3;
					RotatePiece(e);
				}

				if (@event.key == KeyType.RotateCW)
				{
					var e = _manager.GameData.Falling.R + 1;
					if (e > 3)
						e = 0;
					RotatePiece(e);
				}

				if (@event.key == KeyType.Rotate180 && _manager.GameData.Options.Allow180)
				{
					var e = _manager.GameData.Falling.R + 2;
					if (e > 3)
						e -= 4;
					RotatePiece(e);
				}

				if (@event.key == KeyType.HardDrop && _manager.GameData.Options.AllowHardDrop &&
				    _manager.GameData.Falling.SafeLock == 0)
				{
					_manager.FallInfo.FallEvent(int.MaxValue, 1);
				}

				if (@event.key == KeyType.Hold)
				{
					if (!_manager.GameData.HoldLocked)
					{
						if ((_manager.GameData.Options.DisplayHold == null ||
						     (bool)_manager.GameData.Options.DisplayHold))
							_manager.FallInfo.SwapHold();
					}
				}
			}
		}
	}

	public void KeyUp(EventKeyInputData @event)
	{
		if ((int)@event.key < 8)
			_manager.PressingKeys[(int)@event.key] = false;
		
		if (@event.subframe > _manager.GameData.SubFrame)
		{
			_manager.HandleInfo.ProcessShift(false, @event.subframe - _manager.GameData.SubFrame);
			_manager.FallInfo.FallEvent(null, @event.subframe - _manager.GameData.SubFrame);
			_manager.GameData.SubFrame = @event.subframe;
		}

		if (@event.key == KeyType.MoveLeft)
		{
			_manager.GameData.LShift = false;
			_manager.GameData.LDas = 0;

			if (_manager.GameData.Handling.Cancel)
			{
				_manager.GameData.RDas = 0;
				_manager.GameData.RDasIter = _manager.GameData.Handling.ARR;
			}

			return;
		}

		if (@event.key == KeyType.MoveRight)
		{
			_manager.GameData.RShift = false;
			_manager.GameData.RDas = 0;

			if (_manager.GameData.Handling.Cancel)
			{
				_manager.GameData.LDas = 0;
				_manager.GameData.LDasIter = _manager.GameData.Handling.ARR;
			}

			return;
		}

		if (@event.key == KeyType.SoftDrop)
			_manager.GameData.SoftDrop = false;
	}

	internal void RotatePiece(int newRotation)
	{
		var currentRotation = _manager.GameData.Falling.R;
		var currentNewRotation = currentRotation * 10 + newRotation;

		//not used
		var corner = 0;
		Falling.LastRotationKind lastRotation = Falling.LastRotationKind.None;

		if (newRotation < currentRotation)
		{
			corner = 1;
			lastRotation = Falling.LastRotationKind.Right;
		}
		else
		{
			corner = -1;
			lastRotation = Falling.LastRotationKind.Left;
		}

		if (newRotation == 0 && currentRotation == 3)
		{
			corner = 1;
			lastRotation = Falling.LastRotationKind.Right;
		}

		if (newRotation == 3 && currentRotation == 3)
		{
			corner = -1;
			lastRotation = Falling.LastRotationKind.Left;
		}

		if (newRotation == 2 && currentRotation == 0)
			lastRotation = Falling.LastRotationKind.Vertical;
		if (newRotation == 0 && currentRotation == 2)
			lastRotation = Falling.LastRotationKind.Vertical;
		if (newRotation == 3 && currentRotation == 1)
			lastRotation = Falling.LastRotationKind.Horizontal;
		if (newRotation == 1 && currentRotation == 3)
			lastRotation = Falling.LastRotationKind.Horizontal;

		if (_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
			    _manager.GameData.Falling.X - _manager.GameData.Falling.Aox,
			    _manager.GameData.Falling.Y - _manager.GameData.Falling.Aoy, newRotation))
		{
			_manager.GameData.Falling.X -= _manager.GameData.Falling.Aox;
			_manager.GameData.Falling.Y -= _manager.GameData.Falling.Aoy;
			_manager.GameData.Falling.Aox = 0;
			_manager.GameData.Falling.Aoy = 0;
			_manager.GameData.Falling.R = newRotation;
			_manager.GameData.Falling.Last = Falling.LastKind.Rotate;
			_manager.GameData.Falling.LastRotation = lastRotation;
			_manager.GameData.Falling.LastKick = 0;
			_manager.GameData.Falling.SpinType = _manager.FallInfo.IsTspin();
			_manager.GameData.FallingRotations++;
			_manager.GameData.TotalRotations++;

			if (_manager.GameData.Falling.Clamped && _manager.GameData.Handling.DCD > 0)
			{
				_manager.GameData.LDas = Math.Min(_manager.GameData.LDas,
					_manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD);
				_manager.GameData.LDasIter = _manager.GameData.Handling.ARR;
				_manager.GameData.RDas = Math.Min(_manager.GameData.RDas,
					_manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD);
				_manager.GameData.RDasIter = _manager.GameData.Handling.ARR;
			}

			if (++_manager.GameData.Falling.LockResets < 15 || _manager.GameData.Options.InfiniteMovement)
				_manager.GameData.Falling.Locking = 0;


			return;
		}

		if (_manager.GameData.Falling.Type == Constants.Tetromino.MinoType.O)
			return;
	
		var kicktable = Kickset.SRSPLUS[currentNewRotation];

		if (_manager.GameData.Falling.Type == Constants.Tetromino.MinoType.I)
			kicktable = Kickset.SRSPLUS_I[currentNewRotation];

		for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
		{
			var kicktableTest = kicktable[kicktableIndex];
			var newMinoYPos = (int)(_manager.GameData.Falling.Y) + 0.1 +
				kicktableTest.y - _manager.GameData.Falling.Aoy;


			if (!_manager.GameData.Options.InfiniteMovement &&
			    _manager.GameData.TotalRotations > (int)_manager.GameData.Options.LockResets + 15)
			{
				newMinoYPos = _manager.GameData.Falling.Y + kicktableTest.y - _manager.GameData.Falling.Aoy;
			}

			if (_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type,
				    _manager.GameData.Falling.X + (int)kicktableTest.x - _manager.GameData.Falling.Aox,
				    newMinoYPos, newRotation))
			{
				_manager.GameData.Falling.X += (int)kicktableTest.x - _manager.GameData.Falling.Aox;
				_manager.GameData.Falling.Y = newMinoYPos;
				_manager.GameData.Falling.Aox = 0;
				_manager.GameData.Falling.Aoy = 0;
				_manager.GameData.Falling.R = newRotation;
				_manager.GameData.Falling.SpinType = _manager.FallInfo.IsTspin();
				_manager.GameData.Falling.LastKick = kicktableIndex + 1;
				_manager.GameData.Falling.Last = Falling.LastKind.Rotate;
				_manager.GameData.FallingRotations++;
				_manager.GameData.TotalRotations++;

				if (_manager.GameData.Falling.Clamped && _manager.GameData.Handling.DCD > 0)
				{
					_manager.GameData.LDas = Math.Min(_manager.GameData.LDas,
						_manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD);
					_manager.GameData.LDasIter = _manager.GameData.Handling.ARR;
					_manager.GameData.RDas = Math.Min(_manager.GameData.RDas,
						_manager.GameData.Handling.DAS - _manager.GameData.Handling.DCD);
					_manager.GameData.RDasIter = _manager.GameData.Handling.ARR;
				}

				if (++_manager.GameData.Falling.LockResets < 15 || _manager.GameData.Options.InfiniteMovement)
					_manager.GameData.Falling.Locking = 0;


				return;
			}
		}
	}

	private void ProcessLShift(bool value, double subFrameDiff = 1)
	{
		if (!_manager.GameData.LShift || _manager.GameData.RShift && _manager.GameData.LastShift != -1)
			return;

		var subFrameDiffForDas = subFrameDiff;
		var dasDiff = Math.Max(0, _manager.GameData.Handling.DAS - _manager.GameData.LDas);

		_manager.GameData.LDas += value ? 0 : subFrameDiff;
		if (_manager.GameData.LDas < _manager.GameData.Handling.DAS && !value)
			return;

		subFrameDiffForDas = Math.Max(0, subFrameDiffForDas - dasDiff);
		if (_manager.GameData.Falling.Sleep || _manager.GameData.Falling.DeepSleep)
			return;

		var moveLoopCount = 1;
		if (!value)
		{
			_manager.GameData.LDasIter +=
				_manager.GameData.Options.Version >= 15 ? subFrameDiffForDas : subFrameDiff;

			if (_manager.GameData.LDasIter < _manager.GameData.Handling.ARR)
				return;

			moveLoopCount = _manager.GameData.Handling.ARR == 0
				? 10
				: (int)(_manager.GameData.LDasIter / _manager.GameData.Handling.ARR);

			_manager.GameData.LDasIter -= _manager.GameData.Handling.ARR * moveLoopCount;
		}

		for (var moveIndex = 0; moveIndex < moveLoopCount; moveIndex++)
		{
			if (_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type, _manager.GameData.Falling.X - 1,
				    _manager.GameData.Falling.Y,
				    _manager.GameData.Falling.R))
			{
				_manager.GameData.Falling.X--;
				_manager.GameData.Falling.Last = Falling.LastKind.Move;
				_manager.GameData.Falling.Clamped = false;

				if (++_manager.GameData.Falling.LockResets < 15 || _manager.GameData.Options.InfiniteMovement)
					_manager.GameData.Falling.Locking = 0;
			}
			else
			{
				_manager.GameData.Falling.Clamped = true;
			}
		}
	}

	private void ProcessRShift(bool value, double subFrameDiff = 1)
	{
		if (!_manager.GameData.RShift || _manager.GameData.LShift && _manager.GameData.LastShift != 1)
			return;

		var subFrameDiffForDas = subFrameDiff;
		var dasDiff = Math.Max(0, _manager.GameData.Handling.DAS - _manager.GameData.RDas);

		_manager.GameData.RDas += value ? 0 : subFrameDiff;

		if (_manager.GameData.RDas < _manager.GameData.Handling.DAS && !value)
			return;

		subFrameDiffForDas = Math.Max(0, subFrameDiffForDas - dasDiff);
		if (_manager.GameData.Falling.Sleep || _manager.GameData.Falling.DeepSleep)
			return;

		var moveLoopCount = 1;
		if (!value)
		{
			_manager.GameData.RDasIter +=
				_manager.GameData.Options.Version >= 15 ? subFrameDiffForDas : subFrameDiff;

			if (_manager.GameData.RDasIter < _manager.GameData.Handling.ARR)
				return;

			moveLoopCount = _manager.GameData.Handling.ARR == 0
				? 10
				: (int)(_manager.GameData.RDasIter / _manager.GameData.Handling.ARR);

			_manager.GameData.RDasIter -= _manager.GameData.Handling.ARR * moveLoopCount;
		}

		for (var moveIndex = 0; moveIndex < moveLoopCount; moveIndex++)
		{
			if (_manager.BoardInfo.IsLegalAtPos(_manager.GameData.Falling.Type, _manager.GameData.Falling.X + 1,
				    _manager.GameData.Falling.Y,
				    _manager.GameData.Falling.R))
			{
				_manager.GameData.Falling.X++;
				_manager.GameData.Falling.Last = Falling.LastKind.Move;
				_manager.GameData.Falling.Clamped = false;

				if (++_manager.GameData.Falling.LockResets < 15 || _manager.GameData.Options.InfiniteMovement)
					_manager.GameData.Falling.Locking = 0;
			}
			else
			{
				_manager.GameData.Falling.Clamped = true;
			}
		}
	}

	internal void ProcessShift(bool value, double subFrameDiff = 1)
	{
		ProcessLShift(value, subFrameDiff);
		ProcessRShift(value, subFrameDiff);
	}

	public void ProcessInterrupts()
	{
	}
}