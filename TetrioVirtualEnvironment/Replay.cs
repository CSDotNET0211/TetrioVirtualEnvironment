using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TetrReplayLoaderLib;

namespace TetrioVirtualEnvironment
{
    public class Replay
    {
        public List<Environment> Environments = new List<Environment>();



        ReplayDataTTR _replayDataTTR;
        ReplayDataTTRM _replayDataTTRM;

        //   List<int> CurrentIndex = new List<int>();

        bool is_ttr;
     public   readonly int PlayerCount;

        public Replay(string jsondata, bool is_ttr)
        {
            this.is_ttr = is_ttr;

            if (is_ttr)
            {
                _replayDataTTR = (ReplayDataTTR)LibClass.ParseReplay(jsondata, is_ttr);


                var full = _replayDataTTR.data.events.Where(x => x.type == "full").First().data;
                var options = JsonSerializer.Deserialize<EventFull>(full.ToString()).options;
                Environments.Add(new Environment(options,Environment.EnvironmentModeEnum.Seed));
            }
            else
            {
                _replayDataTTRM = (ReplayDataTTRM)LibClass.ParseReplay(jsondata, is_ttr);


                for (int i = 0; i < _replayDataTTRM.data[0].board.Count; i++)
                {
                    var full = _replayDataTTRM.data[0].replays[i].events.Where(x => x.type == "full").First().data;
                    var options = JsonSerializer.Deserialize<EventFull>(full.ToString()).options;
                    Environments.Add(new Environment(options,Environment.EnvironmentModeEnum.Seed));

                }
            }

            PlayerCount=Environments.Count;
        }

        public bool Update()
        {
            for (int playerIndex = 0; playerIndex < Environments.Count; playerIndex++)
            {
                if (is_ttr)
                {
                    Environments[playerIndex].Update(is_ttr, _replayDataTTR.data.events);
                }
                else
                {
                    Environments[playerIndex].Update(is_ttr, _replayDataTTRM.data[0].replays[playerIndex].events);
                }
            }

            return true;

        }
    }
}
