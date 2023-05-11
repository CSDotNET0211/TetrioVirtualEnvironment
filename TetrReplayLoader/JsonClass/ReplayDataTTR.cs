
using System.Text.Json;
using TetrLoader;
using TetrReplayLoader.JsonClass.Event;

namespace TetrReplayLoader.JsonClass
{
	/// <summary>
	/// リプレイファイルのデータ
	/// </summary>
	public class ReplayDataTTR : IReplayData
	{
		public User? user { get; set; } = null;
		//TTR
		public string? _id { get; set; } = null;
		public string? shortid { get; set; } = null;
		public bool ismulti { get; set; } = false;
		public EndContext? endcontext { get; set; } = null;

		public string? ts { get; set; } = null;
		public string? gametype { get; set; } = null;
		public bool? verified { get; set; } = null;
		/// <summary>
		/// 試合ごとのデータ群
		/// </summary>
		public ReplayEvent? data { get; set; } = null;
		public string? back { get; set; } = null;

		public int GetGameTotalFrames(int replayIndex)
			=> -1;

		public int GetPlayerCount()
			=> 1;

		public int GetReplayCount()
			=> 1;

		public List<Event.Event>? GetReplayEvents(int playerIndex, int replayIndex)
		{
			var rawEvent = data?.events;
			List<Event.Event> events = new List<Event.Event>();

			foreach (var @event in rawEvent)
			{
				switch (@event.type)
				{
					case "start":
						events.Add(@event);
						break;

					case "end":
						events.Add(new EventEnd()
						{
							data = JsonSerializer.Deserialize<EventEndData>(@event.data.ToString()),
							type = @event.type,
							frame = @event.frame,
							id = @event.id,
						});
						break;

					case "full":
						events.Add(new EventFull()
						{
							data = JsonSerializer.Deserialize<EventFullData>(@event.data.ToString()),
							type = @event.type,
							frame = @event.frame,
							id = @event.id,
						});
						break;

					case "keydown":
					case "keyup":
						events.Add(new EventKeyInput()
						{
							data = JsonSerializer.Deserialize<EventKeyInputData>(@event.data.ToString()),
							type = @event.type,
							frame = @event.frame,
							id = @event.id,
						});
						break;

					case "targets":
						events.Add(new EventTargets()
						{
							data = JsonSerializer.Deserialize<EventTargetsData>(@event.data.ToString()),
							type = @event.type,
							frame = @event.frame,
							id = @event.id,
						});
						break;

					case "ige":
						events.Add(new EventIge()
						{
							data = JsonSerializer.Deserialize<EventIgeData>(@event.data.ToString()),
							type = @event.type,
							frame = @event.frame,
							id = @event.id,
						});
						break;

					default:
						events.Add(@event);
						break;

				}
			}

			return events;






		}

		public Stats GetReplayStats(int playerIndex, int replayIndex)
			=> new Stats()
			{
				PPS = -1,
				APM = -1,
				VS = -1,
				Winner = false
			};

		public string GetUsername(int playerIndex)
				=> user.username;

	}
}
