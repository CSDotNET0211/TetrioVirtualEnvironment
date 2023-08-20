using TetrLoader.Enum;
using TetrLoader.JsonClass;

namespace TetrioVirtualEnvironment.Env;

public partial class Environment
{
	private void IncomingAttack(GarbageData data, string? sender = null, string? id1 = null, int? id2 = null)
	{
		if (GameData.Options.Passthrough == PassthroughType.Consistent)
		{
			data.amt = GetAmt(data, id1);
		}
		else
		{
			if (id2 != null)
				data.amt = GetImpendingGarbageLineAtID(id2);

			if (data.amt <= 0)
				return;
		}


		WaitFrames(GameData.Options.GarbageSpeed, "incoming-attack-hit", new IgeData()
		{
			data = data,
			sender = sender,
			cid = id2
		});


		int GetImpendingGarbageLineAtID(int? garbageID)
		{
			foreach (var garbage in GameData.ImpendingDamages)
			{
				if (garbage.cid == garbageID)
					return garbage.lines;
			}

			return 0;
		}
	}

	private void FightLines(int attackLine)
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

	private void StartingAttack(GarbageData data, string sender, string? id1, int? id2)
	{
		InsertDamage(data, sender, id1, id2);
	}

	private void Offence(int attackLine)
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

	private void IncomingAttackHit(GarbageData data, string sender, int? cid)
	{
		GameData.NotYetReceivedAttack--;

		if (cid != null)
		{
			//ActiveDamage Function
			foreach (var garbage in GameData.ImpendingDamages)
			{
				if (garbage.cid == cid)
					garbage.active = true;
			}
		}
		else
		{
			InsertDamage(data, sender);
		}
	}

	private void InsertDamage(GarbageData data, string sender, string? id1 = null, int? id2 = null)
	{
		var amt_value = data.amt;

		if (GameData.Options.Passthrough == PassthroughType.Zero)
		{
			amt_value = GetAmt(data, id1);
		}

		if (!(amt_value <= 0))
		{
			GameData.ImpendingDamages.Add(new IgeData()
			{
				id = ++GameData.GarbageID,
				type = data.type,
				active = id2 == null,
				lines = amt_value,
				column = data.column,
				data = data,
				sender = sender,
				cid = id2
			});
		}
	}
	
	private void TakeAllDamage()
	{
		if (!GameData.Options.HasGarbage)
			return;

		var ABoolValue = true;
		int maxReceiveGarbage =
			false ? 400 : (int)Math.Min(GameData.Options.GarbageCapMax, GameData.Options.GarbageCap);
		var oValue = false;
		var iArray = new List<IgeData>();

		for (var impendingIndex = GameData.ImpendingDamages.Count - 1; impendingIndex >= 0; impendingIndex--)
		{
			if (!GameData.ImpendingDamages[impendingIndex].active)
			{
				iArray.Insert(0, GameData.ImpendingDamages[impendingIndex]);
				GameData.ImpendingDamages.RemoveAt(impendingIndex);
			}
		}

		for (var i = 0; i < maxReceiveGarbage && GameData.ImpendingDamages.Count != 0; i++)
		{
			GameData.ImpendingDamages[0].lines--;
			oValue = true;
			bool rValue;
			if (GameData.ImpendingDamages[0].lines <= 0)
				rValue = true;
			else
				rValue = i >= maxReceiveGarbage - 1;

			if (!PushGarbageLine(GameData.ImpendingDamages[0].column, false,
				    (ABoolValue ? 64 : 0) | (rValue ? 4 : 0)))
				break;

			ABoolValue = false;

			if (!(GameData.ImpendingDamages[0].lines > 0))
			{
				GameData.ImpendingDamages.RemoveAt(0);
				ABoolValue = true;
			}
		}

		GameData.ImpendingDamages.AddRange(iArray.ToArray());
	}
}