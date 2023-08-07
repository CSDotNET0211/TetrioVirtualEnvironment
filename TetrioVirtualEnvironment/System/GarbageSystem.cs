using TetrioVirtualEnvironment.Constants;
using TetrLoader.Enum;
using TetrLoader.JsonClass;

namespace TetrioVirtualEnvironment
{
	public partial class Environment
	{
		private  void FightLines(int attackLine)
		{
			if (!GameData.Options.HasGarbage)
				return;

			CustomStats.TotalSendLine += attackLine;
			
			bool count;
			if (GameData.ImpendingDamages.Count == 0)
				count = false;
			else
				count = true;

			var oValue = 0;
			for (; attackLine > 0 && GameData.ImpendingDamages.Count != 0;)
			{
				GameData.ImpendingDamages[0].lines--;
				if (GameData.ImpendingDamages[0].lines == 0)
				{
					GameData.ImpendingDamages.RemoveAt(0);
				}

				attackLine--;
				oValue++;
			}

			if (attackLine > 0)
			{
				if (GameData.Options.Passthrough == PassthroughType.Consistent && GameData.NotYetReceivedAttack > 0)
				{
					//PlaySound
				}

				Offence(attackLine);
			}
		}

		private  void Offence(int attackLine)
		{
			attackLine = attackLine / Math.Max(1, 1);
			if (!(attackLine < 1))
			{
				foreach (var target in GameData.Targets)
				{
					var interactionID = ++GameData.InteractionID;

					if (GameData.Options.Passthrough is PassthroughType.Zero
					    or PassthroughType.Consistent)
					{
						if (!GameData.GarbageActnowledgements.Outgoing.ContainsKey(target))
							GameData.GarbageActnowledgements.Outgoing.Add(target, new List<GarbageData>());


						GameData.GarbageActnowledgements.Outgoing[target].Add(new GarbageData()
						{
							iid = interactionID,
							amt = attackLine
						});
					}
				}
			}
		}

