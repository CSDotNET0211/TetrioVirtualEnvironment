using TetrLoader.Enum;
using TetrLoader.Ige;

namespace TetrEnvironment.Info;

public class GarbageInfo
{
	private readonly Environment _manager;

	public enum WaitingFrameType
	{
		Are,
		IncomingAttackHit,
		ProcessGarbageStatus,
		PushGarbageLine,
		OutgoingAttackHit,
		FreezeCounters,
		ReviveFromStackLoss
	}


	public GarbageInfo(Environment manager)
	{
		_manager = manager;
	}

	private int GetGarbageCap()
	{
		var garbageCap = Math.Min(_manager.GameData.Options.GarbageCapMax, _manager.GameData.Options.GarbageCap);
		return (int)garbageCap;
	}

	public void AddPendingGarbage(GarbageData data, string? gameid, int? cid)
	{
		int amt = (int)data.amt;
		if (_manager.GameData.Options.Passthrough == PassthroughType.Zero && gameid != null)
		{
			amt = RefreshGarbages(data, gameid);
		}

		if (amt <= 0)
			return;

		var newGarbageId = ++_manager.GameData.GarbageId;
		var iid = data.iid;
		var ackiid = data.ackiid;
		var type = data.type;
		var active = data.active ?? cid == null;
		var column = data.column;
		var username = data.username;
		var delay = data.delay ?? (_manager.GameData.Options.GarbagePhase ? 1 : 0);
		var queued = data.queued ?? _manager.GameData.Options.GarbageQueue;
		var size = data.size ?? _manager.GameData.Options.GarbageHoleSize;
		var x = data.x;
		var y = data.y;
		GarbageStatus status = GarbageStatus.Spawn;

		if (queued || delay >= 1)
		{
			status = GarbageStatus.Caution;
			if (_manager.GameData.ImpendingDamage.Count > 0 && queued)
				status = GarbageStatus.Sleeping;
		}

		GarbageData newGarbageData = new GarbageData()
		{
			id = newGarbageId,
			iid = iid,
			ackiid = ackiid,
			username = username,
			type = type,
			active = active,
			status = status,
			delay = delay,
			queued = queued,
			amt = amt,
			x = x,
			y = y,
			size = size,
			column = column,
			cid = cid
		};

		_manager.GameData.ImpendingDamage.Add(newGarbageData);
		
		if (cid == null)
			IncomingAttack(newGarbageData, null, null);
	}

	private int RefreshGarbages(GarbageData data, string gameid)
	{
		if (!_manager.GameData.GarbageAckNowledgements.Incoming.ContainsKey(gameid))
			_manager.GameData.GarbageAckNowledgements.Incoming.Add(gameid, null);

		_manager.GameData.GarbageAckNowledgements.Incoming[gameid] =
			Math.Max((int)data.iid, _manager.GameData.GarbageAckNowledgements.Incoming[gameid] ?? 0);

		int amt = (int)data.amt;
		if (_manager.GameData.GarbageAckNowledgements.Outgoing.ContainsKey(gameid))
		{
			List<GarbageData> list = new List<GarbageData>();

			foreach (var garbage in _manager.GameData.GarbageAckNowledgements.Outgoing[gameid])
			{
				if (garbage.iid <= data.ackiid)
					continue;

				var minAmt = Math.Min((int)garbage.amt, amt);
				garbage.amt -= minAmt;
				amt -= minAmt;
				if (garbage.amt > 0)
					list.Add(garbage);
			}

			_manager.GameData.GarbageAckNowledgements.Outgoing[gameid] = list;
		}

		return amt;
	}

	public void FightLines(int attackLines)
	{
		int newGarbage = 0;
		if (_manager.GameData.Options.GarbageTargetBonus == GarbageTargetBonusType.Countering)
			newGarbage += (int)(_manager.GameData.GarbageBonus * _manager.GameData.Options.GarbageMultiplier);

		//int i = 0;
		for (; (attackLines > 0 || newGarbage > 0) && _manager.GameData.ImpendingDamage.Count > 0;)
		{
			_manager.GameData.ImpendingDamage[0].amt--;
			if (_manager.GameData.ImpendingDamage[0].amt == 0)
			{
				var shifted = _manager.GameData.ImpendingDamage[0];
				_manager.GameData.ImpendingDamage.RemoveAt(0);
				if (shifted.queued == true)
					ProcessGarbageStatus();
			}

			if (attackLines != 0)
				attackLines--;
			else newGarbage--;
		}


		if (attackLines > 0)
			Offence(attackLines);
	}

