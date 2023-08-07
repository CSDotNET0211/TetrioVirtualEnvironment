namespace TetrioVirtualEnvironment
{
	public partial class Environment
	{
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
			if (!GameData.LShift || GameData.RShift && GameData.LastShift != -1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasSomething = Math.Max(0, GameData.Handling.DAS - GameData.LDas);

			GameData.LDas += value ? 0 : subFrameDiff;

			if (GameData.LDas < GameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);

			if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
				return;

			var aboutARRValue = 1;
			if (!value)
			{
				GameData.LDasIter += GameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (GameData.LDasIter < GameData.Handling.ARR)
					return;

				aboutARRValue = GameData.Handling.ARR == 0 ? 10 : (int)(GameData.LDasIter / GameData.Handling.ARR);

				GameData.LDasIter -= GameData.Handling.ARR * aboutARRValue;
			}

			for (var index = 0; index < aboutARRValue; index++)
			{
				if ( IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X - 1, GameData.Falling.Y,
					    GameData.Falling.R, GameData.Board))
				{
					GameData.Falling.X--;
					GameData.Falling.Last = Falling.LastKind.Move;
					GameData.Falling.Clamped = false;

					if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
						GameData.Falling.Locking = 0;
				}
				else
				{
					GameData.Falling.Clamped = true;
				}
			}
		}

		internal void ProcessRShift(bool value,  double subFrameDiff = 1)
		{
			if (!GameData.RShift || GameData.LShift && GameData.LastShift != 1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasDiff = Math.Max(0, GameData.Handling.DAS - GameData.RDas);

			GameData.RDas += value ? 0 : subFrameDiff;

			if (GameData.RDas < GameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasDiff);
			if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
				return;

			var moveARRValue = 1;
			if (!value)
			{
				GameData.RDasIter += GameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (GameData.RDasIter < GameData.Handling.ARR)
					return;

				moveARRValue = GameData.Handling.ARR == 0 ? 10 : (int)(GameData.RDasIter / GameData.Handling.ARR);

				GameData.RDasIter -= GameData.Handling.ARR * moveARRValue;
			}

			for (var ARRIndex = 0; ARRIndex < moveARRValue; ARRIndex++)
			{
				if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X + 1, GameData.Falling.Y,
					    GameData.Falling.R, GameData.Board))
				{
					GameData.Falling.X++;
					GameData.Falling.Last = Falling.LastKind.Move;
					GameData.Falling.Clamped = false;

					if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
						GameData.Falling.Locking = 0;
				}
				else
				{
					GameData.Falling.Clamped = true;
				}
			}
		}
	}
}