using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass.Event;
using static TetrReplayLoader.TetrLoader;

namespace TetrioVirtualEnvironment
{
    public class Replay
    {
        public List<Environment> Environments = new List<Environment>();

        public ReplayKind ReplayKind { get; private set; }
        public int ReplayIndex = -1;
        public int PlaybackSkipFrame = 0;
        int PassedSkipFrame = 0;
        ReplayStatusEnum _replayStatus;
        public event EventHandler? ReplayStatusChanged = null;
        public ReplayStatusEnum ReplayStatus
        {
            get
            {
                return _replayStatus;
            }
            set
            {
                _replayStatus = value;
                if (ReplayStatusChanged != null)
                    ReplayStatusChanged(this, EventArgs.Empty);
            }
        }
        public enum ReplayStatusEnum
        {

            Inited,
            Loaded,
            Playing,
            Paused,

        }

        public int GameFrames { get; private set; }


        public object ReplayData { get; private set; }

        public Replay(string jsondata)
        {
            var ismulti = IsMulti(jsondata);
            ReplayKind = ismulti ? ReplayKind.TTRM : ReplayKind.TTR;

            ReplayData = ParseReplay(jsondata, ReplayKind);
            ReplayStatus = ReplayStatusEnum.Inited;
        }

        public void LoadGame(int replayIndex)
        {
            ReplayIndex = replayIndex;
            PlaybackSkipFrame = 0;
            PassedSkipFrame = 0;

            Environments.Clear();

            for (int playerIndex = 0; playerIndex < GetPlayerCount(ReplayData, ReplayKind); playerIndex++)
            {
                var events = GetReplayEvent(ReplayData, ReplayKind, playerIndex, ReplayIndex);
                string full = null;
                foreach (Event e in events)
                {
                    if (e.type == "full")
                    {
                        full = e.data.ToString();
                        break;
                    }
                }

                var fullevent = JsonSerializer.Deserialize<EventFull>(full.ToString());

                Environments.Add(new Environment(fullevent, Environment.EnvironmentModeEnum.Seed, fullevent.options.username));

            }

            ReplayStatus = ReplayStatusEnum.Playing;
            GameFrames = GetGameFrames(ReplayData, ReplayKind, replayIndex);
        }


        public void SkipFrame(int newframe)
        {
            if (Environments[0].CurrentFrame <= newframe)
            {
                var frameDiff = newframe - Environments[0].CurrentFrame;
                for (int i = 0; i < frameDiff; i++)
                {
                    Update(true);
                }
            }
            else
            {
                for (int i = 0; i < Environments.Count; i++)
                    Environments[i].ResetGame(Environments[i].EventFull, Environments[i].EnvironmentMode, Environments[i].InitData, Environments[i].NextSkipCount);

                //TODO: 終わりより大きかったら終わりまで、
                for (int i = 0; i < newframe; i++)
                {
                    Update(true);
                }
            }
        }

        public bool Update(bool ignoreSkipFrame = false)
        {
            int updateTime = 1;

            if (!ignoreSkipFrame)
            {
                if (PlaybackSkipFrame < 0)
                {
                    updateTime -= PlaybackSkipFrame;
                }
                else
                {
                    if (PassedSkipFrame < PlaybackSkipFrame)
                    {
                        PassedSkipFrame++;
                        return true;
                    }
                    else
                        PassedSkipFrame = 0;
                }



            }

            for (int i = 0; i < updateTime; i++)
            {
                for (int playerIndex = 0; playerIndex < Environments.Count; playerIndex++)
                {
                    var result = Environments[playerIndex].Update(ReplayKind, GetReplayEvent(ReplayData, ReplayKind, playerIndex, ReplayIndex));
                    if (!result)
                    {
                        ReplayStatus = ReplayStatusEnum.Loaded;
                        return false;
                    }

                }
            }

            return true;

        }


    }
}