	public void Offence(int attackLines)
	{
		if (attackLines < 1)
			return;

		var targets = _manager.TargetInfo.GetTargets();
		foreach (var target in targets)
		{
			var iid = ++_manager.GameData.InteractionId;
			if (_manager.GameData.Options.Passthrough is PassthroughType.Zero or PassthroughType.Consistent)
			{
				if (!_manager.GameData.GarbageAckNowledgements.Outgoing.ContainsKey(target))
					_manager.GameData.GarbageAckNowledgements.Outgoing.Add(target, new List<GarbageData>());

				_manager.GameData.GarbageAckNowledgements.Outgoing[target].Add(
					new GarbageData()
					{
						iid = iid,
						amt = attackLines
					}
				);
			}
		}
	}

	public void IncomingAttack(GarbageData data, string? target, int? targetCid)
	{
		if (_manager.GameData.Options.Passthrough == PassthroughType.Consistent && target != null)
			data.amt = CounterAttack(data, target);
		else
		{
			if (targetCid != null)
			{
				data = GetGarbageDataByCid((int)targetCid);
				if (data == null)
					return;
			}
		}

		if (data.amt <= 0)
			return;

		/*
		  t.notyetreceivedattacks++,
  (t.garbagereceived += data.amt),
  (t.stats.garbage.received += data.amt),
  (t.killer.name = data.username),
  (t.killer.gameid = sender),
		 */
		_manager.WaitingFrameInfo.WaitFrames(_manager.GameData.Options.GarbageSpeed, WaitingFrameType.IncomingAttackHit,
			new GarbageData() { gid = data.id });
	}

	private GarbageData? GetGarbageDataByCid(int cid)
	{
		foreach (var garbageData in _manager.GameData.ImpendingDamage)
		{
			if (garbageData.cid == cid)
				return garbageData;
		}

		return null;
	}

	private int CounterAttack(GarbageData data, string? target)
	{
		if (!_manager.GameData.GarbageAckNowledgements.Incoming.ContainsKey(target))
			_manager.GameData.GarbageAckNowledgements.Incoming.Add(target, null);

		_manager.GameData.GarbageAckNowledgements.Incoming[target] =
			Math.Max((int)data.iid, _manager.GameData.GarbageAckNowledgements.Incoming[target]
			                        ?? 0);

		var amt = (int)data.amt;
		if (_manager.GameData.GarbageAckNowledgements.Outgoing.ContainsKey(target))
		{
			List<GarbageData> garbageList = new List<GarbageData>();
			foreach (var garbageData in _manager.GameData.GarbageAckNowledgements.Outgoing[target])
			{
				if (garbageData.iid <= data.ackiid)
					continue;

				var newamt = Math.Min((int)garbageData.amt, (int)amt);
				garbageData.amt -= newamt;
				amt -= newamt;
				if (garbageData.amt > 0) //NOTE: 参照の追加？
					garbageList.Add((GarbageData)garbageData.Clone());
			}

			_manager.GameData.GarbageAckNowledgements.Outgoing[target] = garbageList;
		}

		return amt;
	}

	public void IncomingAttackHit(int gid)
	{
		var garbage = GetGarbageDataById(gid);
		if (garbage != null)
		{
			if (_manager.GameData.Options.Shielded)
			{
				_manager.GameData.ImpendingDamage = _manager.GameData.ImpendingDamage.Where(x => x.id != gid).ToList();
				return;
			}

			garbage.active = true;
			ProcessGarbageStatus(garbage.id);
		}
	}

	public void OutgoingAttack()
	{
		//none
	}

	public void OutgoingAttackHit()
	{
		//none
	}

