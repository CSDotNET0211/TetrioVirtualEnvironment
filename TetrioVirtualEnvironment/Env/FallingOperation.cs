using TetrioVirtualEnvironment.Constants;

namespace TetrioVirtualEnvironment.Env;

public  class FallingOprations
{
	private readonly Environment _environment;
	public FallingOprations(Environment env)
	{
		_environment = env;
	}
	
	/// <summary>
		/// Right Left shift
		/// </summary>
		/// <param name="value"></param>
		/// <param name="subFrameDiff"></param>
		internal void ProcessShift(bool value, double subFrameDiff)
		{
			ProcessLShift(value, subFrameDiff);
			ProcessRShift(value,  subFrameDiff);
		}

		internal void ProcessLShift(bool value,  double subFrameDiff = 1)
		{
			if (!_environment.GameData.LShift || _environment.GameData.RShift && _environment.GameData.LastShift != -1)
				return;

			var subFrameDasDiff = subFrameDiff;
			var dasDiff = Math.Max(0, _environment.GameData.Handling.DAS - _environment.GameData.LDas);

			_environment.GameData.LDas += value ? 0 : subFrameDiff;

			if (_environment.GameData.LDas < _environment.GameData.Handling.DAS && !value)
				return;

			subFrameDasDiff = Math.Max(0, subFrameDasDiff - dasDiff);

			if (_environment.GameData.Falling.Sleep || _environment.GameData.Falling.DeepSleep)
				return;

			var moveLoopCount = 1;
			if (!value)
			{
				_environment.GameData.LDasIter += _environment.GameData.Options.Version >= 15 ? subFrameDasDiff : subFrameDiff;

				if (_environment.GameData.LDasIter < _environment.GameData.Handling.ARR)
					return;

				moveLoopCount = _environment.GameData.Handling.ARR == 0 ? 10 : (int)(_environment.GameData.LDasIter / _environment.GameData.Handling.ARR);

				_environment.GameData.LDasIter -= _environment.GameData.Handling.ARR * moveLoopCount;
			}

			for (var moveIndex = 0; moveIndex < moveLoopCount; moveIndex++)
			{
				if ( IsLegalAtPos(_environment.GameData.Falling.Type, _environment.GameData.Falling.X - 1, _environment.GameData.Falling.Y,
					    _environment.GameData.Falling.R, _environment.GameData.Board))
				{
					_environment.GameData.Falling.X--;
					_environment.GameData.Falling.Last = Falling.LastKind.Move;
					_environment.GameData.Falling.Clamped = false;

					if (++_environment.GameData.Falling.LockResets < 15 || _environment.GameData.Options.InfiniteMovement)
						_environment.GameData.Falling.Locking = 0;
				}
				else
				{
					_environment.GameData.Falling.Clamped = true;
				}
			}
		}

