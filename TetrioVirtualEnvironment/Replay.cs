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
        public List<Environment> Environments=new List<Environment>();
        //  public GameData GameData { get; private set; }
        ReplayDataTTR _replayData;
        public int CurrentFrame { get; private set; } = 0;
        int CurrentIndex = 0;

        public Replay(string jsondata)
        {


            _replayData = (ReplayDataTTR)LibClass.ParseReplay(jsondata, true);


            var full = _replayData.data.events.Where(x => x.type == "full").First().data;
            var options = JsonSerializer.Deserialize<EventFull>(full.ToString()).options;
            Environments.Add(new Environment(options));
            //Environments[0].
        }


        public void SetFrame(int frame)
        {
            CurrentFrame = frame;
        }

        public bool Update()
        {
            Environments[0].GameDataInstance.SubFrame = 0;

            //キー入力
            while (true)
            {


                if (_replayData.data.events[CurrentIndex].frame == CurrentFrame)
                {

                    switch (_replayData.data.events[CurrentIndex].type)
                    {
                        case "start":
                            break;

                        case "full":
                            Environments[0].GameDataInstance.Falling.Init(null);
                            break;

                        case "keydown":
                        case "keyup":
                            var inputevent = JsonSerializer.Deserialize<EventKeyInput>(_replayData.data.events[CurrentIndex].data.ToString());

                            Console.CursorLeft = 55;
                            Console.CursorTop = 10;
                            Console.WriteLine(_replayData.data.events[CurrentIndex].frame + " " +
                              _replayData.data.events[CurrentIndex].type + " " + inputevent.key);


                            Environments[0].KeyInput(_replayData.data.events[CurrentIndex].type,
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
            Environments[0].ProcessShift(false, 1 - Environments[0].GameDataInstance.SubFrame);
            Environments[0].FallEvent(null, 1 - Environments[0].GameDataInstance.SubFrame);


            return true;
        }

        public void InputEvent()
        {

        }


    }
}
