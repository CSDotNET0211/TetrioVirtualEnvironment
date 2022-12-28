using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using TetrReplayLoaderLib;
using static System.Net.Mime.MediaTypeNames;
using static TetrioVirtualEnvironment.Environment;
using static TetrReplayLoaderLib.TetrLoader;

namespace TetrioVirtualEnvironment
{
    /// <summary>
    /// Simple Structure having x,y Data.
    /// </summary>
    public struct Vector2
    {
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x;
        public double y;

        public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 one = new Vector2(1, 1);
        public static readonly Vector2 mone = new Vector2(-1, -1);
        public static readonly Vector2 x1 = new Vector2(1, 0);
        public static readonly Vector2 mx1 = new Vector2(-1, 0);
        public static readonly Vector2 x2 = new Vector2(2, 0);
        public static readonly Vector2 mx2 = new Vector2(-2, 0);
        public static readonly Vector2 y1 = new Vector2(0, 1);
        public static readonly Vector2 my1 = new Vector2(0, -1);
        public static readonly Vector2 y2 = new Vector2(0, 2);
        public static readonly Vector2 my2 = new Vector2(0, -2);

        public static Vector2 operator +(Vector2 obj, Vector2 obj2)
        {
            return new Vector2(obj.x + obj2.x, obj.y + obj2.y);
        }

        public static Vector2 operator -(Vector2 obj, Vector2 obj2)
        {
            return new Vector2(obj.x - obj2.x, obj.y - obj2.y);
        }
        public static Vector2 operator *(Vector2 obj, Vector2 obj2)
        {
            return new Vector2(obj.x * obj2.x, obj.y * obj2.y);
        }

    }

    /// <summary>
    /// TETR.IO Garbage Structure
    /// </summary>
    public class Garbage
    {
        public enum State
        {
            Interaction,
            Interaction_Confirm,
            Ready,
            Attack
        }

        public Garbage(int interaction_frame, int confirmed_frame, int sent_frame, int posX, int power, State state)
        {
            this.interaction_frame = interaction_frame;
            this.confirmed_frame = confirmed_frame;
            this.sent_frame = sent_frame;
            this.posX = posX;
            this.power = power;
            this.state = state;
        }

        public int confirmed_frame;
        public int interaction_frame;
        public int sent_frame;
        public int posX;
        public int power;
        public State state;

    }


    public class InitData
    {
        public InitData()
        {
            Field = null;
            Hold = null;
            Next = null;
            Now = null;
        }

        public InitData(int[] field, int hold, int now, int[] next)
        {
            Field = field;
            Hold = hold;
            Now = now;
            Next = next;
        }

        public int[]? Field;
        public int? Hold;
        public int? Now;
        public int[]? Next;

    }
    public class Environment
    {
        //DOTO: これとかapmに関する情報は関数でも作る
        public List<string> DownKeys = new List<string>();
        public const int FIELD_WIDTH = 10;
        public const int FIELD_HEIGHT = 40;
        //  public const int FIELD_VIEW_HEIGHT = 20;
        public event EventHandler OnPiecePlaced = null;
        List<Garbage> _historyInteraction;

