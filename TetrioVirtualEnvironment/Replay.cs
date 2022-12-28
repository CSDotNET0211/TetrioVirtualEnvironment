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
        public bool IsLoaded = false;
        public bool IsPlaying = false;
        public int GameFrames { get; private set; }


        public object ReplayData { get; private set; }

        public Replay(string jsondata)
        {
            var ismulti = IsMulti(jsondata);
            ReplayKind = ismulti ? ReplayKind.TTRM : ReplayKind.TTR;

            ReplayData = ParseReplay(jsondata, ReplayKind);
        }

        public void LoadGame(int replayIndex)
        {
            ReplayIndex = replayIndex;

            for (int playerIndex = 0; playerIndex < GetPlayerCount(ReplayData, ReplayKind); playerIndex++)
            {
                var full = GetReplayEvent(ReplayData, ReplayKind, playerIndex, ReplayIndex).Where(x => x.type == "full").First().data;
                var fullevent = JsonSerializer.Deserialize<EventFull>(full.ToString());
                Environments.Add(new Environment(fullevent, Environment.EnvironmentModeEnum.Seed));

            }

            IsLoaded = true;
            IsPlaying = true;
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
            if (!IsPlaying)
                return true; //https://dev.classmethod.jp/articles/litedb-on-net-core2/

            for (int playerIndex = 0; playerIndex < Environments.Count; playerIndex++)
            {
                var result = Environments[playerIndex].Update(ReplayKind, GetReplayEvent(ReplayData, ReplayKind, playerIndex, ReplayIndex));
                if (!result)
                {
                    IsPlaying = false;
                    return false;
                }

            }

            return true;

        }
    }
}
