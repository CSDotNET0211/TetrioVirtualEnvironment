using System.Text.Json;
using TetrLoader;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass.Event;
using static TetrReplayLoader.ReplayLoader;

namespace TetrioVirtualEnvironment
{
	public class Replay
	{
		public List<Environment> Environments { get; } = new();
		public ReplayKind ReplayKind { get; }
		public int ReplayIndex { get; private set; } = -1;


		private ReplayStatusKind _replayStatus;
		public event EventHandler? ReplayStatusChanged;
		private List<List<Event>>? _events = null;

		public ReplayStatusKind ReplayStatus
		{
			get => _replayStatus;
			set
			{
				_replayStatus = value;
				ReplayStatusChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		///TODO: End Flag
		/// <summary>
		/// Initialized		Replay is initialized by contructer.
		/// Loaded			One game is loaded.
		/// Playing			Replay is playing. Game update will be called.
		/// Paused			Replay is paused. Game update will not be called.
		/// </summary>		
		public enum ReplayStatusKind
		{
			Initialized,
			Loaded,
			Playing,
			Paused,

		}
		public int TotalGameFramesAtLoadedGame { get; private set; }
		public IReplayData ReplayData { get; }

		public Replay(string jsondata)
		{
			var ismulti = IsMulti(jsondata);
			ReplayKind = ismulti ? ReplayKind.TTRM : ReplayKind.TTR;

			ReplayData = ParseReplay(jsondata, ReplayKind);
			ReplayStatus = ReplayStatusKind.Initialized;


		}

		/// <summary>
		/// Load selected after laoded the game.
		/// </summary>
		/// <param name="replayIndex"></param>
		/// <exception cref="Exception"></exception>
		public void LoadGame(int replayIndex)
		{
			ReplayIndex = replayIndex;
			Environments.Clear();
			_events = new List<List<Event>>();

			var playerCount = ReplayData.GetPlayerCount();

			if (playerCount > 2)
				throw new Exception("more than 3 players are not supported.");

			for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
			{
				_events.Add(ReplayData.GetReplayEvents(playerIndex, ReplayIndex));

				EventFull? fullEvent = null;

				foreach (var @event in _events[_events.Count - 1])
				{
					if (@event.type == "full")
					{
						fullEvent = @event as EventFull;
						break;
					}
				}

				Environments.Add(new Environment(fullEvent.data, Environment.NextGenerateKind.Seed, fullEvent.data.options?.username));
				ReplayData.GetReplayEvents(playerIndex, ReplayIndex);
			}

			ReplayStatus = ReplayStatusKind.Playing;
			TotalGameFramesAtLoadedGame = ReplayData.GetGameTotalFrames(replayIndex);
		}

		/// <summary>
		/// Jump the frame selected.
		/// </summary>
		/// <param name="newframe">Abusolute position of frame to jump.</param>
		public void JumpFrame(int newframe)
		{
			if (Environments[0].CurrentFrame <= newframe)
			{
				var frameDiff = newframe - Environments[0].CurrentFrame;
				for (int i = 0; i < frameDiff; i++)
				{
					Update();
				}
			}
			else
			{
				//use checkpoint to back replay faster
				foreach (var env in Environments)
					env.ResetGame(env.EventFull, env.NextGenerateMode, env.DataForInitialize, env.NextSkipCount);

				for (int i = 0; i < newframe; i++)
					Update();

			}
		}

		/// <summary>
		/// Update individual environment in replay.
		/// </summary>
		/// <returns></returns>
		public bool Update()
		{

			for (int playerIndex = 0; playerIndex < Environments.Count; playerIndex++)
			{
				var gameUpdateSuccess = Environments[playerIndex].Update(_events[playerIndex]);
				if (gameUpdateSuccess) continue;
				//update failed because of game ended.
				//I should be the State ended.
				ReplayStatus = ReplayStatusKind.Loaded;
				return false;


			}

			return true;

		}


	}
}