		internal  bool GetAttackPower(int clearLineCount, Falling.SpinTypeKind isTspin)
		{
			var isBTB = false;

			if (clearLineCount > 0)
			{
				Stats.Combo++;
				Stats.TopCombo = Math.Max(Stats.Combo, Stats.TopCombo);

				if (clearLineCount == 4)
					isBTB = true;
				else
				{
					if (isTspin != Falling.SpinTypeKind.Null)
						isBTB = true;
				}

				if (isBTB)
				{
					Stats.BTB++;
					Stats.TopBTB = Math.Max(Stats.BTB, Stats.TopBTB);
				}
				else
				{
					Stats.BTB = 0;
				}
			}
			else
			{
				Stats.Combo = 0;
				Stats.CurrentComboPower = 0;
			}


			var garbageValue = 0.0;
			switch (clearLineCount)
			{
				case 0:
					if (isTspin == Falling.SpinTypeKind.Mini)
						garbageValue = Constants.Garbage.TSPIN_MINI;
					else if (isTspin == Falling.SpinTypeKind.Normal)
						garbageValue = Constants.Garbage.TSPIN;
					break;

				case 1:
					if (isTspin == Falling.SpinTypeKind.Mini)
						garbageValue = Constants.Garbage.TSPIN_MINI_SINGLE;
					else if (isTspin == Falling.SpinTypeKind.Normal)
						garbageValue = Constants.Garbage.TSPIN_SINGLE;
					else
						garbageValue = Constants.Garbage.SINGLE;
					break;

				case 2:
					if (isTspin == Falling.SpinTypeKind.Mini)
						garbageValue = Constants.Garbage.TSPIN_MINI_DOUBLE;
					else if (isTspin == Falling.SpinTypeKind.Normal)
						garbageValue = Constants.Garbage.TSPIN_DOUBLE;
					else
						garbageValue = Constants.Garbage.DOUBLE;
					break;

				case 3:
					if (isTspin != Falling.SpinTypeKind.Null)
						garbageValue = Constants.Garbage.TSPIN_TRIPLE;
					else
						garbageValue = Constants.Garbage.TRIPLE;
					break;

				case 4:
					if (isTspin != Falling.SpinTypeKind.Null)
						garbageValue = Constants.Garbage.TSPIN_QUAD;
					else
						garbageValue = Constants.Garbage.QUAD;

					break;
			}


			if (clearLineCount > 0 && Stats.BTB > 1)
			{
				if (GameData.Options.BTBChaining)
				{
					double tempValue;
					if (Stats.BTB - 1 == 1)
						tempValue = 0;
					else
						tempValue = 1 + (Math.Log((Stats.BTB - 1) * Constants.Garbage.BACKTOBACK_BONUS_LOG + 1) % 1);


					var btb_bonus = Constants.Garbage.BACKTOBACK_BONUS *
					                (Math.Floor(1 +
					                            Math.Log((Stats.BTB - 1) * Constants.Garbage.BACKTOBACK_BONUS_LOG +
					                                     1)) + (tempValue / 3));

					garbageValue += btb_bonus;

					if ((int)btb_bonus >= 2)
					{
						//AddFire
					}

					if ((int)btb_bonus > GameData.CurrentBTBChainPower)
					{
						GameData.CurrentBTBChainPower = (int)btb_bonus;
					}
				}
				else
					garbageValue += Constants.Garbage.BACKTOBACK_BONUS;
			}
			else
			{
				if (clearLineCount > 0 && Stats.BTB <= 1)
					GameData.CurrentBTBChainPower = 0;
			}

			if (Stats.Combo > 1)
			{
				garbageValue *= 1 + Constants.Garbage.COMBO_BONUS * (Stats.Combo - 1);
			}

			if (Stats.Combo > 2)
			{
				garbageValue = Math.Max(Math.Log(Constants.Garbage.COMBO_MINIFIER *
					(Stats.Combo - 1) * Constants.Garbage.COMBO_MINIFIER_LOG + 1), garbageValue);
			}


			int totalPower = (int)(garbageValue * GameData.Options.GarbageMultiplier);
			if (Stats.Combo > 2)
				Stats.CurrentComboPower = Math.Max(Stats.CurrentComboPower, totalPower);

			if (clearLineCount > 0 && Stats.Combo > 1 && Stats.CurrentComboPower >= 7)
			{
			}
			
			switch (GameData.Options.GarbageBlocking)
			{
				case GarbageBlockingType.ComboBlocking:
					if (clearLineCount > 0)
						FightLines(totalPower);
					return clearLineCount > 0;

				case GarbageBlockingType.LimitedBlocking:
					if (clearLineCount > 0)
						FightLines(totalPower);
					return false;

				case GarbageBlockingType.None:
					Offence(totalPower);
					return false;

				default: throw new Exception();
			}
		}

		internal  bool PushGarbageLine(int line, bool falseValue = false, int whatIsThis = 68)
		{
			var newBoardList = new List<Tetrimino.MinoType>();
			newBoardList.AddRange((Tetrimino.MinoType[])GameData.Board.Clone());

			for (int x = 0; x < FIELD_WIDTH; x++)
			{
				//x+y*10
				if (newBoardList[x] != Tetrimino.MinoType.Empty)
					return false;
			}

			//一番てっぺんを消す
			for (int x = 0; x < FIELD_WIDTH; x++)
				newBoardList.RemoveAt(x);

			var RValueList = new List<Tetrimino.MinoType>();

			for (var tIndex = 0; tIndex < FIELD_WIDTH; tIndex++)
			{
				if (tIndex == line)
					RValueList.Add(Tetrimino.MinoType.Empty);
				else
					RValueList.Add(Tetrimino.MinoType.Garbage);
			}

			newBoardList.AddRange(RValueList);
			GameData.Board = newBoardList.ToArray();
			return true;
		}
	}
}