	public void ProcessGarbageStatus(int? id = null)
	{
		if (id == null && _manager.GameData.ImpendingDamage.Count == 0)
			id = null;

		if (id == null || id == 0)
			return;

		var garbageData = GetGarbageDataById((int)id);
		if (garbageData != null)
		{
			switch (garbageData.status)
			{
				case GarbageStatus.Sleeping:
					if (garbageData.queued == true)
					{
						//NOTE: 参照は同じはずなので、OK 
						if (garbageData == _manager.GameData.ImpendingDamage[0])
						{
							if (garbageData.delay >= 1)
							{
								garbageData.status =GarbageStatus.Caution;
								_manager.WaitingFrameInfo.WaitFrames((int)garbageData.delay, WaitingFrameType.ProcessGarbageStatus,
									garbageData.id);
							}
							else
								garbageData.status =GarbageStatus.Spawn;
						}
						else
						{
							garbageData.status = GarbageStatus.Caution;
							_manager.WaitingFrameInfo.WaitFrames((int)garbageData.delay, WaitingFrameType.ProcessGarbageStatus,
								garbageData.id);
						}
					}

					break;

				case GarbageStatus.Caution:
					if (garbageData.firstcycle == true)
						garbageData.status = GarbageStatus.Danger;
					_manager.WaitingFrameInfo.WaitFrames((int)garbageData.delay, WaitingFrameType.ProcessGarbageStatus,
						garbageData.id);
					break;

				case GarbageStatus.Danger:
					garbageData.status = GarbageStatus.Spawn;
					break;
			}
		}
	}

	private GarbageData? GetGarbageDataById(int id)
	{
		foreach (var data in _manager.GameData.ImpendingDamage)
		{
			if (data.id == id)
				return data;
		}

		return null;
	}

	public void TakeAllDamage(bool hasGarbageCap = false)
	{
		if (_manager.GameData.Options.Shielded || !_manager.GameData.Options.HasGarbage)
			return;

		if (_manager.GameData.GarbageAreLockedUntil > _manager.FrameInfo.CurrentFrame)
			return;

		var nnnnnn = true;
		var ooooo = hasGarbageCap ? 400 : GetGarbageCap();
		//	var iiii = false;
		var rrrr = false;

		List<GarbageData> list = new List<GarbageData>();
		for (int eIndex = _manager.GameData.ImpendingDamage.Count - 1; eIndex >= 0; eIndex--)
		{
			var garbage = _manager.GameData.ImpendingDamage[eIndex];
			if (!((bool)garbage.active && garbage.status == GarbageStatus.Spawn))
			{
				list.Insert(0, garbage);
			//	_manager.GameData.ImpendingDamage[eIndex] = garbage;
			_manager.GameData.ImpendingDamage.RemoveAt(eIndex);
			}
		}


		for (int sIndex = 0; sIndex < ooooo && _manager.GameData.ImpendingDamage.Count != 0; sIndex++)
		{
			var garbage = _manager.GameData.ImpendingDamage[0];
			garbage.amt--;
			_manager.GameData.LastReceivedCount++;
			var column = garbage.column;
			var size = garbage.size;
			//	var edges
			//iiii = true;
			switch (_manager.GameData.Options.GarbageEntry)
			{
				case GarbageEntryType.PieceAre:
					_manager.WaitingFrameInfo.WaitFrames(
						_manager.GameData.Options.GarbageAre * (sIndex + 1),
						WaitingFrameType.PushGarbageLine,
						new GarbageData()
						{
							column = column,
							size = size
						}
					);
					break;

				case GarbageEntryType.Are:
					var a = _manager.GameData.Options.GarbageAre * (sIndex + 1);
					_manager.GameData.GarbageAreLockedUntil = _manager.FrameInfo.CurrentFrame + a;
					_manager.WaitingFrameInfo.WaitFrames(a, WaitingFrameType.PushGarbageLine,
						new GarbageData()
						{
							column = column,
							size = size
						});
					break;

				default: //instant
					_manager.BoardInfo.AreWeToppedYet();
					_manager.BoardInfo.PushGarbageLine((int)column, (int)size);
					break;
			}

			nnnnnn = false;
			if (garbage.amt == 0)
			{
				_manager.GameData.ImpendingDamage.RemoveAt(0);
				rrrr = true;
				nnnnnn = true;
			}
		}

		_manager.GameData.ImpendingDamage.AddRange(list.ToArray());
		if (rrrr && _manager.GameData.ImpendingDamage.Count > 0 && (bool)_manager.GameData.ImpendingDamage[0].queued)
			ProcessGarbageStatus();
	}

	public void AnnounceOffensive()
	{
	}
}