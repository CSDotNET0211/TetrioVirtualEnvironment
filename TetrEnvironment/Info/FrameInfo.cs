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

	public void PullEvent(IReadOnlyList<Event> events)
	{
		while (_currentIndex < events.Count && events[_currentIndex].frame == CurrentFrame)
		{
			switch (events[_currentIndex].type)
			{
				case EventType.Start:
					break;

				case EventType.Full:
					_manager.GameData.Falling.DeepSleep = false;
					_manager.FallInfo.Next(null);
					break;

				case EventType.Keydown:
					var eventInput = events[_currentIndex] as EventKeyInput;
					_manager.HandleInfo.KeyDown(eventInput.data);

					break;
				case EventType.Keyup:
					eventInput = events[_currentIndex] as EventKeyInput;
					_manager.HandleInfo.KeyUp(eventInput.data);
					break;

				case EventType.Targets:
					break;

				case EventType.Ige:
					///EventIgeData igeEvent = (EventIgeData)(events[_currentIndex] as EventIge).data.Clone();
					dynamic igeEvent = events[_currentIndex] as EventIge;


					if (igeEvent.id == null)
						igeEvent = (igeEvent as EventIge).data.Clone();

					if (GarbageIDs.Contains(igeEvent.id))
						break;

					GarbageIDs.Add(igeEvent.id);

					//var igeTarget = igeEvent.data as IgeInteractionConfirm; //targetとは限らない
					var data = igeEvent.data;
					var gameId = data.gameid;

					if (data.type == "interaction" &&
					    data.data.type == "garbage")
					{
						//	var garbageData = (igeEvent as EventIge).data.data as IgeInteraction;
						_manager.GarbageInfo.AddPendingGarbage(data.data, gameId, data.cid);
					}

					if (data.type == "interaction_confirm")
					{
						switch (data.data.type)
						{
							case "garbage":
								//	var igeInteraction =  (igeEvent as EventIge).data.data as IgeInteractionConfirm;
								//	var garbage = igeInteraction.data as GarbageData;
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

					//	var aa = data.data.type;
					//	var nestData = data.data;
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

			_currentIndex++;
		}
	}

	public void DoSpecialEvents(EventIgeData igeEvent)
	{
		switch (igeEvent.type)
		{
		}
	}
}