        static public readonly Vector2[][][] CORNERTABLE =
          new Vector2[][][]{
              //Z
              new Vector2[][]{
                new Vector2[]{
                    new Vector2(-2, -1), new Vector2(1, -1), new Vector2(2, 0), new Vector2(-1, 0)
                },
                  new Vector2[] {
                    new Vector2(0, -1), new Vector2(1, -2), new Vector2(0, 2), new Vector2(1, 2)
                },
                  new Vector2[] {
                    new Vector2(-2, 0), new Vector2(1, 0), new Vector2(2, 1), new Vector2(-1, 1)
                },
                  new Vector2[] {
                    new Vector2(-1, -1), new Vector2(0, -2), new Vector2(0, 1), new Vector2(-1, 2)
                },
              },

                //L
              new Vector2[][]{
                  new Vector2[] {
                      new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, 1)
                  },
                  new Vector2[] {
                      new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(-1, 1)
                  },
                  new Vector2[] {
                      new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(0, 1)
                  },
                  new Vector2[] {
                      new Vector2(-1, 0), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1)
                  },
              },

              
              //S
              new Vector2[][]{
                    new Vector2[] {
                          new Vector2(-1, -1), new Vector2(2, -1), new Vector2(1,0), new Vector2(-2, 0)
                    },
                    new Vector2[] {
                           new Vector2(0, -2), new Vector2(1, -1), new Vector2(1, 2), new Vector2(0, 1)
                    },
                    new Vector2[] {
                         new Vector2(-1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(-2,1)
                    },
                   new Vector2[] {
                    new Vector2(-1,-2), new Vector2(0, -1), new Vector2(-1, 1), new Vector2(0, 2)
                    },
              },
              
              //J
              new Vector2[][]{
                    new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
                    new Vector2[] { new Vector2(-1, -1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(-1, 1) },
                    new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(0, 1), new Vector2(-1, 1) },
                    new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 0) },
              },
              
              //T
              new Vector2[][]{
                   new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
                   new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
                   new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
                   new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
                   },





        };

        static public Vector2[][] CORNERADDITIONALTTABLE =
               new Vector2[][]{
                   new Vector2[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
                   new Vector2[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
                   new Vector2[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
                   new Vector2[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
                   };



        /// <summary>
        /// Tetrimino Array
        /// </summary>
        static public Vector2[][][] TETRIMINOS =
          new Vector2[][][]{
              //Z
              new Vector2[][]{
                new Vector2[]{
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)
                },
                  new Vector2[] {
                    new Vector2(2, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2)
                },
                  new Vector2[] {
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2)
                },
                  new Vector2[] {
                    new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 2)
                },
              },

                //L
              new Vector2[][]{
                  new Vector2[] {
                      new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1)
                  },
                  new Vector2[] {
                      new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2)
                  },
                  new Vector2[] {
                      new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2)
                  },
                  new Vector2[] {
                      new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2)
                  },
              },

                //O
              new Vector2[][]{
                     new Vector2[] {
                         new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
                     },
                    new Vector2[] {
                        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
                    },
                     new Vector2[]
                     { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
                     },
                   new Vector2[] {
                       new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
                   },
              },
              
              //S
              new Vector2[][]{
                    new Vector2[] {
                          new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1)
                    },
                    new Vector2[] {
                           new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2)
                    },
                    new Vector2[] {
                         new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2), new Vector2(1, 2)
                    },
                   new Vector2[] {
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2)
                    },
              },
              
              //I
              new Vector2[][]{
                    new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(3, 1) },
                    new Vector2[] { new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2), new Vector2(2, 3) },
                    new Vector2[] { new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2), new Vector2(3, 2) },
                    new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(1, 3) },
              },
              
              
              //J
              new Vector2[][]{
                    new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) },
                    new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(1, 2) },
                    new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2) },
                    new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 2), new Vector2(1, 2) },
              },
              
              //T
              new Vector2[][]{
                   new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) },
                   new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) },
                   new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) },
                   new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) },
              },





        };
        /// <summary>
        /// Diff Position for Fix Tetrimino Position
        /// </summary>
        static public readonly Vector2[] TETRIMINO_DIFF = new Vector2[] { new Vector2(1, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1) };

        static public readonly int[] MINOTYPES = new int[] { (int)MinoKind.Z, (int)MinoKind.L, (int)MinoKind.O, (int)MinoKind.S, (int)MinoKind.I, (int)MinoKind.J, (int)MinoKind.T, };

        /// <summary>
        /// GarbageList
        /// </summary>
        public List<Garbage> Garbages;
        /// <summary>
        /// GarbageList for Garbage.State.Attack
        /// </summary>
        public List<Garbage> GarbagesImmediatery;
        public Dictionary<string, Vector2[]> KICKSET_SRSPLUS { get; private set; }
        public Dictionary<string, Vector2[]> KICKSET_SRSPLUSI { get; private set; }

        public GameData GameData;
        public Stats Stats;
        public RNG RNG = new RNG();
        public EnvironmentModeEnum EnvironmentMode;
        public EventFull EventFull;
        public InitData InitData;
        public int NextSkipCount;
        public int CurrentIndex { get; private set; }
        public int CurrentFrame { get; private set; }

        public enum EnvironmentModeEnum
        {
            Limited,
            Seed
        }
        public enum MinoKind
        {
            Z,
            L,
            O,
            S,
            I,
            J,
            T,
            Ojama,
            Empty
        }
        /// <summary>
        /// 回転状態を表す構造体
        /// </summary>
        public enum Rotation
        {
            Zero,
            Right,
            Turn,
            Left
        }
        /// <summary>
        /// 行動を表す構造体　AI用
        /// </summary>
        public enum Action : byte
        {
            Null,
            MoveUp,
            MoveDown,
            MoveRight,
            MoveLeft,
            RotateRight,
            RotateLeft,
            Rotate180,
            Harddrop,
            QuickSoftdrop,
            Softdrop,
            Hold
        }
        /// <summary>
        /// 操作中のテトリミノの状態を保持する構造体
        /// 種類・回転・位置の情報からなる
        /// </summary>
        /// <summary>
        /// 右か左回転か
        /// </summary>
        public enum Rotate : byte
        {
            Right,
            Left
        }

        /// <summary>
        /// 操作中のテトリミノの状態を保持する構造体
        /// 種類・回転・位置の情報からなる
        /// </summary>
        public struct Mino
        {
            public Mino(int minokind, int rotation, Vector2[] position)
            {
                MinoKind = minokind;
                Rotation = rotation;
                Positions = position;
            }

            public int MinoKind;
            public int Rotation;
            public Vector2[] Positions;
        }

        public enum KeyEvent
        {
            KeyDown,
            KeyUp
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envData"></param>
        /// <param name="envMode"></param>
        public Environment(EventFull envData, EnvironmentModeEnum envMode, InitData initData = null, int nextSkipCount = 0)
        {
            if (initData == null)
                initData = new InitData();

            EnvironmentMode = envMode;
            EventFull = envData;
            InitData = initData;
            NextSkipCount = nextSkipCount;

            ResetGame(envData, envMode, initData, nextSkipCount);

            var tempkickset = new Dictionary<string, Vector2[]>();
            var tempkicksetI = new Dictionary<string, Vector2[]>();

            #region KickSet Init
            tempkickset.Add("01", new Vector2[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2), });
            tempkickset.Add("10", new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2), });
            tempkickset.Add("12", new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2), });
            tempkickset.Add("21", new Vector2[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2), });
            tempkickset.Add("23", new Vector2[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2), });
            tempkickset.Add("32", new Vector2[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2), });
            tempkickset.Add("30", new Vector2[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2), });
            tempkickset.Add("03", new Vector2[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2), });
            tempkickset.Add("02", new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(-1, 0), });
            tempkickset.Add("13", new Vector2[] { new Vector2(1, 0), new Vector2(1, -2), new Vector2(1, -1), new Vector2(0, -2), new Vector2(0, -1), });
            tempkickset.Add("20", new Vector2[] { new Vector2(0, 1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(1, 0), });
            tempkickset.Add("31", new Vector2[] { new Vector2(-1, 0), new Vector2(-1, -2), new Vector2(-1, -1), new Vector2(0, -2), new Vector2(0, -1), });
            KICKSET_SRSPLUS = tempkickset;

            tempkicksetI.Add("01", new Vector2[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(-2, 1), new Vector2(1, -2), });
            tempkicksetI.Add("10", new Vector2[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, 2), new Vector2(2, -1), });
            tempkicksetI.Add("12", new Vector2[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(2, 1), });
            tempkicksetI.Add("21", new Vector2[] { new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, -1), new Vector2(1, 2), });
            tempkicksetI.Add("23", new Vector2[] { new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2), });
            tempkicksetI.Add("32", new Vector2[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, -2), new Vector2(-2, 1), });
            tempkicksetI.Add("30", new Vector2[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1), });
            tempkicksetI.Add("03", new Vector2[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(2, 1), new Vector2(-1, -2), });
            tempkicksetI.Add("02", new Vector2[] { new Vector2(0, -1) });
            tempkicksetI.Add("13", new Vector2[] { new Vector2(1, 0) });
            tempkicksetI.Add("20", new Vector2[] { new Vector2(0, 1) });
            tempkicksetI.Add("31", new Vector2[] { new Vector2(-1, 0) });
            KICKSET_SRSPLUSI = tempkicksetI;
            #endregion

        }

        /// <summary>
        /// ResetGame for Rewind
        /// </summary>
        /// <param name="envData"></param>
        public void ResetGame(EventFull envData, EnvironmentModeEnum envMode, InitData initData, int nextSkipCount = 0)
        {

            _historyInteraction = new List<Garbage>();
            Garbages = new List<Garbage>();
            GarbagesImmediatery = new List<Garbage>();
            CurrentFrame = 0;
            CurrentIndex = 0;
            Stats = new Stats();

            if (envMode == EnvironmentModeEnum.Seed)
                new GameData(envData, this, ref GameData, nextSkipCount, initData);
            else
                new GameData(envData.options, this, ref GameData, initData);
        }

        /// <summary>
        /// KeyInput for Self Control
        /// </summary>
        /// <param name="keyEvent"></param>
        /// <param name="keyKind"></param>
        public void InputKeyEvent(KeyEvent keyEvent, Action keyKind)
        {
            if (EnvironmentMode == EnvironmentModeEnum.Limited)
                if (keyKind == Action.Hold)
                {

                }
                else
                    return;

            if (keyEvent == KeyEvent.KeyUp)
            {

            }
            else if (keyEvent == KeyEvent.KeyDown)
            {

            }
        }


        static public bool IsEmptyPos(int x, int y, int[] field)
        {
            if (!(x >= 0 && x < FIELD_WIDTH &&
                y >= 0 && y < FIELD_HEIGHT))
                return false;
            return field[x + y * 10] == (int)MinoKind.Empty;
        }

        static public bool IsLegalField(int type, int x, double y, int rotation)
        {
            var positions = TETRIMINOS[type][rotation];
            var diff = TETRIMINO_DIFF[type];

            foreach (var position in positions)
            {
                if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
                  position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check Is Valid Position or Not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static public bool IsLegalField(int x, double y)
        {
            if (!(x >= 0 && x < FIELD_WIDTH &&
            y >= 0 && y < FIELD_HEIGHT))
                return false;


            return true;
        }

        static public bool IsLegalAtPos(int type, int x, double y, int rotation, int[] field)
        {
            var positions = TETRIMINOS[type][rotation];
            var diff = TETRIMINO_DIFF[type];

            foreach (var position in positions)
            {
                if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
                  position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT &&
                     field[(int)position.x + x - (int)diff.x + (int)(position.y + y - (int)diff.y) * 10] == (int)MinoKind.Empty))
                    return false;
            }

            return true;
        }



        public void KeyInput(string type, EventKeyInput @event)
        {
            if (type == "keydown")
            {
                if (@event.subframe > GameData.SubFrame)
                {
                    ProcessShift(false, @event.subframe - GameData.SubFrame);
                    FallEvent(null, @event.subframe - GameData.SubFrame);
                    GameData.SubFrame = @event.subframe;
                }

                if (@event.key == "moveLeft")
                {
                    GameData.LShift = true;
                    GameData.LastShift = -1;
                    GameData.LDas = @event.hoisted ?
                        GameData.Handling.DAS - GameData.Handling.DCD : 0;
                    if (GameData.Options.Version >= 12)
                        GameData.LDasIter = GameData.Handling.ARR;

                    ProcessLShift(true, GameData.Options.Version >= 15 ? 0 : 1);
                    return;
                }

                if (@event.key == "moveRight")
                {
                    GameData.RShift = true;
                    GameData.LastShift = 1;
                    GameData.RDas = @event.hoisted ? GameData.Handling.DAS - GameData.Handling.DCD : 0;
                    if (GameData.Options.Version >= 12)
                        GameData.RDasIter = GameData.Handling.ARR;

                    ProcessRShift(true, GameData.Options.Version >= 15 ? 0 : 1);
                    return;
                }

                if (@event.key == "softDrop")
                {
                    GameData.SoftDrop = true;
                    return;
                }

                if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
                {
                    throw new Exception("未実装");
                }
                else
                {
                    if (@event.key == "rotateCCW")
                    {
                        var e = GameData.Falling.r - 1;
                        if (e < 0)
                            e = 3;
                        RotatePiece(e);
                    }

                    if (@event.key == "rotateCW")
                    {
                        var e = GameData.Falling.r + 1;
                        if (e > 3)
                            e = 0;
                        RotatePiece(e);
                    }

                    if (@event.key == "rotate180" && GameData.Options.Allow180)
                    {
                        var e = GameData.Falling.r + 2;
                        if (e > 3)
                            e -= 4;
                        RotatePiece(e);
                    }
                    if (@event.key == "hardDrop" && GameData.Options.AllowHardDrop &&
                        GameData.Falling.SafeLock == 0)
                    {
                        FallEvent(int.MaxValue, 1);
                    }

                    if (@event.key == "hold")
                    {
                        if (!GameData.HoldLocked)
                        {
                            if ((GameData.Options.DisplayHold == null ||
                            (bool)GameData.Options.DisplayHold))
                            {
                                SwapHold();
                            }

                        }
                    }
                }

            }
            else if (type == "keyup")
            {
                if (@event.subframe > GameData.SubFrame)
                {
                    ProcessShift(false, @event.subframe - GameData.SubFrame);
                    FallEvent(null, @event.subframe - GameData.SubFrame);
                    GameData.SubFrame = @event.subframe;
                }

                if (@event.key == "moveLeft")
                {
                    GameData.LShift = false;
                    GameData.LDas = 0;

                    if (GameData.Handling.Cancel)
                    {
                        GameData.RDas = 0;
                        GameData.RDasIter = GameData.Handling.ARR;
                    }

                    return;
                }

                if (@event.key == "moveRight")
                {
                    GameData.RShift = false;
                    GameData.RDas = 0;

                    if (GameData.Handling.Cancel)
                    {
                        GameData.LDas = 0;
                        GameData.LDasIter = GameData.Handling.ARR;
                    }

                    return;
                }

                if (@event.key == "softDrop")
                    GameData.SoftDrop = false;


            }
        }

        void RotatePiece(int rotation)
        {
            var nowmino_rotation = GameData.Falling.r;
            var nowmino_newmino_rotation = nowmino_rotation.ToString() + rotation.ToString();
            var o = 0;
            var i = "none";

            if (rotation < nowmino_rotation)
            {
                o = 1;
                i = "right";
            }
            else
            {
                o = -1;
                i = "left";
            }

            if (rotation == 0 && nowmino_rotation == 3)
            {
                o = 1;
                i = "right";
            }

            if (rotation == 3 && nowmino_rotation == 3)
            {
                o = -1;
                i = "left";
            }

            if (rotation == 2 && nowmino_rotation == 0)
                i = "vertical";
            if (rotation == 0 && nowmino_rotation == 2)
                i = "vertical";
            if (rotation == 3 && nowmino_rotation == 1)
                i = "horizontal";
            if (rotation == 1 && nowmino_rotation == 3)
                i = "horizontal";

            if (IsLegalAtPos(GameData.Falling.type,
                GameData.Falling.x - GameData.Falling.aox,
                GameData.Falling.y - GameData.Falling.aoy, rotation,
                GameData.Field))
            {
                GameData.Falling.x -= GameData.Falling.aox;
                GameData.Falling.y -= GameData.Falling.aoy;
                GameData.Falling.aox = 0;
                GameData.Falling.aoy = 0;
                GameData.Falling.r = rotation;
                GameData.Falling.Last = "rotate";
                GameData.Falling.LastRotation = i;
                GameData.Falling.LastKick = 0;
                GameData.Falling.SpinType = IsTspin();
                GameData.FallingRotations++;
                GameData.TotalRotations++;

                if (GameData.Falling.Clamped && GameData.Handling.DCD > 0)
                {
                    GameData.LDas = Math.Min(GameData.LDas, GameData.Handling.DAS - GameData.Handling.DCD);
                    GameData.LDasIter = GameData.Handling.ARR;
                    GameData.RDas = Math.Min(GameData.RDas, GameData.Handling.DAS - GameData.Handling.DCD);
                    GameData.RDasIter = GameData.Handling.ARR;
                }

                if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                    GameData.Falling.Locking = 0;


                //落下ミノ更新フラグ true
                return;
            }

            if (GameData.Falling.type == (int)MinoKind.O)
                return;

            var kicktable = KICKSET_SRSPLUS[nowmino_newmino_rotation];

            if (GameData.Falling.type == (int)MinoKind.I)
                kicktable = KICKSET_SRSPLUSI[nowmino_newmino_rotation];

            for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
            {
                var kicktableTest = kicktable[kicktableIndex];
                var i2 = (int)(GameData.Falling.y) + 0.1 +
                    kicktableTest.y - GameData.Falling.aoy;


                if (!GameData.Options.InfiniteMovement &&
                    GameData.TotalRotations > (int)GameData.Options.LockResets + 15)
                {
                    i2 = GameData.Falling.y + kicktableTest.y - GameData.Falling.aoy;
                }

                if (IsLegalAtPos(GameData.Falling.type,
                    GameData.Falling.x + (int)kicktableTest.x - GameData.Falling.aox,
                    i2, rotation, GameData.Field))
                {

                    GameData.Falling.x += (int)kicktableTest.x - GameData.Falling.aox;
                    GameData.Falling.y = i2;
                    GameData.Falling.aox = 0;
                    GameData.Falling.aoy = 0;
                    GameData.Falling.r = rotation;
                    GameData.Falling.SpinType = IsTspin();
                    GameData.Falling.LastKick = kicktableIndex + 1;
                    GameData.Falling.Last = "rotate";
                    GameData.FallingRotations++;
                    GameData.TotalRotations++;

                    if (GameData.Falling.Clamped && GameData.Handling.DCD > 0)
                    {
                        GameData.LDas = Math.Min(GameData.LDas, GameData.Handling.DAS - GameData.Handling.DCD);
                        GameData.LDasIter = GameData.Handling.ARR;
                        GameData.RDas = Math.Min(GameData.RDas, GameData.Handling.DAS - GameData.Handling.DCD);
                        GameData.RDasIter = GameData.Handling.ARR;
                    }

                    if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking = 0;


                    return;
                }
            }


        }

        public void ProcessShift(bool value, double subFrameDiff)
        {
            ProcessLShift(value, subFrameDiff);
            ProcessRShift(value, subFrameDiff);

        }

        public void Update()
        {
            GameData.SubFrame = 0;
            //    CurrentFrame++;
            ProcessShift(false, 1 - GameData.SubFrame);
            FallEvent(null, 1 - GameData.SubFrame);
        }
        //TODO: いつか下のに結合
        public bool Update(ReplayKind replayKind, List<Event> events)
        {
            GameData.SubFrame = 0;

            if (!Event(events))
                return false;

            CurrentFrame++;
            ProcessShift(false, 1 - GameData.SubFrame);
            FallEvent(null, 1 - GameData.SubFrame);

            CheckGarbage();

            if (GameData.Options.GravityIncrease > 0 &&
                CurrentFrame > GameData.Options.GravityMargin)
                GameData.Gravity += GameData.Options.GravityIncrease / 60;

            if (GameData.Options.GarbageIncrease > 0 &&
                CurrentFrame > GameData.Options.GarbageMargin)
                GameData.Options.GarbageMultiplier += GameData.Options.GarbageIncrease / 60;

            if (GameData.Options.GarbageCapIncrease > 0)
                GameData.Options.GarbageCap += GameData.Options.GarbageCapIncrease / 60;



            return true;
        }

        void CheckGarbage()
        {
            foreach (var garbage in Garbages)
            {
                if (garbage.confirmed_frame + 20 == CurrentFrame)
                {
                    if (garbage.state == Garbage.State.Interaction_Confirm)
                        garbage.state = Garbage.State.Ready;

                }
            }

            for (int i = GarbagesImmediatery.Count - 1; i >= 0; i--)
            {
                if (GarbagesImmediatery[i].confirmed_frame + 20 == CurrentFrame)
                {
                    GarbagesImmediatery[i].state = Garbage.State.Ready;
                    Garbages.Add(GarbagesImmediatery[i]);
                    GarbagesImmediatery.RemoveAt(i);
                }
            }
        }

        public bool Event(List<Event> events)
        {
            while (true)
            {
                if (events[CurrentIndex].frame == CurrentFrame)
                {

                    switch (events[CurrentIndex].type)
                    {
                        case "start":
                            break;

                        case "full":
                            GameData.Falling.Init(null, EnvironmentMode);
                            break;

                        case "keydown":
                        case "keyup":
                            var inputEvent = JsonSerializer.Deserialize<EventKeyInput>(events[CurrentIndex].data.ToString());

                            if (events[CurrentIndex].type == "keydown")
                            {
                                DownKeys.Add(inputEvent.key);
                            }
                            else
                            {
                                DownKeys.Remove(inputEvent.key);
                            }

                            KeyInput(events[CurrentIndex].type, inputEvent);

                            break;

                        case "targets":
                            break;

                        case "ige":
                            var data = JsonSerializer.Deserialize<EventIge>(events[CurrentIndex].data.ToString());

                            if (data.data.type == "interaction_confirm")
                            {
                                var flag = true;
                                foreach (var garbage in Garbages)
                                {
                                    if (garbage.sent_frame == data.data.sent_frame && garbage.state == Garbage.State.Interaction)
                                    {

                                        garbage.state = Garbage.State.Interaction_Confirm;
                                        garbage.confirmed_frame = (int)events[CurrentIndex].frame;
                                        flag = false;
                                        break;
                                    }

                                }

                                if (flag)
                                {
                                    var flag2 = true;

                                    var count = _historyInteraction.Count;
                                    for (int i = 0; i < count; i++)
                                    {
                                        if (_historyInteraction[0].sent_frame == data.data.sent_frame)
                                        {
                                            flag2 = false;
                                            _historyInteraction.RemoveAt(0);
                                            break;
                                        }
                                    }

                                    if (flag2)
                                        GarbagesImmediatery.Add(new Garbage(data.frame,
                                        (int)events[CurrentIndex].frame, (int)data.data.sent_frame, data.data.data.column, (int)data.data.data.amt, Garbage.State.Attack));


                                    //TODO: これ本当にあってる？
                                    //  //  Garbages.Add(new Garbage(data.frame, -1, (int)data.data.sent_frame, data.data.data.column,
                                    //    (int)data.data.data.amt, Garbage.State.Ready));

                                }
                            }
                            else if (data.data.type == "interaction")
                            {
                                Garbages.Add(new Garbage(data.frame, -1, (int)data.data.sent_frame, data.data.data.column, (int)data.data.data.amt, Garbage.State.Interaction));
                                //    _historyInteraction.Add(data.data.sent_frame);
                            }
                            else if (data.data.type == "attack")
                                GarbagesImmediatery.Add(new Garbage(data.frame, (int)events[CurrentIndex].frame, (int)data.data.sent_frame, data.data.column, (int)data.data.lines, Garbage.State.Attack));
                            else if (data.data.type == "kev")
                            {

                            }
                            else throw new Exception("Unknown InGameEvent");

                            break;

                        case "end":
                            return false;
                        default:
                            throw new Exception("invalid key:" + events[CurrentIndex].type);
                    }

                    CurrentIndex++;
                }
                else break;
            }


            return true;
        }


        void ProcessLShift(bool value, double subFrameDiff = 1)
        {
            if (!GameData.LShift || GameData.RShift && GameData.LastShift != -1)
                return;

            var subFrameDiff2 = subFrameDiff;
            var dasSomething = Math.Max(0, GameData.Handling.DAS - GameData.LDas);

            GameData.LDas += value ? 0 : subFrameDiff;

            if (GameData.LDas < GameData.Handling.DAS && !value)
                return;

            subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);

            if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
                return;

            var aboutARRValue = 1;
            if (!value)
            {
                GameData.LDasIter += GameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

                if (GameData.LDasIter < GameData.Handling.ARR)
                    return;

                aboutARRValue = GameData.Handling.ARR == 0 ? 10 : (int)(GameData.LDasIter / GameData.Handling.ARR);

                GameData.LDasIter -= GameData.Handling.ARR * aboutARRValue;
            }

            for (var index = 0; index < aboutARRValue; index++)
            {
                if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x - 1, GameData.Falling.y, GameData.Falling.r, GameData.Field))
                {
                    GameData.Falling.x--;
                    GameData.Falling.Last = "move";
                    GameData.Falling.Clamped = false;

                    if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking = 0;

                }
                else
                {
                    GameData.Falling.Clamped = true;
                }
            }
        }

        void ProcessRShift(bool value, double subFrameDiff = 1)
        {
            if (!GameData.RShift || GameData.LShift && GameData.LastShift != 1)
                return;

            var subFrameDiff2 = subFrameDiff;
            var dasSomething = Math.Max(0, GameData.Handling.DAS - GameData.RDas);

            GameData.RDas += value ? 0 : subFrameDiff;

            if (GameData.RDas < GameData.Handling.DAS && !value)
                return;

            subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);
            if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
                return;

            var aboutARRValue = 1;
            if (!value)
            {
                GameData.RDasIter += GameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

                if (GameData.RDasIter < GameData.Handling.ARR)
                    return;

                aboutARRValue = GameData.Handling.ARR == 0 ? 10 : (int)(GameData.RDasIter / GameData.Handling.ARR);

                GameData.RDasIter -= GameData.Handling.ARR * aboutARRValue;
            }

            for (var index = 0; index < aboutARRValue; index++)
            {
                if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x + 1, GameData.Falling.y, GameData.Falling.r, GameData.Field))
                {
                    GameData.Falling.x++;
                    GameData.Falling.Last = "move";
                    GameData.Falling.Clamped = false;

                    if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking = 0;

                }
                else
                {
                    GameData.Falling.Clamped = true;
                }
            }
        }

        public void FallEvent(int? value, double subFrameDiff)
        {
            if (GameData.Falling.SafeLock > 0)
                GameData.Falling.SafeLock--;

            if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
                return;

            var subframeGravity = GameData.Gravity * subFrameDiff;

            if (GameData.SoftDrop)
            {
                if (GameData.Handling.SDF == (GameData.Options.Version >= 15 ? 41 : 21))
                    subframeGravity = (GameData.Options.Version >= 15 ? 400 : 20) * subFrameDiff;
                else
                {
                    subframeGravity *= GameData.Handling.SDF;
                    subframeGravity = Math.Max(subframeGravity, GameData.Options.Version >= 13 ?
                        0.05 * GameData.Handling.SDF : 0.42);
                }
            }

            if (value != null)
                subframeGravity = (int)value;

            if (!GameData.Options.InfiniteMovement &&
                GameData.Falling.LockResets >= (int)GameData.Options.LockResets &&
                !IsLegalAtPos(GameData.Falling.type, GameData.Falling.x,
                GameData.Falling.y + 1, GameData.Falling.r, GameData.Field))
            {
                subframeGravity = 20;
                GameData.Falling.ForceLock = true;
            }


            if (!GameData.Options.InfiniteMovement &&
                GameData.FallingRotations > (int)GameData.Options.LockResets + 15)
            {
                subframeGravity += 0.5 * subFrameDiff *
                    (GameData.FallingRotations - ((int)GameData.Options.LockResets + 15));
            }

            var r = subframeGravity;

            for (; subframeGravity > 0;)
            {
                var ceiledValue = Math.Ceiling(GameData.Falling.y);
                if (!HardDrop(Math.Min(1, subframeGravity), r))
                {
                    if (value != null)
                        GameData.Falling.ForceLock = true;
                    FunctionA(value != 0 && value != null, subFrameDiff);
                    break;
                }

                subframeGravity -= Math.Min(1, subframeGravity);
                if (ceiledValue != Math.Ceiling(GameData.Falling.y))
                {
                    GameData.Falling.Last = "fall";
                    if (GameData.SoftDrop)
                    {
                        //ScoreAdd

                    }
                }
            }
        }

        bool HardDrop(double value, double value2)
        {
            var fallingy_kouho = Math.Floor(Math.Pow(10, 13) * GameData.Falling.y) /
                Math.Pow(10, 13) + value;

            if (fallingy_kouho % 1 == 0)
                fallingy_kouho += 0.00001;

            var o = Math.Floor(Math.Pow(10, 13) * GameData.Falling.y) / Math.Pow(10, 13) + 1;
            if (o % 1 == 0)
                o -= 0.00002;

            if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, fallingy_kouho, GameData.Falling.r, GameData.Field) &&
                IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, o, GameData.Falling.r, GameData.Field))
            {
                var s = GameData.Falling.HighestY;
                o = GameData.Falling.y;

                GameData.Falling.y = fallingy_kouho;
                GameData.Falling.HighestY = Math.Ceiling(Math.Max(GameData.Falling.HighestY, fallingy_kouho));
                GameData.Falling.Floored = false;
                if (Math.Ceiling(fallingy_kouho) != Math.Ceiling(o))
                {
                    // TODO: 更新フラグtrue

                }

                if (fallingy_kouho > s || GameData.Options.InfiniteMovement)
                    GameData.Falling.LockResets = 0;
                GameData.FallingRotations = 0;

                if (value2 >= int.MaxValue)
                {
                    //スコア足す
                }

                return true;
            }

            return false;
        }

        void PiecePlace(bool sValue)
        {
            GameData.Falling.Sleep = true;
            //ミノを盤面に適用
            foreach (var pos in TETRIMINOS[GameData.Falling.type][GameData.Falling.r])
            {
                GameData.Field[(int)((pos.x + GameData.Falling.x - TETRIMINO_DIFF[GameData.Falling.type].x) +
                    (int)(pos.y + GameData.Falling.y - TETRIMINO_DIFF[GameData.Falling.type].y) * 10)] = GameData.Falling.type;
            }

            if (!sValue && GameData.Handling.SafeLock > 0)
                GameData.Falling.SafeLock = 7;

            var istspin = IsTspin();
            var clearedLineCount = ClearLine();


            // int tempGargabeCount = 0;


            GetAttackPower(clearedLineCount, istspin);
            IsBoardEmpty();

            //

            if (OnPiecePlaced != null)
                OnPiecePlaced(this, EventArgs.Empty);


            GameData.Falling.Init(null, EnvironmentMode);




        }


        public void Move(Action action)
        {
            switch (action)
            {
                case Action.MoveRight:
                    if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x + 1, GameData.Falling.y, GameData.Falling.r, GameData.Field))
                        GameData.Falling.x++;
                    break;

                case Action.MoveLeft:
                    if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x - 1, GameData.Falling.y, GameData.Falling.r, GameData.Field))
                        GameData.Falling.x--;
                    break;

                case Action.RotateLeft:
                    var e = GameData.Falling.r - 1;
                    if (e < 0)
                        e = 3;
                    RotatePiece(e);
                    break;

                case Action.RotateRight:
                    e = GameData.Falling.r + 1;
                    if (e > 3)
                        e = 0;
                    RotatePiece(e);
                    break;

                case Action.Rotate180:
                    e = GameData.Falling.r + 2;
                    if (e > 3)
                        e -= 4;
                    RotatePiece(e);
                    break;

                case Action.Hold:
                    SwapHold();
                    break;

                case Action.Harddrop:
                    FallEvent(int.MaxValue, 1);
                    break;

                case Action.QuickSoftdrop:
                    while (true)
                    {
                        if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y + 1, GameData.Falling.r, GameData.Field))
                            GameData.Falling.y++;
                        else
                            break;
                    }
                    break;

                case Action.MoveUp:
                    if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y - 1, GameData.Falling.r, GameData.Field))
                        GameData.Falling.y--;
                    break;

                case Action.MoveDown:
                    if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y + 1, GameData.Falling.r, GameData.Field))
                        GameData.Falling.y++;
                    break;

                default: throw new Exception();
            }

        }

        /// <summary>
        /// This is for DEBUG
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public object GetData(string kind)
        {
            switch (kind)
            {
                case "safelock":

                    break;
            }

            if (kind == "safelock")
                return GameData.Falling.SafeLock;

            return null;
        }

        int ClearLine()
        {
            List<int> list = new List<int>();
            bool flag = true;

            for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
            {
                flag = true;
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (GameData.Field[x + y * 10] == (int)MinoKind.Empty)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                    list.Add(y);
            }

            list.Reverse();
            foreach (var value in list)
            {
                DownLine(value, GameData.Field);
            }

            return list.Count;

        }

        static public Vector2[] GetMinoPos(int type, int x, int y, int r)
        {
            var positions = (Vector2[])TETRIMINOS[type][r].Clone();
            var diff = TETRIMINO_DIFF[type];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].x += x - diff.x;
                positions[i].y += y - diff.y;
            }

            return positions;
        }

        public int GetFallestPosDiff()
        {
            int testy = 1;
            while (true)
            {
                if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y + testy,
                    GameData.Falling.r, GameData.Field))
                    testy++;
                else
                {
                    testy--;
                    break;
                }
            }

            return testy;
        }
        /// <summary>
        /// 消去したラインを下げる
        /// </summary>
        /// <param name="value">y座標</param>
        /// <param name="field">盤面</param>
        private void DownLine(int value, int[] field)
        {
            for (int y = value; y >= 0; y--)
            {
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (y - 1 == -1)
                        field[x + y * 10] = (int)(MinoKind.Empty);
                    else
                        field[x + y * 10] = field[x + (y - 1) * 10];
                }
            }
        }

        void FunctionA(bool value = false, double subframe = 1)
        {
            if (GameData.Options.Version >= 15)
                GameData.Falling.Locking += subframe;
            else
                GameData.Falling.Locking += 1;

            if (!GameData.Falling.Floored)
                GameData.Falling.Floored = true;



            if (GameData.Falling.Locking > GameData.Options.LockTime ||
                GameData.Falling.ForceLock ||
                GameData.Falling.LockResets > GameData.Options.LockResets &&
                !GameData.Options.InfiniteMovement)
            {
                PiecePlace(value);




            }
        }

        public int RefreshNext(List<int> next, bool noszo)
        {
            var value = next[0];

            for (int i = 0; i < next.Count - 1; i++)
            {
                next[i] = next[i + 1];
            }

            if (GameData.NextBag.Count == 0)
            {
                var array = (int[])MINOTYPES.Clone();
                RNG.ShuffleArray(array);
                GameData.NextBag.AddRange(array);
            }

            if (noszo)
            {
                while (true)
                {
                    if (GameData.NextBag[0] == (int)MinoKind.S ||
                         GameData.NextBag[0] == (int)MinoKind.Z ||
                        GameData.NextBag[0] == (int)MinoKind.O)
                    {
                        var temp = GameData.NextBag[0];
                        for (int i = 0; i < GameData.NextBag.Count - 1; i++)
                        {
                            GameData.NextBag[i] = GameData.NextBag[i + 1];
                        }
                        GameData.NextBag[GameData.NextBag.Count - 1] = temp;
                    }
                    else
                        break;
                }

            }


            next[next.Count - 1] = GameData.NextBag[0];
            GameData.NextBag.RemoveAt(0);
            return value;
        }

        void SwapHold()
        {

            var s = GameData.Falling.type;
            GameData.Falling.Init(GameData.Hold, EnvironmentMode);
            GameData.Hold = s;
        }

        string? IsTspin()
        {
            if (GameData.SpinBonuses == "none")
                return null;

            if (GameData.SpinBonuses == "stupid")
                throw new Exception("未実装");

            if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y + 1, GameData.Falling.r, GameData.Field))
                return null;

            if (GameData.Falling.type != (int)MinoKind.T && GameData.SpinBonuses != "handheld")
            {
                if (GameData.SpinBonuses == "all")
                {
                    if (!(IsLegalAtPos(GameData.Falling.type, GameData.Falling.x - 1, GameData.Falling.y, GameData.Falling.r, GameData.Field) ||
                       IsLegalAtPos(GameData.Falling.type, GameData.Falling.x + 1, GameData.Falling.y, GameData.Falling.r, GameData.Field) ||
                       IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y - 1, GameData.Falling.r, GameData.Field) ||
                       IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y + 1, GameData.Falling.r, GameData.Field)))
                        return "normal";
                    else
                        return null;
                }
                else
                    return null;

            }

            if (GameData.Falling.Last != "rotate")
                return null;

            var cornerCount = 0;
            var a = 0;

            for (int n = 0; n < 4; n++)
            {
                Vector2[][] minoCorner = null;

                minoCorner = CORNERTABLE[TEMP(GameData.Falling.type)];

                int TEMP(int type)
                {
                    switch (type)
                    {
                        case (int)MinoKind.Z:
                        case (int)MinoKind.L:
                            return type;
                        case (int)MinoKind.S:
                            return type - 1;
                        case (int)MinoKind.J:
                        case (int)MinoKind.T:
                            return type - 2;

                        default: throw new Exception("不明");

                    }
                }

                if (!IsEmptyPos((int)(GameData.Falling.x + minoCorner[GameData.Falling.r][n].x),
                    (int)(GameData.Falling.y + minoCorner[GameData.Falling.r][n].y), GameData.Field))
                {
                    cornerCount++;

                    //AdditinalTableは無理やり追加したものなのでx,yは関係ない
                    if (!(GameData.Falling.type != (int)MinoKind.T ||
                        (GameData.Falling.r != CORNERADDITIONALTTABLE[GameData.Falling.r][n].x &&
                        GameData.Falling.r != CORNERADDITIONALTTABLE[GameData.Falling.r][n].y)))
                        a++;
                }

            }


            if (cornerCount < 3)
                return null;

            var spintype = "normal";

            if (GameData.Falling.type == (int)MinoKind.T && a != 2)
                spintype = "mini";

            if (GameData.Falling.LastKick == 4)
                spintype = "normal";


            return spintype;

        }

        public void ReceiveGarbage(int garbageX, int power)
        {
            //power分上に上げる
            //下をループでx以外

            for (int y = 0; y < FIELD_HEIGHT; y++)
            {
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (y + power >= FIELD_HEIGHT)
                        break;
                    else
                        GameData.Field[x + (y) * 10] = GameData.Field[x + (y + power) * 10];
                }
            }


            for (int y = FIELD_HEIGHT - 1; y > FIELD_HEIGHT - 1 - power; y--)
            {
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (x == garbageX)
                        GameData.Field[x + (y) * 10] = (int)MinoKind.Empty;
                    else
                        GameData.Field[x + (y) * 10] = (int)MinoKind.Ojama;
                }
            }
        }

        public void GetAttackPower(int clearLineCount, string? isTspin)
        {
            var isBTB = false;

            if (clearLineCount > 0)
            {
                Stats.Combo++;
                Stats.TopCombo = Math.Max(Stats.Combo, Stats.TopCombo);

                if (clearLineCount == 4)
                    isBTB = true;
                else
                {
                    if (isTspin != null)
                        isBTB = true;
                }

                if (isBTB)
                {
                    Stats.BTB++;
                    Stats.TopBTB = Math.Max(Stats.BTB, Stats.TopBTB);
                }
                else
                {
                    //btb updateの中身何
                    Stats.BTB = 0;

                }

            }
            else
            {
                Stats.Combo = 0;
                Stats.CurrentComboPower = 0;
            }


            var garbageValue = 0.0;
            switch (clearLineCount)
            {
                case 0:
                    if (isTspin == "mini")
                    {
                        garbageValue = DataGarbage.TSPIN_MINI;
                    }
                    else if (isTspin == "normal")
                    {
                        garbageValue = DataGarbage.TSPIN;
                    }
                    break;

                case 1:
                    if (isTspin == "mini")
                    {
                        garbageValue = DataGarbage.TSPIN_MINI_SINGLE;
                    }
                    else if (isTspin == "normal")
                    {
                        garbageValue = DataGarbage.TSPIN_SINGLE;
                    }
                    else
                    {
                        garbageValue = DataGarbage.SINGLE;
                    }
                    break;

                case 2:
                    if (isTspin == "mini")
                    {
                        garbageValue = DataGarbage.TSPIN_MINI_DOUBLE;
                    }
                    else if (isTspin == "normal")
                    {
                        garbageValue = DataGarbage.TSPIN_DOUBLE;
                    }
                    else
                    {
                        garbageValue = DataGarbage.DOUBLE;
                    }
                    break;

                case 3:
                    if (isTspin != null)
                    {
                        garbageValue = DataGarbage.TSPIN_TRIPLE;
                    }
                    else
                    {
                        garbageValue = DataGarbage.TRIPLE;
                    }
                    break;

                case 4:
                    if (isTspin != null)
                    {
                        garbageValue = DataGarbage.TSPIN_QUAD;
                    }
                    else
                    {
                        garbageValue = DataGarbage.QUAD;
                    }
                    break;
            }

            if (clearLineCount > 0 && Stats.BTB > 1)
            {
                if (GameData.Options.BTBChaining)
                {
                    double tempValue;
                    if (Stats.BTB - 1 == 1)
                        tempValue = 0;
                    else
                        tempValue = 1 + (Math.Log((Stats.BTB - 1) * DataGarbage.BACKTOBACK_BONUS_LOG + 1) % 1);

                    var btb_bonus = DataGarbage.BACKTOBACK_BONUS *
                        (Math.Floor(1 + Math.Log((Stats.BTB - 1) * DataGarbage.BACKTOBACK_BONUS_LOG + 1)) + (tempValue / 3));

                    garbageValue += btb_bonus;

                    if ((int)btb_bonus >= 2)
                    {
                        //AddFire
                    }

                    if ((int)btb_bonus > GameData.CurrentBTBChainPower)
                    {
                        GameData.CurrentBTBChainPower = (int)btb_bonus;
                    }



                }
                else
                    garbageValue += DataGarbage.BACKTOBACK_BONUS;
            }
            else
            {
                //TODO: これ本当？
                if (clearLineCount > 0 && Stats.BTB <= 1)
                    GameData.CurrentBTBChainPower = 0;
            }

            if (Stats.Combo > 1)
            {
                garbageValue *= 1 + DataGarbage.COMBO_BONUS * (Stats.Combo - 1);
            }

            if (Stats.Combo > 2)
            {
                garbageValue = Math.Max(Math.Log(DataGarbage.COMBO_MINIFIER *
                    (Stats.Combo - 1) *
                    DataGarbage.COMBO_MINIFIER_LOG + 1), garbageValue);
            }









            int totalPower = (int)(garbageValue * GameData.Options.GarbageMultiplier);
            if (Stats.Combo > 2)
                Stats.CurrentComboPower = Math.Max(Stats.CurrentComboPower, totalPower);

            if (clearLineCount > 0 && Stats.Combo > 1 && Stats.CurrentComboPower >= 7)
            {
                //そもそもAddFireって？
                //AddFire
            }

            //火力の相殺をする
            SousaiAttacks(totalPower);
            //リプレイだと火力送信する必要なし、相殺のみ
            if (clearLineCount > 0)
            {
                //FightLines
            }
            else
            {
                TakeAllDamage();
                // TODO: もらった火力を受ける
                //TakeAllDamage
                // TakeAllDamage();
            }

        }

        void TakeAllDamage()
        {
            var receivedDamage = 0;

            var receiveGarbageCount = Garbages.CountCanReceive();
            for (int i = 0; i < receiveGarbageCount; i++)
            {
                receivedDamage += Garbages[0].power;

                if (receivedDamage > GameData.Options.GarbageCap)
                {
                    //receive
                    var receivedValue = (int)GameData.Options.GarbageCap - (receivedDamage - Garbages[0].power);
                    ReceiveGarbage(Garbages[0].posX, receivedValue);
                    Garbages[0].power -= receivedValue;
                    receivedDamage = (int)GameData.Options.GarbageCap;
                    break;
                }
                else
                {
                    //receive
                    ReceiveGarbage(Garbages[0].posX, Garbages[0].power);
                    Garbages.RemoveAt(0);
                }
            }

        }

        void IsBoardEmpty()
        {
            int emptyLineCount = 0;
            for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
            {
                if (emptyLineCount >= 2)
                    break;

                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (GameData.Field[x + y * 10] != (int)MinoKind.Empty)
                        return;

                }

                emptyLineCount++;
            }

            //PC
            //TODO: かけた後変換？
            SousaiAttacks((int)(DataGarbage.ALL_CLEAR * GameData.Options.GarbageMultiplier));



        }

        void SousaiAttacks(int lines)
        {

            while (Garbages.CountCanSousai() != 0 && lines != 0)
            {
                if (Garbages[0].power <= lines)
                {
                    lines -= Garbages[0].power;
                    if (Garbages[0].confirmed_frame == -1)
                        _historyInteraction.Add(Garbages[0]);
                    Garbages.RemoveAt(0);
                }
                else
                {
                    Garbages[0].power -= lines;
                    lines = 0;
                }
            }

        }
    }
}