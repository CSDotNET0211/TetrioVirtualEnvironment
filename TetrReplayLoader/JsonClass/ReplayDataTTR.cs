
using TetrLoader;

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
		public  ReplayEvent? data { get; set; } = null;
		public string? back { get; set; } = null;

		public int GetGameTotalFrames(int replayIndex)
			=> -1;

		public int GetPlayerCount()
			=> 1;

		public int GetReplayCount()
			=> 1;

		public List<Event.Event>? GetReplayEvents(int playerIndex, int replayIndex)
		=> data?.events;

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
