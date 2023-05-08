using System.Text.Json;
using TetrLoader;
using TetrReplayLoader.JsonClass;
using TetrReplayLoader.JsonClass.Event;

namespace TetrReplayLoader
{

	public class ReplayLoader
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="jsonString">raw json replay string</param>
		/// <returns></returns>
		public static bool IsMulti(string jsonString)
		{
			IsMulti? replay = JsonSerializer.Deserialize<IsMulti>(jsonString);

			if (replay?.ismulti == null || replay.ismulti == false)
				return false;
			else
				return true;
		}


		/// <summary>
		/// Parse json replay data
		/// </summary>
		/// <param name="jsonString"></param>
		/// <param name="replayKind"></param>
		/// <returns>Parsed replay object. Cast ReplayDataTTR or ReplayDataTTRM to use</returns>
		/// <exception cref="Exception"></exception>
		public static IReplayData ParseReplay(string jsonString, ReplayKind replayKind)
		{
			IReplayData replay;
			if (replayKind == ReplayKind.TTR)
				replay = JsonSerializer.Deserialize<ReplayDataTTR>(jsonString);
			else
				replay = JsonSerializer.Deserialize<ReplayDataTTRM>(jsonString);
				
			if (replay== null)
				throw new Exception("Failed to Convert Json File.\r\n" +
					"Supported Json File is TETR.IO Replay(.ttr | .ttrm) Only.");

			return replay;
		}

	}






	/// <summary>
	/// 試合中のデータ、プレイヤーの数分だけリストの個数がある
	/// </summary>
	public class PlayDataTTRM
	{
		/// <summary>
		/// プレイヤーの情報に関するデータ
		/// </summary>
		public List<Board>? board { get; set; } = null;
		/// <summary>
		/// リプレイの操作に関するデータ
		/// </summary>
		public List<ReplayEvent>? replays { get; set; } = null;
	}


	public class Board
	{
		public User? user { get; set; } = null;
		public bool? active { get; set; } = null;
		public bool? success { get; set; } = null;
		public int? winning { get; set; } = null;
	}

	/// <summary>
	/// 試合の入力データ
	/// </summary>
	public class ReplayEvent
	{
		/// <summary>
		/// 総フレーム数
		/// </summary>
		public int? frames { get; set; } = null;
		public List<Event>? events { get; set; } = null;
	}




	public class Export
	{
		public bool? successful { get; set; } = null;
		public string? gameoverreason { get; set; } = null;
		public EventFullReplayData? replay { get; set; } = null;
		public EventFullSourceData? source { get; set; } = null;
		public EventFullOptionsData? options { get; set; } = null;
		public EventFullStatsData? stats { get; set; } = null;
		// public EventTargets? targets { get; set; } = null;
		public int? fire { get; set; } = null;
		public EventFullGameData? game { get; set; } = null;
		public EventKillerData? killer { get; set; } = null;
		public AggregateStats? aggregatestats { get; set; } = null;
	}

	public class AggregateStats
	{
		public double? apm { get; set; } = null;
		public double? pps { get; set; } = null;
		public double? vsscore { get; set; } = null;

	}













}