using TetrLoader.Enum;
using TetrLoader.JsonClass;

namespace TetrioVirtualEnvironment.Env;

public   class Garbage
{
	
	private readonly Environment _environment;
	public Garbage(Environment env)
	{
		_environment = env;
	}
	private void IncomingAttack(GarbageData data, string? sender = null, string? id1 = null, int? id2 = null)
	{
		if (_environment.GameData.Options.Passthrough == PassthroughType.Consistent)
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


		WaitFrames(_environment.GameData.Options.GarbageSpeed, "incoming-attack-hit", new IgeData()
		{
			data = data,
			sender = sender,
			cid = id2
		});


		int GetImpendingGarbageLineAtID(int? garbageID)
		{
			foreach (var garbage in _environment.GameData.ImpendingDamages)
			{
				if (garbage.cid == garbageID)
					return garbage.lines;
			}

			return 0;
		}
	}

	private void FightLines(int attackLine)
	{
		if (!_environment.GameData.Options.HasGarbage)
			return;

		Custom_environment.Stats.TotalSendLine += attackLine;

		bool count;
		if (_environment.GameData.ImpendingDamages.Count == 0)
			count = false;
		else
			count = true;

		var oValue = 0;
		for (; attackLine > 0 && _environment.GameData.ImpendingDamages.Count != 0;)
		{
			_environment.GameData.ImpendingDamages[0].lines--;
			if (_environment.GameData.ImpendingDamages[0].lines == 0)
			{
				_environment.GameData.ImpendingDamages.RemoveAt(0);
			}

			attackLine--;
			oValue++;
		}

		if (attackLine > 0)
		{
			if (_environment.GameData.Options.Passthrough == PassthroughType.Consistent && _environment.GameData.NotYetReceivedAttack > 0)
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
			foreach (var target in _environment.GameData.Targets)
			{
				var interactionID = ++_environment.GameData.InteractionID;

				if (_environment.GameData.Options.Passthrough is PassthroughType.Zero
				    or PassthroughType.Consistent)
				{
					if (!_environment.GameData.GarbageActnowledgements.Outgoing.ContainsKey(target))
						_environment.GameData.GarbageActnowledgements.Outgoing.Add(target, new List<GarbageData>());


					_environment.GameData.GarbageActnowledgements.Outgoing[target].Add(new GarbageData()
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
		_environment.GameData.NotYetReceivedAttack--;

		if (cid != null)
		{
			//ActiveDamage Function
			foreach (var garbage in _environment.GameData.ImpendingDamages)
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

		if (_environment.GameData.Options.Passthrough == PassthroughType.Zero)
		{
			amt_value = GetAmt(data, id1);
		}

		if (!(amt_value <= 0))
		{
			_environment.GameData.ImpendingDamages.Add(new IgeData()
			{
				id = ++_environment.GameData.GarbageID,
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
		if (!_environment.GameData.Options.HasGarbage)
			return;

		var ABoolValue = true;
		int maxReceiveGarbage =
			false ? 400 : (int)Math.Min(_environment.GameData.Options.GarbageCapMax, _environment.GameData.Options.GarbageCap);
		var oValue = false;
		var iArray = new List<IgeData>();

		for (var impendingIndex = _environment.GameData.ImpendingDamages.Count - 1; impendingIndex >= 0; impendingIndex--)
		{
			if (!_environment.GameData.ImpendingDamages[impendingIndex].active)
			{
				iArray.Insert(0, _environment.GameData.ImpendingDamages[impendingIndex]);
				_environment.GameData.ImpendingDamages.RemoveAt(impendingIndex);
			}
		}

		for (var i = 0; i < maxReceiveGarbage && _environment.GameData.ImpendingDamages.Count != 0; i++)
		{
			_environment.GameData.ImpendingDamages[0].lines--;
			oValue = true;
			bool rValue;
			if (_environment.GameData.ImpendingDamages[0].lines <= 0)
				rValue = true;
			else
				rValue = i >= maxReceiveGarbage - 1;

			if (!PushGarbageLine(_environment.GameData.ImpendingDamages[0].column, false,
				    (ABoolValue ? 64 : 0) | (rValue ? 4 : 0)))
				break;

			ABoolValue = false;

			if (!(_environment.GameData.ImpendingDamages[0].lines > 0))
			{
				_environment.GameData.ImpendingDamages.RemoveAt(0);
				ABoolValue = true;
			}
		}

		_environment.GameData.ImpendingDamages.AddRange(iArray.ToArray());
	}
}