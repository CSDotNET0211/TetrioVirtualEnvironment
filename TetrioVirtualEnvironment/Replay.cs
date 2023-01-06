using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TetrReplayLoaderLib;
using static TetrReplayLoaderLib.TetrLoader;

namespace TetrioVirtualEnvironment
{
    public class Replay
    {
        public List<Environment> Environments = new List<Environment>();
        public ReplayKind ReplayKind { get; private set; }
        public int ReplayIndex = -1;
        public int PlaybackSkipFrame=0;
        int PassedSkipFrame=0;
        public ReplayStatusEnum ReplayStatus;
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
            ReplayStatus=ReplayStatusEnum.Inited;
        }

        public void LoadGame(int replayIndex)
        {
            ReplayIndex = replayIndex;
            PlaybackSkipFrame=0;
            PassedSkipFrame=0;

            Environments.Clear();

            for (int playerIndex = 0; playerIndex < GetPlayerCount(ReplayData, ReplayKind); playerIndex++)
            {
                var full = GetReplayEvent(ReplayData, ReplayKind, playerIndex, ReplayIndex).Where(x => x.type == "full").First().data;
                var fullevent = JsonSerializer.Deserialize<EventFull>(full.ToString());
                Environments.Add(new Environment(fullevent, Environment.EnvironmentModeEnum.Seed));

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
                    Update();
                }
            }
            else
            {
                for (int i = 0; i < Environments.Count; i++)
                    Environments[i].ResetGame(Environments[i].EventFull, Environments[i].EnvironmentMode, Environments[i].InitData, Environments[i].NextSkipCount);

                //TODO: 終わりより大きかったら終わりまで、
                for (int i = 0; i < newframe; i++)
                {
                    Update();
                }
            }
        }

        public bool Update()
        {

            if(PassedSkipFrame<PlaybackSkipFrame)
            {
                PassedSkipFrame++;
                return true;
            }else
                PassedSkipFrame= 0;

            for (int playerIndex = 0; playerIndex < Environments.Count; playerIndex++)
            {
                var result = Environments[playerIndex].Update(ReplayKind, GetReplayEvent(ReplayData, ReplayKind, playerIndex, ReplayIndex));
                if (!result)
                {
                    ReplayStatus = ReplayStatusEnum.Loaded;
                    return false;
                }

            }

            return true;

        }
    }
}
