namespace TetrioVirtualEnvironment.System
{
	public abstract class MovementSystem
	{
		/// <summary>
		/// Right Left shift
		/// </summary>
		/// <param name="value"></param>
		/// <param name="subFrameDiff"></param>
		internal static void ProcessShift(bool value, double subFrameDiff, GameData gameData)
		{
			ProcessLShift(value, gameData, subFrameDiff);
			ProcessRShift(value, gameData, subFrameDiff);
		}

		internal static void ProcessLShift(bool value, GameData gameData, double subFrameDiff = 1)
		{
			if (!gameData.LShift || gameData.RShift && gameData.LastShift != -1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasSomething = Math.Max(0, gameData.Handling.DAS - gameData.LDas);

			gameData.LDas += value ? 0 : subFrameDiff;

			if (gameData.LDas < gameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);

			if (gameData.Falling.Sleep || gameData.Falling.DeepSleep)
				return;

			var aboutARRValue = 1;
			if (!value)
			{
				gameData.LDasIter += gameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (gameData.LDasIter < gameData.Handling.ARR)
					return;

				aboutARRValue = gameData.Handling.ARR == 0 ? 10 : (int)(gameData.LDasIter / gameData.Handling.ARR);

				gameData.LDasIter -= gameData.Handling.ARR * aboutARRValue;
			}

			for (var index = 0; index < aboutARRValue; index++)
			{
				if (JudgeSystem.IsLegalAtPos(gameData.Falling.Type, gameData.Falling.X - 1, gameData.Falling.Y,
					    gameData.Falling.R, gameData.Board))
				{
					gameData.Falling.X--;
					gameData.Falling.Last = Falling.LastKind.Move;
					gameData.Falling.Clamped = false;

					if (++gameData.Falling.LockResets < 15 || gameData.Options.InfiniteMovement)
						gameData.Falling.Locking = 0;
				}
				else
				{
					gameData.Falling.Clamped = true;
				}
			}
		}

		internal static void ProcessRShift(bool value, GameData gameData, double subFrameDiff = 1)
		{
			if (!gameData.RShift || gameData.LShift && gameData.LastShift != 1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasDiff = Math.Max(0, gameData.Handling.DAS - gameData.RDas);

			gameData.RDas += value ? 0 : subFrameDiff;

			if (gameData.RDas < gameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasDiff);
			if (gameData.Falling.Sleep || gameData.Falling.DeepSleep)
				return;

			var moveARRValue = 1;
			if (!value)
			{
				gameData.RDasIter += gameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (gameData.RDasIter < gameData.Handling.ARR)
					return;

				moveARRValue = gameData.Handling.ARR == 0 ? 10 : (int)(gameData.RDasIter / gameData.Handling.ARR);

				gameData.RDasIter -= gameData.Handling.ARR * moveARRValue;
			}

			for (var ARRIndex = 0; ARRIndex < moveARRValue; ARRIndex++)
			{
				if (JudgeSystem.IsLegalAtPos(gameData.Falling.Type, gameData.Falling.X + 1, gameData.Falling.Y,
					    gameData.Falling.R, gameData.Board))
				{
					gameData.Falling.X++;
					gameData.Falling.Last = Falling.LastKind.Move;
					gameData.Falling.Clamped = false;

					if (++gameData.Falling.LockResets < 15 || gameData.Options.InfiniteMovement)
						gameData.Falling.Locking = 0;
				}
				else
				{
					gameData.Falling.Clamped = true;
				}
			}
		}
	}
}