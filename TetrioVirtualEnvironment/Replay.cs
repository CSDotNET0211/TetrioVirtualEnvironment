using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoaderLib;

namespace TetrioVirtualEnvironment
{
    public class Replay
    {
        public Environment Environment { get; private set; }
        ReplayDataSolo _replayData;
        public int CurrentFrame { get; private set; } = 0;
        int CurrentIndex = 0;

        public void Load(string jsondata)
        {
            Environment = new Environment();
            _replayData = (ReplayDataSolo)LibClass.ParseReplay(jsondata, false);
        }

        public void SetFrame()
        {

        }

        public bool Update()
        {
            Environment.GameData.SubFrame = 0;
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
                        case "keyup":
                            var inputevent = (EventKeyInput)_replayData.data.events[CurrentIndex].data;

                            Environment.KeyInput(_replayData.data.events[CurrentIndex].type,
                            inputevent);

                            break;

                        case "ige":
                            break;

                        case "end":
                            return false;
                        default:
                            throw new Exception("invalid key:" + _replayData.data.events[CurrentIndex].type);
                    }

                    CurrentIndex++;
                }
                else break;
            }



            CurrentFrame++;
            Environment.ProcessShift(false, 1 - Environment.GameData.SubFrame);
            Environment.FallEvent(null, 1 - Environment.GameData.SubFrame);


            return true;
        }


    }
}
