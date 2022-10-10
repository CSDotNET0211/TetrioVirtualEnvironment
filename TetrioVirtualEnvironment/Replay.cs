using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoaderLib;

namespace TetrioVirtualEnvironment
{
    internal class Replay
    {
        Environment _environment;
        ReplayDataSolo _replayData;
        int CurrentFrame = 0;
        int CurrentIndex = 0;

        public void Load(string jsondata)
        {
            _environment = new Environment();
            _replayData = (ReplayDataSolo)LibClass.ParseReplay(jsondata, false);
        }

        public void SetFrame()
        {

        }

        public bool Update()
        {
            _environment.GameData.SubFrame = 0;
            //キー入力
            while (true)
            {
                if (_replayData.data.events[CurrentIndex].frame == CurrentFrame)
                {
                    switch (_replayData.data.events[CurrentIndex].type)
                    {
                        case "start":
                            continue;

                        case "full":

                            break;

                        case "keydown":

                            break;

                        case "keyup":
                            break;

                        case "ige":
                            break;


                    }

                    var @event = (EventKeyInput)_replayData.data.events[CurrentIndex].data;

                    _environment.KeyInput()
                    CurrentIndex++;
                }
            }
            CurrentFrame++;
            _environment.ProcessShift(false, 1 - _environment.GameData.SubFrame);
            _environment.FallEvent(null,1-_environment.GameData.SubFrame);


        }



    }
}