		internal void ProcessRShift(bool value,  double subFrameDiff = 1)
		{
			if (!_environment.GameData.RShift || _environment.GameData.LShift && _environment.GameData.LastShift != 1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasDiff = Math.Max(0, _environment.GameData.Handling.DAS - _environment.GameData.RDas);

			_environment.GameData.RDas += value ? 0 : subFrameDiff;

			if (_environment.GameData.RDas < _environment.GameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasDiff);
			if (_environment.GameData.Falling.Sleep || _environment.GameData.Falling.DeepSleep)
				return;

			var moveARRValue = 1;
			if (!value)
			{
				_environment.GameData.RDasIter += _environment.GameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (_environment.GameData.RDasIter < _environment.GameData.Handling.ARR)
					return;

				moveARRValue = _environment.GameData.Handling.ARR == 0 ? 10 : (int)(_environment.GameData.RDasIter / _environment.GameData.Handling.ARR);

				_environment.GameData.RDasIter -= _environment.GameData.Handling.ARR * moveARRValue;
			}

			for (var ARRIndex = 0; ARRIndex < moveARRValue; ARRIndex++)
			{
				if (IsLegalAtPos(_environment.GameData.Falling.Type, _environment.GameData.Falling.X + 1, _environment.GameData.Falling.Y,
					    _environment.GameData.Falling.R, _environment.GameData.Board))
				{
					_environment.GameData.Falling.X++;
					_environment.GameData.Falling.Last = Falling.LastKind.Move;
					_environment.GameData.Falling.Clamped = false;

					if (++_environment.GameData.Falling.LockResets < 15 || _environment.GameData.Options.InfiniteMovement)
						_environment.GameData.Falling.Locking = 0;
				}
				else
				{
					_environment.GameData.Falling.Clamped = true;
				}
			}
		}
		
		

		private  void RotatePiece(int rotation)
		{
			var currentMinoRotation = _environment.GameData.Falling.R;
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

			if (IsLegalAtPos(_environment.GameData.Falling.Type,
				_environment.GameData.Falling.X - _environment.GameData.Falling.Aox,
				_environment.GameData.Falling.Y - _environment.GameData.Falling.Aoy, rotation,
				_environment.GameData.Board))
			{
				_environment.GameData.Falling.X -= _environment.GameData.Falling.Aox;
				_environment.GameData.Falling.Y -= _environment.GameData.Falling.Aoy;
				_environment.GameData.Falling.Aox = 0;
				_environment.GameData.Falling.Aoy = 0;
				_environment.GameData.Falling.R = rotation;
				_environment.GameData.Falling.Last = Falling.LastKind.Rotate;
				_environment.GameData.Falling.LastRotation = lastRotation;
				_environment.GameData.Falling.LastKick = 0;
				_environment.GameData.Falling.SpinType = IsTspin();
				_environment.GameData.FallingRotations++;
				_environment.GameData.TotalRotations++;

				if (_environment.GameData.Falling.Clamped && _environment.GameData.Handling.DCD > 0)
				{
					_environment.GameData.LDas = Math.Min(_environment.GameData.LDas, _environment.GameData.Handling.DAS - _environment.GameData.Handling.DCD);
					_environment.GameData.LDasIter = _environment.GameData.Handling.ARR;
					_environment.GameData.RDas = Math.Min(_environment.GameData.RDas, _environment.GameData.Handling.DAS - _environment.GameData.Handling.DCD);
					_environment.GameData.RDasIter = _environment.GameData.Handling.ARR;
				}

				if (++_environment.GameData.Falling.LockResets < 15 || _environment.GameData.Options.InfiniteMovement)
					_environment.GameData.Falling.Locking = 0;


				return;
			}

			if (_environment.GameData.Falling.Type == Tetrimino.MinoType.O)
				return;

			var kicktable = KickTable.SRSPLUS[currentMinoNewRotation];

			if (_environment.GameData.Falling.Type == Tetrimino.MinoType.I)
				kicktable = KickTable.SRSPLUS_I[currentMinoNewRotation];

			for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
			{
				var kicktableTest = kicktable[kicktableIndex];
				var newMinoYPos = (int)(_environment.GameData.Falling.Y) + 0.1 +
					kicktableTest.y - _environment.GameData.Falling.Aoy;


				if (!_environment.GameData.Options.InfiniteMovement &&
					_environment.GameData.TotalRotations > (int)_environment.GameData.Options.LockResets + 15)
				{
					newMinoYPos = _environment.GameData.Falling.Y + kicktableTest.y - _environment.GameData.Falling.Aoy;
				}

				if (IsLegalAtPos(_environment.GameData.Falling.Type,
					_environment.GameData.Falling.X + (int)kicktableTest.x - _environment.GameData.Falling.Aox,
					newMinoYPos, rotation, _environment.GameData.Board))
				{

					_environment.GameData.Falling.X += (int)kicktableTest.x - _environment.GameData.Falling.Aox;
					_environment.GameData.Falling.Y = newMinoYPos;
					_environment.GameData.Falling.Aox = 0;
					_environment.GameData.Falling.Aoy = 0;
					_environment.GameData.Falling.R = rotation;
					_environment.GameData.Falling.SpinType = IsTspin();
					_environment.GameData.Falling.LastKick = kicktableIndex + 1;
					_environment.GameData.Falling.Last = Falling.LastKind.Rotate;
					_environment.GameData.FallingRotations++;
					_environment.GameData.TotalRotations++;

					if (_environment.GameData.Falling.Clamped && _environment.GameData.Handling.DCD > 0)
					{
						_environment.GameData.LDas = Math.Min(_environment.GameData.LDas, _environment.GameData.Handling.DAS - _environment.GameData.Handling.DCD);
						_environment.GameData.LDasIter = _environment.GameData.Handling.ARR;
						_environment.GameData.RDas = Math.Min(_environment.GameData.RDas, _environment.GameData.Handling.DAS - _environment.GameData.Handling.DCD);
						_environment.GameData.RDasIter = _environment.GameData.Handling.ARR;
					}

					if (++_environment.GameData.Falling.LockResets < 15 || _environment.GameData.Options.InfiniteMovement)
						_environment.GameData.Falling.Locking = 0;


					return;
				}
			}
		}
}