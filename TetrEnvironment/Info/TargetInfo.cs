namespace TetrEnvironment.Info;

public class TargetInfo
{
	private Environment _manager;

	public TargetInfo(Environment manager)
	{
		_manager = manager;
	}

	public List<string> GetTargets()
	{
		return _manager.GameData.Targets;
	}

	public void SetTargets(List<string> targetIds)
	{
		var list = new List<string>();
		foreach (var target in _manager.GameData.Targets)
		{
			if (targetIds.Contains(target))
				list.Add(target);
				//	targetIds.Add(target);
			else
			{
				//this.self.elm.emit("target", target, !1);
			}
		}

		foreach (var target in list)
		{
			if (!targetIds.Contains(target))
			{
				//this.self.elm.emit("target", target, !0);
			}
		}

//NOTE: targetIdsをcloneする？
		_manager.GameData.Targets = targetIds;

		if (_manager.GameData.Targets.Count == 0)
		{
			//UpdateStrategy();
		}
	}

	public void SetTargeted()
	{
		throw new NotImplementedException();
	}

	public void SetEnemy()
	{
		throw new NotImplementedException();
	}
}