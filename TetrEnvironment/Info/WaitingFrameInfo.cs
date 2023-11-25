using TetrEnvironment.Constants;
using TetrLoader.Ige;

namespace TetrEnvironment.Info;

public class WaitingFrameInfo
{
	private Environment _manager;

	public WaitingFrameInfo(Environment manager)
	{
		_manager = manager;
	}

	public void WaitFrames(int frame, string type, object data)
	{
		_manager.GameData.WaitingFrames.Add(new WaitingFrameData()
		{
			target = _manager.FrameInfo.CurrentFrame + frame,
			type = type,
			data = data
		});
	}

	public void ExcuteWaitingFrames()
	{
		for (int i = _manager.GameData.WaitingFrames.Count - 1; i >= 0; i--)
		{
			if (_manager.GameData.WaitingFrames[i].target == _manager.FrameInfo.CurrentFrame)
			{
				ExcuteWaitingFrame(_manager.GameData.WaitingFrames[i]);
				_manager.GameData.WaitingFrames.RemoveAt(i);
			}
		}
	}

	public void ExcuteWaitingFrame(WaitingFrameData data)
	{
		switch (data.type)
		{
			case "are":
				_manager.FallInfo.Next(null);
				break;

			case "incoming-attack-hit":
				var garbage = data.data as GarbageData;
				_manager.GarbageInfo.IncomingAttackHit((int)garbage.gid);
				break;

			case "outgoing-attack-hit":
				_manager.GarbageInfo.OutgoingAttackHit();
				break;

			case "process-garbage-status":
				_manager.GarbageInfo.ProcessGarbageStatus();
				break;

			case "push-garbage-line":
				var garbageData = data.data as GarbageData;
				_manager.BoardInfo.PushGarbageLine((int)garbageData.column, (int)garbageData.size);
				break;

			case "freeze-counters":
				break;

			case "revive-from-stock-loss":
				break;
		}
	}

	public void WaitFramesUnsafe()
	{
	}

	public void ExcuteUnsafeWaitingFrames()
	{
	}
}