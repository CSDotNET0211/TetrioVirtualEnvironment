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

        List<int> CurrentIndex = new List<int>();

        bool is_ttr;
        public int PlayerCount { get; private set; }

        public Replay(string jsondata, bool is_ttr)
        {
            this.is_ttr = is_ttr;

            if (is_ttr)
            {
                _replayDataTTR = (ReplayDataTTR)LibClass.ParseReplay(jsondata, is_ttr);


                var full = _replayDataTTR.data.events.Where(x => x.type == "full").First().data;
                var options = JsonSerializer.Deserialize<EventFull>(full.ToString()).options;
                Environments.Add(new Environment(options));
                CurrentIndex.Add(0);
                PlayerCount++;
            }
            else
            {
                _replayDataTTRM = (ReplayDataTTRM)LibClass.ParseReplay(jsondata, is_ttr);


                for (int i = 0; i < _replayDataTTRM.data[0].board.Count; i++)
                {
                    var full = _replayDataTTRM.data[0].replays[i].events.Where(x => x.type == "full").First().data;
                    var options = JsonSerializer.Deserialize<EventFull>(full.ToString()).options;
                    Environments.Add(new Environment(options));
                    CurrentIndex.Add(0);
                    //    Environments[i].Add(new List<(int frame, int posX, int power)>());
                    PlayerCount++;

                }
            }

        }


        public void SetFrame(int frame)
        {
            for (int i = 0; i < PlayerCount; i++)
                Environments[i].CurrentFrame = frame;

        }

        public bool Update()
        {
            for (int playerIndex = 0; playerIndex < Environments.Count; playerIndex++)
            {
                Environments[playerIndex].GameDataInstance.SubFrame = 0;

                if (is_ttr)
                {
                    if (!Event(playerIndex, _replayDataTTR.data.events))
                        return false;
                }
                else
                {
                    if (!Event(playerIndex, _replayDataTTRM.data[0].replays[playerIndex].events))
                        return false;
                }

                Environments[playerIndex].CurrentFrame++;
                Environments[playerIndex].ProcessShift(false, 1 - Environments[playerIndex].GameDataInstance.SubFrame);
                Environments[playerIndex].FallEvent(null, 1 - Environments[playerIndex].GameDataInstance.SubFrame);





            }

            return true;

        }

        void KeyDown(Environment.Action action)
        {
            var @event = new EventKeyInput();
            @event.hoisted = false;
            @event.subframe = 0;

            switch (action)
            {
                case Environment.Action.MoveRight:
                    @event.key = "moveRight";
                    break;

                case Environment.Action.MoveLeft:
                    @event.key = "moveLeft";
                    break;

                case Environment.Action.Softdrop:
                    @event.key = "softDrop";
                    break;

                case Environment.Action.RotateLeft:
                    @event.key = "rotateCCW";
                    break;

                case Environment.Action.RotateRight:
                    @event.key = "rotateCW";
                    break;

                case Environment.Action.Rotate180:
                    @event.key = "rotate180";
                    break;

                case Environment.Action.Harddrop:
                    @event.key = "hardDrop";
                    break;

                case Environment.Action.Hold:
                    @event.key = "hold";
                    break;





            }



            Environments[0].KeyInput("keydown", @event);
        }






        public bool Event(int playerIndex, List<Event> events)
        {
            while (true)
            {
                if (events[CurrentIndex[playerIndex]].frame == Environments[playerIndex].CurrentFrame)
                {

                    switch (events[CurrentIndex[playerIndex]].type)
                    {
                        case "start":
                            break;

                        case "full":
                            Environments[playerIndex].GameDataInstance.Falling.Init(null);
                            break;

                        case "keydown":
                        case "keyup":
                            var inputEvent = JsonSerializer.Deserialize<EventKeyInput>(events[CurrentIndex[playerIndex]].data.ToString());

                            if (events[CurrentIndex[playerIndex]].type == "keydown")
                            {
                                Environments[playerIndex].DownKeys.Add(inputEvent.key);
                            }
                            else
                            {
                                Environments[playerIndex].DownKeys.Remove(inputEvent.key);
                            }

                            Environments[playerIndex].KeyInput(events[CurrentIndex[playerIndex]].type,
                                                inputEvent);

                            break;

                        case "targets":
                            break;

                        case "ige":
                            var data = JsonSerializer.Deserialize<EventIge>(events[CurrentIndex[playerIndex]].data.ToString());

                            if (data.data.type == "interaction_confirm")
                            {
                                Environments[playerIndex].Garbages.Add(new Garbage((int)events[CurrentIndex[playerIndex]].frame, data.data.data.column, data.data.data.amt));
                            }
                            else if(data.data.type == "interaction")
                            {

                            }

                            break;

                        case "end":
                            return false;
                        default:
                            throw new Exception("invalid key:" + events[CurrentIndex[playerIndex]].type);
                    }

                    CurrentIndex[playerIndex]++;
                }
                else break;
            }


            return true;
        }


    }
}
