using TetrLoader.Ige;

namespace TetrEnvironment.Info;

public class WaitingFrameInfo
{
	private Environment _manager;

	public WaitingFrameInfo(Environment manager)
	{
		_manager = manager;
	}

	public void WaitFrames(int frame, GarbageInfo.WaitingFrameType type, object data)
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
			case GarbageInfo.WaitingFrameType.Are:
				_manager.FallInfo.Next(null);
				break;

			case GarbageInfo.WaitingFrameType.IncomingAttackHit:
				var garbage = data.data as GarbageData;
				_manager.GarbageInfo.IncomingAttackHit((int)garbage.gid);
				break;

			case GarbageInfo.WaitingFrameType.OutgoingAttackHit:
				_manager.GarbageInfo.OutgoingAttackHit();
				break;

			case GarbageInfo.WaitingFrameType.ProcessGarbageStatus:
				_manager.GarbageInfo.ProcessGarbageStatus();
				break;

			case GarbageInfo.WaitingFrameType.PushGarbageLine:
				var garbageData = data.data as GarbageData;
				_manager.BoardInfo.PushGarbageLine((int)garbageData.column, (int)garbageData.size);
				break;

			case GarbageInfo.WaitingFrameType.FreezeCounters:
				break;

			case GarbageInfo.WaitingFrameType.ReviveFromStackLoss:
				break;
			default: throw new Exception("unknown");
		}
	}

	public void WaitFramesUnsafe()
	{
	}

	public void ExcuteUnsafeWaitingFrames()
	{
	}
}