using System.Text.Json;
using TetrReplayLoader.JsonClass;
using TetrReplayLoader.JsonClass.Event;

namespace TetrReplayLoader
{

    public class TetrLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonString">raw json replay string</param>
        /// <returns></returns>
        public static bool IsMulti(string jsonString)
        {
            IsMulti? replay = JsonSerializer.Deserialize<IsMulti>(jsonString);

            if (replay == null || replay.ismulti == null || replay.ismulti == false)
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
        public static object ParseReplay(string jsonString, ReplayKind replayKind)
        {
            object? returnobj = null;

            if (replayKind == ReplayKind.TTR)
            {
                ReplayDataTTR? replay = JsonSerializer.Deserialize<ReplayDataTTR>(jsonString);
                returnobj = replay;
            }
            else
            {
                ReplayDataTTRM? replay = JsonSerializer.Deserialize<ReplayDataTTRM>(jsonString);
                returnobj = replay;
            }

            if (returnobj == null)
                throw new Exception("Failed to Convert Json File.\r\n" +
                    "Supported Json File is TETR.IO Replay(.ttr | .ttrm) Only.");

            return returnobj;
        }

        /// <summary>
        /// Get event list from parsed replay data.
        /// </summary>
        /// <param name="replayData">Parsed Replay Object</param>
        /// <param name="replayKind"></param>
        /// <param name="playerIndex">if ReplayKind is TTR, this value will be 0</param>
        /// <param name="replayIndex">if ReplayKind is TTR, this value will be 0</param>
        /// <returns>EventList</returns>
        public static List<Event>? GetReplayEvent(object replayData, ReplayKind replayKind, int playerIndex, int replayIndex)
        {
            if (replayKind == ReplayKind.TTR)
                return ((ReplayDataTTR)replayData).data.events;
            else
                return ((ReplayDataTTRM)replayData).data[replayIndex].replays[playerIndex].events;

        }

        /// <summary>
        /// Get player count of the game.
        /// There must be at least one match.
        /// </summary>
        /// <param name="replaydata"></param>
        /// <param name="replayKind"></param>
        /// <returns>Player count</returns>
        public static int GetPlayerCount(object replaydata, ReplayKind replayKind)
        {
            if (replayKind == ReplayKind.TTR)
                return 1;
            else
                return ((ReplayDataTTRM)replaydata).data[0].replays.Count;

        }

        /// <summary>
        /// Get the game count.
        /// </summary>
        /// <param name="replaydata"></param>
        /// <param name="replayKind"></param>
        /// <returns></returns>
        public static int GetReplayCount(object replaydata, ReplayKind replayKind)
        {
            if (replayKind == ReplayKind.TTR)
                return 1;
            else
                return ((ReplayDataTTRM)replaydata).data.Count;
        }

        /// <summary>
        /// Statistics per game.
        /// </summary>
        /// <param name="replayData"></param>
        /// <param name="replayKind"></param>
        /// <param name="playerIndex"></param>
        /// <param name="replayIndex"></param>
        /// <returns></returns>
        public static Stats GetReplayStats(object replayData, ReplayKind replayKind, int playerIndex, int replayIndex)
        {
            var result = new Stats();
            if (replayKind == ReplayKind.TTR)
            {
                //DOTO: NotImplemented 
                var ttr = (ReplayDataTTR)replayData;
                result.PPS = -1;
                result.APM = -1;
                result.VS = -1;
                result.Winner = false;
            }
            else
            {
                var ttrm = (ReplayDataTTRM)replayData;

                var events = GetReplayEvent(replayData, replayKind, playerIndex, replayIndex);
                EventEnd eventEnd = null;
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    if (events[i].type == "end")
                    {
                        eventEnd = JsonSerializer.Deserialize<EventEnd>(events[i].data.ToString());
                        break;
                    }
                }

                result.VS = eventEnd.export.aggregatestats.vsscore == null ? -1 : (double)eventEnd.export.aggregatestats.vsscore;
                result.APM = eventEnd.export.aggregatestats.apm == null ? -1 : (double)eventEnd.export.aggregatestats.apm;
                result.PPS = eventEnd.export.aggregatestats.pps == null ? -1 : (double)eventEnd.export.aggregatestats.pps;

                var frames = GetGameFrames(replayData, replayKind, replayIndex);
                int time = frames / 60;
                result.Time = (time / 60).ToString() + ":" + (time % 60).ToString("00");

                if (ttrm.data[replayIndex].board[playerIndex].success == null)
                {
                    result.Winner = false;
                }
                else
                {
                    if (GetUsername(replayData, replayKind, playerIndex) == ttrm.data[replayIndex].board[playerIndex].user.username)
                        result.Winner = (bool)ttrm.data[replayIndex].board[playerIndex].success;
                    else
                        result.Winner = !(bool)ttrm.data[replayIndex].board[playerIndex].success;

                }

            }

            return result;
        }

        /// <summary>
        /// Get total frame of the game.
        /// </summary>
        /// <param name="replaydata"></param>
        /// <param name="replayKind"></param>
        /// <param name="replayIndex"></param>
        /// <returns>total frame</returns>
        public static int GetGameFrames(object replaydata, ReplayKind replayKind, int replayIndex)
        {
            //NotImplemented
            if (replayKind == ReplayKind.TTR)
                return -1;
            else
                return (int)((ReplayDataTTRM)replaydata).data[replayIndex].replays[0].frames;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replaydata"></param>
        /// <param name="replayKind"></param>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public static string GetUsername(object replaydata, ReplayKind replayKind, int playerIndex)
        {
            //NotImplemented
            if (replayKind == ReplayKind.TTR)
            {
                var data = replaydata as ReplayDataTTR;
                return "";
            }
            else
            {
                var data = replaydata as ReplayDataTTRM;
                var name = data.endcontext[0].naturalorder == playerIndex ? data.endcontext[0].user.username : data.endcontext[1].user.username;
                return name;
            }
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
        public EventFullReplay? replay { get; set; } = null;
        public EventFullSource? source { get; set; } = null;
        public EventFullOptions? options { get; set; } = null;
        public EventFullStats? stats { get; set; } = null;
        // public EventTargets? targets { get; set; } = null;
        public int? fire { get; set; } = null;
        public EventFullGame? game { get; set; } = null;
        public EventKiller? killer { get; set; } = null;
        public AggregateStats? aggregatestats { get; set; } = null;
    }

    public class AggregateStats
    {
        public double? apm { get; set; } = null;
        public double? pps { get; set; } = null;
        public double? vsscore { get; set; } = null;

    }








    
  
    


}