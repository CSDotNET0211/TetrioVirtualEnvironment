using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TetrLoader;
using TetrReplayLoader.JsonClass.Event;

namespace TetrReplayLoader.JsonClass
{
	/// <summary>
	/// リプレイファイルのデータ
	/// </summary>
	public class ReplayDataTTRM : IReplayData
	{
		public string? _id { get; set; } = null;
		public string? shortid { get; set; } = null;
		public bool ismulti { get; set; } = false;
		public List<EndContext>? endcontext { get; set; } = null;

		public string? ts { get; set; } = null;
		public string? gametype { get; set; } = null;
		public bool? verified { get; set; } = null;
		/// <summary>
		/// 試合ごとのデータ群
		/// </summary>
		public List<PlayDataTTRM>? data { get; set; } = null;
		public string? back { get; set; } = null;

		public int GetGameTotalFrames(int replayIndex)
		{
			int maxFrame = -1;

			foreach (var game in data[replayIndex].replays)
			{
				if (maxFrame < game.frames)
					maxFrame = (int)game.frames;
			}

			return maxFrame;
		}


		public int GetPlayerCount()
		=> data[0].replays.Count;


		public int GetReplayCount()
		=> data.Count;

		public List<Event.Event>? GetReplayEvents(int playerIndex, int replayIndex)
		{
			var rawEvent = data?[replayIndex].replays?[playerIndex].events;
			List<Event.Event> events = new List<Event.Event>();

			foreach (var @event in rawEvent)
			{
				if (@event == null)
					throw new Exception();

				switch (@event.type)
				{
					case "start":
						events.Add(@event);
						break;

					case "end":
						events.Add(new EventEnd
						(
							@event.id,
							(int)@event.frame,
							@event.type,
							JsonSerializer.Deserialize<EventEndData>(@event.data.ToString())
						));
						break;

					case "full":
						events.Add(new EventFull
						(
							@event.id,
							(int)@event.frame,
							@event.type,
							 JsonSerializer.Deserialize<EventFullData>(@event.data.ToString())
						));
						break;

					case "keydown":
					case "keyup":
						events.Add(new EventKeyInput
						(
							@event.id,
							(int)@event.frame,
							@event.type,
							JsonSerializer.Deserialize<EventKeyInputData>(@event.data.ToString())
						));
						break;

					case "targets":
						events.Add(new EventTargets(
							@event.id,
							 (int)@event.frame,
							 @event.type,
							 JsonSerializer.Deserialize<EventTargetsData>(@event.data.ToString())
						));
						break;

					case "ige":
						events.Add(new EventIge(
							@event.id,
							(int)@event.frame,
							@event.type,
							JsonSerializer.Deserialize<EventIgeData?>(@event.data.ToString())
						));
						break;

					default:
						events.Add(@event);
						break;

				}
			}

			return events;

		}

		public Stats GetReplayStats(int playerIndex, int replayIndex)
		{
			var result = new Stats();
			var events = GetReplayEvents(playerIndex, replayIndex);
			EventEnd eventEnd = null;
			for (int i = events.Count - 1; i >= 0; i--)
			{
				if (events[i].type == "end")
				{
					eventEnd = events[i] as EventEnd;
					break;
				}
			}

			result.VS = eventEnd.data.export.aggregatestats.vsscore ?? -1;
			result.APM = eventEnd.data.export.aggregatestats.apm ?? -1;
			result.PPS = eventEnd.data.export.aggregatestats.pps ?? -1;

			var frames = GetGameTotalFrames(replayIndex);
			int time = frames / 60;
			result.Time = (time / 60).ToString() + ":" + (time % 60).ToString("00");

			if (data[replayIndex].board[playerIndex].success == null)
			{
				result.Winner = false;
			}
			else
			{
				if (GetUsername(playerIndex) == data[replayIndex].board[playerIndex].user.username)
					result.Winner = (bool)data[replayIndex].board[playerIndex].success;
				else
					result.Winner = !(bool)data[replayIndex].board[playerIndex].success;

			}

			return result;
		}

		public string GetUsername(int playerIndex)
		=> endcontext[0].naturalorder == playerIndex ?
			endcontext[0].user.username :
			endcontext[1].user.username;
	}

	public class ReplayDataTTRMData
	{


	}

}
