using TetrEnvironment.Constants;
using TetrLoader.Enum;

namespace TetrEnvironment.Info;

public class LineInfo
{
	private Environment _manager;

	public LineInfo(Environment manager)
	{
		_manager = manager;
	}

	public int ClearLines()
	{
		var completedLines = _manager.BoardInfo.GetFullLines();
		foreach (var y in completedLines)
		{
			if (_manager.GameData.Board[0 + y * _manager.BoardInfo.Width] == Tetromino.MinoType.Garbage ||
			    _manager.GameData.Board[1 + y * _manager.BoardInfo.Width] == Tetromino.MinoType.Garbage)
				_manager.CustomStats.garbageCleared++;
			else
				_manager.CustomStats.stackCleared++;
		}

		//NOTE: tetrio.jsだとなぜか引数があるけど無視、そもそも引数を受け付けてない
		var spinType = _manager.FallInfo.IsTspin();

		if (_manager.GameData.Falling.Type == Tetromino.MinoType.T &&
		    _manager.GameData.Options.SpinBonuses == SpinBonusesType.Stupid)
			spinType = _manager.GameData.Falling.SpinType;

		//AnnounceDownstack(completedLines);

		//AnnounceDownstack();
		if (completedLines.Count > 0)
			_manager.BoardInfo.RemoveLinesFromStack(completedLines);

		var announceLines = AnnounceLines(completedLines.Count, spinType);

		if (_manager.GameData.Options.AllClears)
			AnnounceClear();

		if (!announceLines)
			_manager.GarbageInfo.TakeAllDamage();
		//AnnounseOffensive

		return completedLines.Count;
	}

	public void LevelLines(int clearedLineCount, bool value = false, int newLevel = 1)
	{
		if (value)
		{
			_manager.GameData.Stats.Level = newLevel;
			_manager.GameData.Gravity = Math.Min(int.MaxValue - 1,
				1.0 / 60 / Math.Pow(
					Math.Max(0.000000001,
						(_manager.GameData.Options.GBase) - (_manager.GameData.Stats.Level - 1) *
						(_manager.GameData.Options.GSpeed)), _manager.GameData.Stats.Level - 1));
			_manager.GameData.Stats.LevelLines = 0;

			if (_manager.GameData.Options.LevelStatic)
				_manager.GameData.Stats.LevelLinesNeeded = _manager.GameData.Options.LevelStaticSpeed ?? 10;
			else
				_manager.GameData.Stats.LevelLinesNeeded = Math.Ceiling(_manager.GameData.Stats.Level *
				                                                        (_manager.GameData.Options.LevelSpeed ?? 1) *
				                                                        5);
			if (_manager.GameData.Options.MasterLevels && _manager.GameData.Stats.Level > 20)
				_manager.GameData.Options.LockTime = 30 - Math.Min(25, _manager.GameData.Stats.Level - 20);
			if (_manager.GameData.Options.MasterLevels && _manager.GameData.Stats.Level > 45)
				_manager.GameData.Options.LockResets = 15 - Math.Min(10, _manager.GameData.Stats.Level - 45);
		}
		else
		{
			_manager.GameData.Stats.LevelLines += clearedLineCount;

			if (_manager.GameData.Stats.LevelLines >= _manager.GameData.Stats.LevelLinesNeeded)
			{
				for (; _manager.GameData.Stats.LevelLines >= _manager.GameData.Stats.LevelLinesNeeded;)
				{
					_manager.GameData.Stats.LevelLines -= _manager.GameData.Stats.LevelLinesNeeded;
					_manager.GameData.Stats.Level++;
					_manager.GameData.Gravity = Math.Min(int.MaxValue - 1,
						1f / 60 / Math.Pow(
							Math.Max(0.000000001,
								(_manager.GameData.Options.GBase) - (_manager.GameData.Stats.Level - 1) *
								(_manager.GameData.Options.GSpeed)), _manager.GameData.Stats.Level - 1));

					if (_manager.GameData.Options.LevelStatic)
						_manager.GameData.Stats.LevelLinesNeeded = _manager.GameData.Options.LevelStaticSpeed ?? 10;
					else
						_manager.GameData.Stats.LevelLinesNeeded = Math.Ceiling(_manager.GameData.Stats.Level *
							(_manager.GameData.Options.LevelSpeed ?? 1) *
							5);

					if (_manager.GameData.Options.MasterLevels && _manager.GameData.Stats.Level > 20)
						_manager.GameData.Options.LockTime = 30 - Math.Min(25, _manager.GameData.Stats.Level - 20);
					if (_manager.GameData.Options.MasterLevels && _manager.GameData.Stats.Level > 45)
						_manager.GameData.Options.LockResets = 15 - Math.Min(10, _manager.GameData.Stats.Level - 45);
				}
			}
		}
	}

