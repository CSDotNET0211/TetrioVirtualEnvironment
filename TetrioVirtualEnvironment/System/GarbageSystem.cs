using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.Interface;
using TetrReplayLoader.JsonClass;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment.System
{
	public class GarbageSystem
	{
		internal static void FightLines(int attackLine, GameData gameData)
		{

			if (!gameData.Options.HasGarbage)
				return;

			bool count;
			if (gameData.ImpendingDamages.Count == 0)
				count = false;
			else
				count = true;

			var oValue = 0;
			for (; attackLine > 0 && gameData.ImpendingDamages.Count != 0;)
			{
				gameData.ImpendingDamages[0].lines--;
				if (gameData.ImpendingDamages[0].lines == 0)
				{
					gameData.ImpendingDamages.RemoveAt(0);
				}

				attackLine--;
				oValue++;
			}

			if (attackLine > 0)
			{
				if (gameData.Options.Passthrough == "consistent" && gameData.NotYetReceivedAttack > 0)
				{
					//PlaySound
				}
				Offence(attackLine, gameData);


			}

		}

		internal static void Offence(int attackLine, GameData gameData)
		{

			attackLine = attackLine / Math.Max(1, 1);
			if (!(attackLine < 1))
			{
				foreach (var target in gameData.Targets)
				{
					var interactionID = ++gameData.InteractionID;
					if (gameData.Options.Passthrough is "zero" or "consistent")
					{
						if (!gameData.GarbageActnowledgements.Outgoing.ContainsKey(target))
							gameData.GarbageActnowledgements.Outgoing.Add(target, new List<GarbageData>());


						gameData.GarbageActnowledgements.Outgoing[target].Add(new GarbageData()
						{
							iid = interactionID,
							amt = attackLine
						});

					}
				}
			}
		}

		internal static bool GetAttackPower(int clearLineCount, string? isTspin, GameData gameData,Stats stats)
		{
			var isBTB = false;

			if (clearLineCount > 0)
			{
				stats.Combo++;
				stats.TopCombo = Math.Max(stats.Combo, stats.TopCombo);

				if (clearLineCount == 4)
					isBTB = true;
				else
				{
					if (isTspin != null)
						isBTB = true;
				}

				if (isBTB)
				{
					stats.BTB++;
					stats.TopBTB = Math.Max(stats.BTB, stats.TopBTB);
				}
				else
				{
					stats.BTB = 0;

				}

			}
			else
			{
				stats.Combo = 0;
				stats.CurrentComboPower = 0;
			}


			var garbageValue = 0.0;
			switch (clearLineCount)
			{
				case 0:
					if (isTspin == "mini")
						garbageValue = ConstValue.Garbage.TSPIN_MINI;
					else if (isTspin == "normal")
						garbageValue = ConstValue.Garbage.TSPIN;
					break;

				case 1:
					if (isTspin == "mini")
						garbageValue = ConstValue.Garbage.TSPIN_MINI_SINGLE;
					else if (isTspin == "normal")
						garbageValue = ConstValue.Garbage.TSPIN_SINGLE;
					else
						garbageValue = ConstValue.Garbage.SINGLE;
					break;

				case 2:
					if (isTspin == "mini")
						garbageValue = ConstValue.Garbage.TSPIN_MINI_DOUBLE;
					else if (isTspin == "normal")
						garbageValue = ConstValue.Garbage.TSPIN_DOUBLE;
					else
						garbageValue = ConstValue.Garbage.DOUBLE;
					break;

				case 3:
					if (isTspin != null)
						garbageValue = ConstValue.Garbage.TSPIN_TRIPLE;
					else
						garbageValue = ConstValue.Garbage.TRIPLE;
					break;

				case 4:
					if (isTspin != null)
						garbageValue = ConstValue.Garbage.TSPIN_QUAD;
					else
						garbageValue = ConstValue.Garbage.QUAD;

					break;
			}


			if (clearLineCount > 0 && stats.BTB > 1)
			{
				if (gameData.Options.BTBChaining)
				{
					double tempValue;
					if (stats.BTB - 1 == 1)
						tempValue = 0;
					else
						tempValue = 1 + (Math.Log((stats.BTB - 1) * ConstValue.Garbage.BACKTOBACK_BONUS_LOG + 1) % 1);


					var btb_bonus = ConstValue.Garbage.BACKTOBACK_BONUS *
						(Math.Floor(1 + Math.Log((stats.BTB - 1) * ConstValue.Garbage.BACKTOBACK_BONUS_LOG + 1)) + (tempValue / 3));

					garbageValue += btb_bonus;

					if ((int)btb_bonus >= 2)
					{
						//AddFire
					}

					if ((int)btb_bonus > gameData.CurrentBTBChainPower)
					{
						gameData.CurrentBTBChainPower = (int)btb_bonus;
					}



				}
				else
					garbageValue += ConstValue.Garbage.BACKTOBACK_BONUS;
			}
			else
			{
				if (clearLineCount > 0 && stats.BTB <= 1)
					gameData.CurrentBTBChainPower = 0;
			}

			if (stats.Combo > 1)
			{
				garbageValue *= 1 + ConstValue.Garbage.COMBO_BONUS * (stats.Combo - 1);
			}

			if (stats.Combo > 2)
			{
				garbageValue = Math.Max(Math.Log(ConstValue.Garbage.COMBO_MINIFIER *
					(stats.Combo - 1) * ConstValue.Garbage.COMBO_MINIFIER_LOG + 1), garbageValue);
			}


			int totalPower = (int)(garbageValue * gameData.Options.GarbageMultiplier);
			if (stats.Combo > 2)
				stats.CurrentComboPower = Math.Max(stats.CurrentComboPower, totalPower);

			if (clearLineCount > 0 && stats.Combo > 1 && stats.CurrentComboPower >= 7)
			{

			}

			switch (gameData.Options.GarbageBlocking)
			{
				case "combo blocking":
					if (clearLineCount > 0)
						FightLines(totalPower, gameData);
					return clearLineCount > 0;

				case "limited blocking":
					if (clearLineCount > 0)
						FightLines(totalPower, gameData);
					return false;

				case "none":	
					Offence(totalPower, gameData);
					return false;

				default: throw new Exception();
			}
		}

		internal static bool PushGarbageLine(int line, GameData gameData,bool falseValue = false, int whatIsThis = 68)
		{
			var newBoardList = new List<MinoKind>();
			newBoardList.AddRange((MinoKind[])gameData.Board.Clone());

			for (int x = 0; x < FIELD_WIDTH; x++)
			{
				//x+y*10
				if (newBoardList[x] != MinoKind.Empty)
					return false;
			}

			//一番てっぺんを消す
			for (int x = 0; x < FIELD_WIDTH; x++)
				newBoardList.RemoveAt(x);

			var RValueList = new List<MinoKind>();

			for (var tIndex = 0; tIndex < FIELD_WIDTH; tIndex++)
			{
				if (tIndex == line)
					RValueList.Add(MinoKind.Empty);
				else
					RValueList.Add(MinoKind.Garbage);
			}

			newBoardList.AddRange(RValueList);
			gameData.Board = newBoardList.ToArray();
			return true;
		}

	}
}
