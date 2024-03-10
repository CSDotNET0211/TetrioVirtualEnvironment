using TetrLoader.Enum;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment.Info;

public class FrameInfo
{
	private readonly Environment _manager;
	private List<int>? GarbageIDs;

	public FrameInfo(Environment manager)
	{
		_manager = manager;
		GarbageIDs = new List<int>();
	}

	public int CurrentFrame { get; set; }
	public int _currentIndex;
	internal int CurerntIndex => _currentIndex;

	public void PullEvents(IReadOnlyList<Event> events)
	{
		while (_currentIndex < events.Count && events[_currentIndex].frame == CurrentFrame)
		{
			PullEvent(events[_currentIndex]);
			_currentIndex++;
		}
	}

	public void PullEvent(Event @event)
	{
		switch (@event.type)
		{
			case EventType.Start:
				break;

			case EventType.Full:
				_manager.GameData.Falling.DeepSleep = false;
				_manager.FallInfo.Next(null);
				break;

			case EventType.Keydown:
				var eventInput = @event as EventKeyInput;
				_manager.HandleInfo.KeyDown(eventInput.data);

				break;
			case EventType.Keyup:
				eventInput = @event as EventKeyInput;
				_manager.HandleInfo.KeyUp(eventInput.data);
				break;

			case EventType.Ige:
				dynamic igeEvent = @event as EventIge;


				if (igeEvent.id == null)
					igeEvent = (igeEvent as EventIge).data.Clone();

				if (GarbageIDs.Contains(igeEvent.id))
					break;

				GarbageIDs.Add(igeEvent.id);
				
				var data = igeEvent.data;
				var gameId = data.gameid;

				if (data.type == "interaction" &&
				    data.data.type == "garbage")
				{
					_manager.GarbageInfo.AddPendingGarbage(data.data, gameId, data.cid);
				}

				if (data.type == "interaction_confirm")
				{
					switch (data.data.type)
					{
						case "garbage":
							//_manager.GarbageInfo.AddPendingGarbage(data.data, gameId, data.cid);
							_manager.GarbageInfo.IncomingAttack(data.data, gameId, data.cid);
							break;

						case "targeted":
							//		IgeAllowTargeting allowTargeting =   (igeEvent as EventIge).data.data as IgeAllowTargeting;
							if (data.data.value)
							{
								//AddEnemy
							}
							else
							{
								//RemoveEnemy
							}

							break;
					}
				}
				
				if (data.type == "allow_targeting")
				{
					//SetEnabled
				}
				else if (data.type == "target")
				{
					//	var targetData = data.data as IgeTarget;
					_manager.TargetInfo.SetTargets((data.targets as string[]).ToList());
				}
				else if (data.type == "kev")
				{
					//	var igeKev = igeEvent.data as IgeKev;
					//RemovePlayer
					/*(e.tm.RemovePlayer(sssJson.victim.gameid),
					sssJson.killer.gameid == e.id &&
						(e.fim.AddFire(150 + (sssJson.fire / 700) * 200),
							t.stats.kills++))*/
				}

				break;

			case EventType.End:
				break;

			case EventType.Exit:
				break;
		}
	}

	public void DoSpecialEvents(EventIgeData igeEvent)
	{
		throw new NotImplementedException();
		switch (igeEvent.type)
		{
		}
	}
}