	public void AnnounceDownstack()
	{
		throw new NotImplementedException();
	}

	public bool AnnounceLines(int completedLineCount, Falling.SpinTypeKind spinbonus)
	{
		bool isBTB = false;
		if (_manager.GameData.Options.Levels)
			LevelLines(completedLineCount);

		if (completedLineCount > 0)
		{
			if (_manager.GameData.Options.ComboTable != ComboTableType.None)
			{
				_manager.GameData.Stats.Combo++;
				_manager.GameData.Stats.TopCombo =
					Math.Max(_manager.GameData.Stats.Combo, _manager.GameData.Stats.TopCombo);
			}


			if (completedLineCount >= 4)
				isBTB = true;
			else
			{
				if (spinbonus != Falling.SpinTypeKind.Null)
					isBTB = true;
			}

			if (isBTB)
			{
				_manager.GameData.Stats.BTB++;
				_manager.GameData.Stats.TopBTB =
					Math.Max(_manager.GameData.Stats.BTB, _manager.GameData.Stats.TopBTB);
			}
			else
			{
				_manager.GameData.Stats.BTB = 0;
			}
		}
		else
		{
			_manager.GameData.Stats.Combo = 0;
			_manager.GameData.Stats.CurrentComboPower = 0;
		}

		if (spinbonus != Falling.SpinTypeKind.Null)
		{
		}

		var score = 0;
		var garbageValue = 0.0;

		switch (completedLineCount)
		{
			case 0:
				if (spinbonus == Falling.SpinTypeKind.Mini)
					garbageValue = Garbage.TSPIN_MINI;
				else if (spinbonus == Falling.SpinTypeKind.Normal)
					garbageValue = Garbage.TSPIN;
				break;

			case 1:
				if (spinbonus == Falling.SpinTypeKind.Mini)
					garbageValue = Garbage.TSPIN_MINI_SINGLE;
				else if (spinbonus == Falling.SpinTypeKind.Normal)
					garbageValue = Garbage.TSPIN_SINGLE;
				else
					garbageValue = Garbage.SINGLE;
				break;

			case 2:
				if (spinbonus == Falling.SpinTypeKind.Mini)
					garbageValue = Garbage.TSPIN_MINI_DOUBLE;
				else if (spinbonus == Falling.SpinTypeKind.Normal)
					garbageValue = Garbage.TSPIN_DOUBLE;
				else
					garbageValue = Garbage.DOUBLE;
				break;

			case 3:
				if (spinbonus != Falling.SpinTypeKind.Null)
					garbageValue = Garbage.TSPIN_TRIPLE;
				else
					garbageValue = Garbage.TRIPLE;
				break;

			case 4:
				if (spinbonus != Falling.SpinTypeKind.Null)
					garbageValue = Garbage.TSPIN_QUAD;
				else
					garbageValue = Garbage.QUAD;
				break;

			case 5:
				if (spinbonus != Falling.SpinTypeKind.Null)
					garbageValue = Garbage.TSPIN_PENTA;
				else
					garbageValue = Garbage.PENTA;
				break;

			default:
				var clearDiff = completedLineCount - 5;
				if (spinbonus != Falling.SpinTypeKind.Null)
					garbageValue = Garbage.TSPIN_PENTA + 2 * clearDiff;
				else
					garbageValue = Garbage.PENTA + clearDiff;
				break;
		}

		if (spinbonus != Falling.SpinTypeKind.Null &&
		    _manager.GameData.Options.SpinBonuses == SpinBonusesType.Handheld &&
		    _manager.GameData.Falling.Type != Tetromino.MinoType.T)
			garbageValue /= 2;

		if (completedLineCount > 0 && _manager.GameData.Stats.BTB > 1)
		{
			if (_manager.GameData.Options.BTBChaining)
			{
				double tempValue;
				if (_manager.GameData.Stats.BTB - 1 == 1)
					tempValue = 0;
				else
					tempValue = 1 + (Math.Log((_manager.GameData.Stats.BTB - 1) *
						Garbage.BACKTOBACK_BONUS_LOG + 1) % 1);


				var btbBonus = Garbage.BACKTOBACK_BONUS *
				               (Math.Floor(1 + Math.Log((_manager.GameData.Stats.BTB - 1) *
					               Garbage.BACKTOBACK_BONUS_LOG + 1)) + (tempValue / 3));

				garbageValue += btbBonus;

				if ((int)btbBonus >= 2)
				{
					//AddFire
				}

				if ((int)btbBonus > _manager.GameData.CurrentBTBChainPower)
					_manager.GameData.CurrentBTBChainPower = (int)btbBonus;
			}
			else
				garbageValue += Garbage.BACKTOBACK_BONUS;
		}
		else
		{
			if (completedLineCount > 0 && _manager.GameData.Stats.BTB <= 1)
				_manager.GameData.CurrentBTBChainPower = 0;
		}

		if (_manager.GameData.Options.ComboTable == ComboTableType.Multiplier)
		{
			garbageValue *= 1 + Garbage.COMBO_BONUS * (_manager.GameData.Stats.Combo - 1);

			if (_manager.GameData.Stats.Combo > 2)
			{
				garbageValue = Math.Max(Math.Log(Garbage.COMBO_MINIFIER *
						(_manager.GameData.Stats.Combo - 1) * Garbage.COMBO_MINIFIER_LOG + 1),
					garbageValue);
			}
		}
		else
		{
			var combotable = Garbage.COMBO_TABLE[(int)_manager.GameData.Options.ComboTable];
			garbageValue += combotable[Math.Max(0, Math.Min(_manager.GameData.Stats.Combo - 2, combotable.Length - 1))];
		}


		if (completedLineCount > 0 && _manager.GameData.Options.GarbageTargetBonus != GarbageTargetBonusType.None)
		{
			int enemyBonus = 0;
			switch (_manager.GameData.Enemies.Count)
			{
				case 0:
				case 1:
					break;
				case 2:
					enemyBonus += 1;
					break;
				case 3:
					enemyBonus += 3;
					break;
				case 4:
					enemyBonus += 5;
					break;
				case 5:
					enemyBonus += 7;
					break;
				default:
					enemyBonus += 9;
					break;
			}

			if (_manager.GameData.Options.GarbageTargetBonus == GarbageTargetBonusType.Offensive)
				garbageValue += enemyBonus;
			else
				_manager.GameData.GarbageBonus = (int)(enemyBonus * _manager.GameData.Options.GarbageMultiplier);
		}

		int multiplieredGarbage = (int)(garbageValue * _manager.GameData.Options.GarbageMultiplier);

		if (_manager.GameData.Options.GarbageAttackCap)
			multiplieredGarbage = (int)(Math.Min(_manager.GameData.Options.GarbageCap, multiplieredGarbage));

		if (_manager.GameData.Stats.Combo > 2)
			_manager.GameData.Stats.CurrentComboPower =
				Math.Max(_manager.GameData.Stats.CurrentComboPower, multiplieredGarbage);

		if (completedLineCount != 0 &&
		    _manager.GameData.Stats.Combo > 1 &&
		    _manager.GameData.Stats.CurrentComboPower >= 7)
		{
			//AddFire
		}

		if (completedLineCount != 0)
			_manager.CustomStats.lineSent += multiplieredGarbage;

		switch (_manager.GameData.Options.GarbageBlocking)
		{
			case GarbageBlockingType.ComboBlocking:
				if (completedLineCount != 0)
					_manager.GarbageInfo.FightLines(multiplieredGarbage);
				return completedLineCount > 0;

			case GarbageBlockingType.LimitedBlocking:
				if (completedLineCount != 0)
					_manager.GarbageInfo.FightLines(multiplieredGarbage);
				return false;

			case GarbageBlockingType.None:
				_manager.GarbageInfo.Offence(multiplieredGarbage);
				return false;

			default:
				throw new Exception();
		}
	}

	public void AnnounceClear()
	{
		//TODO:処理の簡略化 
		var isCleared = _manager.GameData.Board.All(x => x == Tetromino.MinoType.Empty);
		if (isCleared)
			_manager.GarbageInfo.FightLines((int)(Garbage.ALL_CLEAR * _manager.GameData.Options.GarbageMultiplier));
	}
}