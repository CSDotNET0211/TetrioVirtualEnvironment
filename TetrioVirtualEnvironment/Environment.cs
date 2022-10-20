using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TetrReplayLoaderLib;
using static System.Net.Mime.MediaTypeNames;

namespace TetrioVirtualEnvironment
{
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

    public class Garbage
    {
        public Garbage(int frame, int posX, int power)
        {
            this.frame = frame;
            this.posX = posX;
            this.power = power;
        }

        public int frame;
        public int posX;
        public int power;

    }

    public class Environment
    {


        public const int FIELD_WIDTH = 10;
        public const int FIELD_HEIGHT = 40;
        public const int FIELD_VIEW_HEIGHT = 20;

        //minokind rotation vec
        static public Vector2[][][] TETRIMINOS;
        static public readonly Vector2[] TETRIMINO_DIFF = new Vector2[7];
        static public readonly int[] MINOTYPES = new int[] { (int)MinoKind.Z, (int)MinoKind.L, (int)MinoKind.O, (int)MinoKind.S, (int)MinoKind.I, (int)MinoKind.J, (int)MinoKind.T, };

        public List<Garbage> Garbages = new List<Garbage>();
        public Dictionary<string, Vector2[]> KICKSET_SRSPLUS { get; private set; }
        public Dictionary<string, Vector2[]> KICKSET_SRSPLUSI { get; private set; }
        public GameData GameDataInstance;
        public Stats StatsInstance;
        public RNG RNG = new RNG();
        public int CurrentFrame = 0;

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

        public Environment(EventFullOptions options)
        {
            new GameData(options, false, this, ref GameDataInstance);
            StatsInstance = new Stats();

            var tempTetriminos = new Vector2[7][][];

            for (int i = 0; i < 7; i++)
            {
                tempTetriminos[i] = new Vector2[4][];
            }

            #region Z
            tempTetriminos[(int)MinoKind.Z][(int)Rotation.Zero] =
                new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1) };

            tempTetriminos[(int)MinoKind.Z][(int)Rotation.Right] =
                new Vector2[] { new Vector2(2, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) };

            tempTetriminos[(int)MinoKind.Z][(int)Rotation.Turn] =
                new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2) };

            tempTetriminos[(int)MinoKind.Z][(int)Rotation.Left] =
                new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 2) };
            #endregion
            #region L
            tempTetriminos[(int)MinoKind.L][(int)Rotation.Zero] =
              new Vector2[] { new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) };

            tempTetriminos[(int)MinoKind.L][(int)Rotation.Right] =
              new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2) };

            tempTetriminos[(int)MinoKind.L][(int)Rotation.Turn] =
              new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2) };

            tempTetriminos[(int)MinoKind.L][(int)Rotation.Left] =
              new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2) };

            #endregion
            #region O
            tempTetriminos[(int)MinoKind.O][(int)Rotation.Zero] =
              new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

            tempTetriminos[(int)MinoKind.O][(int)Rotation.Right] =
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

            tempTetriminos[(int)MinoKind.O][(int)Rotation.Turn] =
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

            tempTetriminos[(int)MinoKind.O][(int)Rotation.Left] =
            new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

            #endregion
            #region S
            tempTetriminos[(int)MinoKind.S][(int)Rotation.Zero] =
            new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1) };

            tempTetriminos[(int)MinoKind.S][(int)Rotation.Right] =
            new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2) };

            tempTetriminos[(int)MinoKind.S][(int)Rotation.Turn] =
            new Vector2[] { new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2), new Vector2(1, 2) };

            tempTetriminos[(int)MinoKind.S][(int)Rotation.Left] =
            new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) };

            #endregion
            #region I
            tempTetriminos[(int)MinoKind.I][(int)Rotation.Zero] =
            new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(3, 1) };
            tempTetriminos[(int)MinoKind.I][(int)Rotation.Right] =
new Vector2[] { new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2), new Vector2(2, 3) };
            tempTetriminos[(int)MinoKind.I][(int)Rotation.Turn] =
new Vector2[] { new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2), new Vector2(3, 2) };
            tempTetriminos[(int)MinoKind.I][(int)Rotation.Left] =
new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(1, 3) };

            #endregion
            #region J
            tempTetriminos[(int)MinoKind.J][(int)Rotation.Zero] =
            new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) };

            tempTetriminos[(int)MinoKind.J][(int)Rotation.Right] =
            new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(1, 2) };

            tempTetriminos[(int)MinoKind.J][(int)Rotation.Turn] =
            new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2) };

            tempTetriminos[(int)MinoKind.J][(int)Rotation.Left] =
            new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 2), new Vector2(1, 2) };

            #endregion
            #region T
            tempTetriminos[(int)MinoKind.T][(int)Rotation.Zero] =
            new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) };

            tempTetriminos[(int)MinoKind.T][(int)Rotation.Right] =
            new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) };

            tempTetriminos[(int)MinoKind.T][(int)Rotation.Turn] =
            new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) };

            tempTetriminos[(int)MinoKind.T][(int)Rotation.Left] =
            new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) };

            #endregion

            TETRIMINOS = tempTetriminos;

            TETRIMINO_DIFF[0] = new Vector2(1, 1);
            TETRIMINO_DIFF[1] = new Vector2(1, 1);
            TETRIMINO_DIFF[2] = new Vector2(0, 1);
            TETRIMINO_DIFF[3] = new Vector2(1, 1);
            TETRIMINO_DIFF[4] = new Vector2(1, 1);
            TETRIMINO_DIFF[5] = new Vector2(1, 1);
            TETRIMINO_DIFF[6] = new Vector2(1, 1);

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


            //GameDataInstance=GameDataInstance;



        }

        public void InputKeyEvent(KeyEvent keyEvent, Action keyKind)
        {
            if (keyEvent == KeyEvent.KeyUp)
            {

            }
            else if (keyEvent == KeyEvent.KeyDown)
            {

            }
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
                if (@event.subframe > GameDataInstance.SubFrame)
                {
                    ProcessShift(false, @event.subframe - GameDataInstance.SubFrame);
                    FallEvent(null, @event.subframe - GameDataInstance.SubFrame);
                    GameDataInstance.SubFrame = @event.subframe;
                }

                if (@event.key == "moveLeft")
                {
                    GameDataInstance.LShift = true;
                    GameDataInstance.LastShift = -1;
                    GameDataInstance.LDas = @event.hoisted ? GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD : 0;
                    if (GameDataInstance.Options.Version >= 12)
                        GameDataInstance.LDasIter = GameDataInstance.Handling.ARR;

                    ProcessLShift(true, GameDataInstance.Options.Version >= 15 ? 0 : 1);
                    return;
                }

                if (@event.key == "moveRight")
                {
                    GameDataInstance.RShift = true;
                    GameDataInstance.LastShift = 1;
                    GameDataInstance.RDas = @event.hoisted ? GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD : 0;
                    if (GameDataInstance.Options.Version >= 12)
                        GameDataInstance.RDasIter = GameDataInstance.Handling.ARR;

                    ProcessRShift(true, GameDataInstance.Options.Version >= 15 ? 0 : 1);
                    return;
                }

                if (@event.key == "softDrop")
                {
                    GameDataInstance.SoftDrop = true;
                    return;
                }

                if (GameDataInstance.Falling.Sleep)
                {
                    throw new Exception("未実装");
                }
                else
                {
                    if (@event.key == "rotateCCW")
                    {
                        var e = GameDataInstance.Falling.r - 1;
                        if (e < 0)
                            e = 3;
                        RotatePiece(e);
                    }

                    if (@event.key == "rotateCW")
                    {
                        var e = GameDataInstance.Falling.r + 1;
                        if (e > 3)
                            e = 0;
                        RotatePiece(e);
                    }

                    if (@event.key == "rotate180" && GameDataInstance.Options.Allow180)
                    {
                        var e = GameDataInstance.Falling.r + 2;
                        if (e > 3)
                            e -= 4;
                        RotatePiece(e);
                    }
                    if (@event.key == "hardDrop" && GameDataInstance.Options.AllowHardDrop &&
                        GameDataInstance.Falling.SafeLock == 0)
                    {
                        FallEvent(int.MaxValue, 1);
                    }

                    if (@event.key == "hold")
                    {
                        if (!GameDataInstance.HoldLocked)
                        {
                            if ((GameDataInstance.Options.DisplayHold == null ||
                            (bool)GameDataInstance.Options.DisplayHold))
                            {
                                SwapHold();
                            }

                        }
                    }
                }

            }
            else if (type == "keyup")
            {
                if (@event.subframe > GameDataInstance.SubFrame)
                {
                    ProcessShift(false, @event.subframe - GameDataInstance.SubFrame);
                    FallEvent(null, @event.subframe - GameDataInstance.SubFrame);
                    GameDataInstance.SubFrame = @event.subframe;
                }

                if (@event.key == "moveLeft")
                {
                    GameDataInstance.LShift = false;
                    GameDataInstance.LDas = 0;

                    if (GameDataInstance.Handling.Cancel)
                    {
                        GameDataInstance.RDas = 0;
                        GameDataInstance.RDasIter = GameDataInstance.Handling.ARR;
                    }

                    return;
                }

                if (@event.key == "moveRight")
                {
                    GameDataInstance.RShift = false;
                    GameDataInstance.RDas = 0;

                    if (GameDataInstance.Handling.Cancel)
                    {
                        GameDataInstance.LDas = 0;
                        GameDataInstance.LDasIter = GameDataInstance.Handling.ARR;
                    }

                    return;
                }

                if (@event.key == "softDrop")
                    GameDataInstance.SoftDrop = false;


            }
        }

        void RotatePiece(int rotation)
        {
            var nowmino_rotation = GameDataInstance.Falling.r;
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

            if (IsLegalAtPos(GameDataInstance.Falling.type,
                GameDataInstance.Falling.x - GameDataInstance.Falling.aox,
                GameDataInstance.Falling.y - GameDataInstance.Falling.aoy, rotation,
                GameDataInstance.Field))
            {
                GameDataInstance.Falling.x -= GameDataInstance.Falling.aox;
                GameDataInstance.Falling.y -= GameDataInstance.Falling.aoy;
                GameDataInstance.Falling.aox = 0;
                GameDataInstance.Falling.aoy = 0;
                GameDataInstance.Falling.r = rotation;
                GameDataInstance.Falling.Last = "rotate";
                GameDataInstance.Falling.LastRotation = i;
                GameDataInstance.Falling.LastKick = 0;
                GameDataInstance.Falling.SpinType = IsTspin();
                GameDataInstance.FallingRotations++;
                GameDataInstance.TotalRotations++;

                if (GameDataInstance.Falling.Clamped && GameDataInstance.Handling.DCD > 0)
                {
                    GameDataInstance.LDas = Math.Min(GameDataInstance.LDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
                    GameDataInstance.LDasIter = GameDataInstance.Handling.ARR;
                    GameDataInstance.RDas = Math.Min(GameDataInstance.RDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
                    GameDataInstance.RDasIter = GameDataInstance.Handling.ARR;
                }

                if (++GameDataInstance.Falling.LockResets < 15 || GameDataInstance.Options.InfiniteMovement)
                    GameDataInstance.Falling.Locking = 0;

                if (IsTspin())
                {
                    // DOTO: t.hm.H.holderstate.dr += o / 4 * t.lm.H.lastdT; これ何

                }

                //落下ミノ更新フラグ true
                return;
            }

            if (GameDataInstance.Falling.type == (int)MinoKind.O)
                return;

            var kicktable = KICKSET_SRSPLUS[nowmino_newmino_rotation];

            if (GameDataInstance.Falling.type == (int)MinoKind.I)
                kicktable = KICKSET_SRSPLUSI[nowmino_newmino_rotation];

            for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
            {
                var kicktableTest = kicktable[kicktableIndex];
                var i2 = (int)(GameDataInstance.Falling.y) + 0.1 +
                    kicktableTest.y - GameDataInstance.Falling.aoy;


                if (!GameDataInstance.Options.InfiniteMovement &&
                    GameDataInstance.TotalRotations > (int)GameDataInstance.Options.LockResets + 15)
                {
                    i2 = GameDataInstance.Falling.y + kicktableTest.y - GameDataInstance.Falling.aoy;
                }

                if (IsLegalAtPos(GameDataInstance.Falling.type,
                    GameDataInstance.Falling.x + (int)kicktableTest.x - GameDataInstance.Falling.aox,
                    i2, rotation, GameDataInstance.Field))
                {

                    GameDataInstance.Falling.x += (int)kicktableTest.x - GameDataInstance.Falling.aox;
                    GameDataInstance.Falling.y = i2;
                    GameDataInstance.Falling.aox = 0;
                    GameDataInstance.Falling.aoy = 0;
                    GameDataInstance.Falling.r = rotation;
                    GameDataInstance.Falling.SpinType = IsTspin();
                    GameDataInstance.Falling.LastKick = kicktableIndex + 1;
                    GameDataInstance.Falling.Last = "rotate";
                    GameDataInstance.FallingRotations++;
                    GameDataInstance.TotalRotations++;

                    if (GameDataInstance.Falling.Clamped && GameDataInstance.Handling.DCD > 0)
                    {
                        GameDataInstance.LDas = Math.Min(GameDataInstance.LDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
                        GameDataInstance.LDasIter = GameDataInstance.Handling.ARR;
                        GameDataInstance.RDas = Math.Min(GameDataInstance.RDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
                        GameDataInstance.RDasIter = GameDataInstance.Handling.ARR;
                    }

                    if (++GameDataInstance.Falling.LockResets < 15 || GameDataInstance.Options.InfiniteMovement)
                        GameDataInstance.Falling.Locking = 0;

                    if (IsTspin())
                    {
                        // DOTO: t.hm.H.holderstate.dr += o / 4 * t.lm.H.lastdT;
                    }

                    // DOTO: 更新フラグ true
                    return;
                }
            }


        }

        public void ProcessShift(bool value, double subFrameDiff)
        {
            ProcessLShift(value, subFrameDiff);
            ProcessRShift(value, subFrameDiff);

        }

        void ProcessLShift(bool value, double subFrameDiff = 1)
        {
            if (!GameDataInstance.LShift || GameDataInstance.RShift && GameDataInstance.LastShift != -1)
                return;

            var subFrameDiff2 = subFrameDiff;
            var dasSomething = Math.Max(0, GameDataInstance.Handling.DAS - GameDataInstance.LDas);

            if (!value)
                GameDataInstance.LDas += subFrameDiff;

            if (GameDataInstance.LDas < GameDataInstance.Handling.DAS && !value)
                return;

            subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);

            if (GameDataInstance.Falling.Sleep || GameDataInstance.Falling.DeepSleep)
                return;

            var aboutARRValue = 1;
            if (!value)
            {
                if (GameDataInstance.Options.Version >= 15)
                    GameDataInstance.LDasIter = subFrameDiff2;
                else
                    GameDataInstance.LDasIter = subFrameDiff;

                if (GameDataInstance.LDasIter < GameDataInstance.Handling.ARR)
                    return;

                if (GameDataInstance.Handling.ARR == 0)
                    aboutARRValue = 10;
                else
                    aboutARRValue = (int)(GameDataInstance.LDasIter / GameDataInstance.Handling.ARR);

                GameDataInstance.LDasIter -= GameDataInstance.Handling.ARR * aboutARRValue;
            }

            for (var index = 0; index < aboutARRValue; index++)
            {
                if (IsLegalAtPos(GameDataInstance.Falling.type, GameDataInstance.Falling.x - 1, GameDataInstance.Falling.y, GameDataInstance.Falling.r, GameDataInstance.Field))
                {
                    GameDataInstance.Falling.x--;
                    GameDataInstance.Falling.Last = "move";
                    GameDataInstance.Falling.Clamped = false;

                    if (++GameDataInstance.Falling.LockResets < 15 || GameDataInstance.Options.InfiniteMovement)
                        GameDataInstance.Falling.Locking = 0;

                }
                else
                {
                    GameDataInstance.Falling.Clamped = true;
                }
            }
        }

        void ProcessRShift(bool value, double subFrameDiff = 1)
        {
            if (!GameDataInstance.RShift || GameDataInstance.LShift && GameDataInstance.LastShift != 1)
                return;

            var subFrameDiff2 = subFrameDiff;
            var o = Math.Max(0, GameDataInstance.Handling.DAS - GameDataInstance.RDas);

            if (!value)
                GameDataInstance.RDas += subFrameDiff;

            if (GameDataInstance.RDas < GameDataInstance.Handling.DAS && !value)
                return;

            subFrameDiff2 = Math.Max(0, subFrameDiff2 - o);
            if (GameDataInstance.Falling.Sleep || GameDataInstance.Falling.DeepSleep)
                return;

            var aboutARRValue = 1;
            if (!value)
            {
                if (GameDataInstance.Options.Version >= 15)
                    GameDataInstance.RDasIter = subFrameDiff2;
                else
                    GameDataInstance.RDasIter = subFrameDiff;

                if (GameDataInstance.RDasIter < GameDataInstance.Handling.ARR)
                    return;

                if (GameDataInstance.Handling.ARR == 0)
                    aboutARRValue = 10;
                else
                    aboutARRValue = (int)(GameDataInstance.RDasIter / GameDataInstance.Handling.ARR);

                GameDataInstance.RDasIter -= GameDataInstance.Handling.ARR * aboutARRValue;
            }

            for (var index = 0; index < aboutARRValue; index++)
            {
                if (IsLegalAtPos(GameDataInstance.Falling.type, GameDataInstance.Falling.x + 1, GameDataInstance.Falling.y, GameDataInstance.Falling.r, GameDataInstance.Field))
                {
                    GameDataInstance.Falling.x++;
                    GameDataInstance.Falling.Last = "move";
                    GameDataInstance.Falling.Clamped = false;

                    if (++GameDataInstance.Falling.LockResets < 15 || GameDataInstance.Options.InfiniteMovement)
                        GameDataInstance.Falling.Locking = 0;

                }
                else
                {
                    GameDataInstance.Falling.Clamped = true;
                }
            }
        }

        public void FallEvent(int? value, double subFrameDiff)
        {
            if (GameDataInstance.Falling.SafeLock > 0)
                GameDataInstance.Falling.SafeLock--;

            if (GameDataInstance.Falling.Sleep || GameDataInstance.Falling.DeepSleep)
                return;

            var subframeGravity = GameDataInstance.Gravity * subFrameDiff;

            if (GameDataInstance.SoftDrop)
            {
                if (GameDataInstance.Options.Version >= 15 && GameDataInstance.Handling.SDF == 41)
                    subframeGravity = 400 * subFrameDiff;
                else if (!(GameDataInstance.Options.Version >= 15) && GameDataInstance.Handling.SDF == 21)
                    subframeGravity = 20 * subFrameDiff;
                else
                {
                    subframeGravity *= GameDataInstance.Handling.SDF;
                    int tempvalue;
                    if (GameDataInstance.Options.Version >= 13)
                        tempvalue = 1;
                    else
                        tempvalue = 0;

                    if (Math.Max(subframeGravity, tempvalue) > 0)
                        subframeGravity = 0.05 * GameDataInstance.Handling.SDF;
                    else
                        subframeGravity = 0.42;
                }
            }

            if (value != null)
                subframeGravity = (int)value;

            if (!GameDataInstance.Options.InfiniteMovement &&
                GameDataInstance.Falling.LockResets >= (int)GameDataInstance.Options.LockResets &&
                !IsLegalAtPos(GameDataInstance.Falling.type, GameDataInstance.Falling.x,
                GameDataInstance.Falling.y + 1, GameDataInstance.Falling.r, GameDataInstance.Field))
            {
                subframeGravity = 20;
                GameDataInstance.Falling.ForceLock = true;
            }


            if (!GameDataInstance.Options.InfiniteMovement &&
                GameDataInstance.FallingRotations > (int)GameDataInstance.Options.LockResets + 15)
            {
                subframeGravity += 0.5 * subFrameDiff *
                    (GameDataInstance.FallingRotations - ((int)GameDataInstance.Options.LockResets + 15));
            }

            var r = subframeGravity;

            for (; subframeGravity > 0;)
            {
                var ceiledValue = Math.Ceiling(GameDataInstance.Falling.y);
                if (!HardDrop(Math.Min(1, subframeGravity), r))
                {
                    if (value != null)
                        GameDataInstance.Falling.ForceLock = true;
                    FunctionA(value != 0, subFrameDiff);
                    break;
                }

                subframeGravity -= Math.Min(1, subframeGravity);
                if (ceiledValue != Math.Ceiling(GameDataInstance.Falling.y))
                {
                    GameDataInstance.Falling.Last = "fall";
                    if (GameDataInstance.SoftDrop)
                    {
                        //ScoreAdd

                    }
                }
            }
        }

        bool HardDrop(double value, double value2)
        {
            var fallingy_kouho = Math.Floor(Math.Pow(10, 13) * GameDataInstance.Falling.y) /
                Math.Pow(10, 13) + value;

            if (fallingy_kouho % 1 == 0)
                fallingy_kouho += 0.00001;

            var o = Math.Floor(Math.Pow(10, 13) * GameDataInstance.Falling.y) / Math.Pow(10, 13) + 1;
            if (o % 1 == 0)
                o -= 0.00002;

            if (IsLegalAtPos(GameDataInstance.Falling.type, GameDataInstance.Falling.x, fallingy_kouho, GameDataInstance.Falling.r, GameDataInstance.Field) &&
                IsLegalAtPos(GameDataInstance.Falling.type, GameDataInstance.Falling.x, o, GameDataInstance.Falling.r, GameDataInstance.Field))
            {
                var s = GameDataInstance.Falling.HighestY;
                o = GameDataInstance.Falling.y;

                GameDataInstance.Falling.y = fallingy_kouho;
                GameDataInstance.Falling.HighestY = Math.Ceiling(Math.Max(GameDataInstance.Falling.HighestY, fallingy_kouho));
                GameDataInstance.Falling.Floored = false;
                if (Math.Ceiling(fallingy_kouho) != Math.Ceiling(o))
                {
                    // TODO: 更新フラグtrue

                }

                if (fallingy_kouho > s || GameDataInstance.Options.InfiniteMovement)
                    GameDataInstance.Falling.LockResets = 0;
                GameDataInstance.FallingRotations = 0;

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
            GameDataInstance.Falling.Sleep = true;
            //ミノを盤面に適用
            foreach (var pos in TETRIMINOS[GameDataInstance.Falling.type][GameDataInstance.Falling.r])
            {
                GameDataInstance.Field[(int)((pos.x + GameDataInstance.Falling.x - TETRIMINO_DIFF[GameDataInstance.Falling.type].x) +
                    (int)(pos.y + GameDataInstance.Falling.y - TETRIMINO_DIFF[GameDataInstance.Falling.type].y) * 10)] = GameDataInstance.Falling.type;
            }

            if (!sValue && GameDataInstance.Handling.SafeLock > 0)
                GameDataInstance.Falling.SafeLock = 7;

            var clearedLineCount = ClearLine();


            int tempGargabeCount = 0;

            while (Garbages.Count != 0 || tempGargabeCount == GameDataInstance.Options.GarbageCap)
            {
                //足しても大丈夫なら全部取得
                //ダメならCAPになるまで
                if (Garbages.Count != 0 &&
                    Garbages[0].frame + GameDataInstance.Options.GarbageSpeed - 1 <= CurrentFrame)
                {
                    if (tempGargabeCount + Garbages[0].power <= GameDataInstance.Options.GarbageCap)
                    {
                        ReceiveGarbage(Garbages[0].posX, Garbages[0].power);
                        tempGargabeCount += Garbages[0].power;
                        Garbages.RemoveAt(0);
                    }
                    else if (GameDataInstance.Options.GarbageCap - tempGargabeCount > 0)
                    {
                        ReceiveGarbage(Garbages[0].posX, GameDataInstance.Options.GarbageCap - tempGargabeCount);
                        Garbages[0].power -= GameDataInstance.Options.GarbageCap - tempGargabeCount;

                        tempGargabeCount += GameDataInstance.Options.GarbageCap - tempGargabeCount;
                        break;
                    }
                    else
                        break;

                }
                else
                    break;
            }



            GameDataInstance.Falling.Init(null);




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
                    if (GameDataInstance.Field[x + y * 10] == (int)MinoKind.Empty)
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
                DownLine(value, GameDataInstance.Field);
            }

            return list.Count;

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
            if (GameDataInstance.Options.Version >= 15)
                GameDataInstance.Falling.Locking += subframe;
            else
                GameDataInstance.Falling.Locking += 1;

            if (!GameDataInstance.Falling.Floored)
                GameDataInstance.Falling.Floored = true;



            if (GameDataInstance.Falling.Locking > GameDataInstance.Options.LockTime ||
                GameDataInstance.Falling.ForceLock ||
                GameDataInstance.Falling.LockResets > GameDataInstance.Options.LockResets &&
                !GameDataInstance.Options.InfiniteMovement)
            {
                PiecePlace(value);




            }
        }

        public int RefreshNext(int[] next, bool noszo)
        {
            var value = next[0];

            for (int i = 0; i < next.Length - 1; i++)
            {
                next[i] = next[i + 1];
            }

            if (GameDataInstance.NextBag.Count == 0)
            {
                var array = (int[])MINOTYPES.Clone();
                RNG.ShuffleArray(array);
                GameDataInstance.NextBag.AddRange(array);
            }

            if (noszo)
            {
                while (true)
                {
                    if (GameDataInstance.NextBag[0] == (int)MinoKind.S ||
                         GameDataInstance.NextBag[0] == (int)MinoKind.Z ||
                        GameDataInstance.NextBag[0] == (int)MinoKind.O)
                    {
                        var temp = GameDataInstance.NextBag[0];
                        for (int i = 0; i < GameDataInstance.NextBag.Count - 1; i++)
                        {
                            GameDataInstance.NextBag[i] = GameDataInstance.NextBag[i + 1];
                        }
                        GameDataInstance.NextBag[GameDataInstance.NextBag.Count - 1] = temp;
                    }
                    else
                        break;
                }

            }


            next[next.Length - 1] = GameDataInstance.NextBag[0];
            GameDataInstance.NextBag.RemoveAt(0);
            return value;
        }

        void SwapHold()
        {
            var s = GameDataInstance.Falling.type;
            GameDataInstance.Falling.Init(GameDataInstance.Hold);
            GameDataInstance.Hold = s;
        }

        bool IsTspin()
        {
            return false;
            //switch(GameDataInstance.Options.bou)

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
                        GameDataInstance.Field[x + (y) * 10] = GameDataInstance.Field[x + (y + power) * 10];
                }
            }


            for (int y = FIELD_HEIGHT - 1; y > FIELD_HEIGHT - 1 - power; y--)
            {
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (x == garbageX)
                        GameDataInstance.Field[x + (y) * 10] = (int)MinoKind.Empty;
                    else
                        GameDataInstance.Field[x + (y) * 10] = (int)MinoKind.Ojama;
                }
            }
        }

        public void GetAttackPower(int clearLineCount, string? isTspin)
        {
            var isBTB = false;

            if (clearLineCount > 0)
            {
                StatsInstance.Combo++;
                StatsInstance.TopCombo = Math.Max(StatsInstance.Combo, StatsInstance.TopCombo);

                if (clearLineCount == 4)
                    isBTB = true;
                else
                {
                    if (isTspin != null)
                        isBTB = true;
                }

                if (isBTB)
                {
                    StatsInstance.BTB++;
                    StatsInstance.TopBTB = Math.Max(StatsInstance.BTB, StatsInstance.TopBTB);
                }
                else
                {
                    //btb updateの中身何
                    StatsInstance.BTB = 0;

                }

            }
            else
            {
                StatsInstance.Combo = 0;
                StatsInstance.CurrentComboPower = 0;
            }


            var garbageValue = 0;
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

            if (clearLineCount > 0 && StatsInstance.BTB > 1)
            {
                if (GameDataInstance.Options.BTBChaining)
                {
                    double tempValue;
                    if (StatsInstance.BTB - 1 == 1)
                        tempValue = 0;
                    else
                        tempValue = 1 + (Math.Log((StatsInstance.BTB - 1) * DataGarbage.BACKTOBACK_BONUS_LOG + 1) % 1);

                    var btb_bonus = DataGarbage.BACKTOBACK_BONUS *
                        (Math.Floor(1 + Math.Log((StatsInstance.BTB - 1) * DataGarbage.BACKTOBACK_BONUS_LOG + 1)) + (tempValue / 3));

                    garbageValue += btb_bonus;

                    if ((int)btb_bonus >= 2)
                    {
                        //AddFire
                    }

                    if ((int)btb_bonus > GameDataInstance.CurrentBTBChainPower)
                    {
                        GameDataInstance.CurrentBTBChainPower = (int)btb_bonus;
                    }



                }
                else
                    garbageValue += DataGarbage.BACKTOBACK_BONUS;
            }
            else
            {
                if (clearLineCount > 0 & StatsInstance.BTB <= 1)
                    GameDataInstance.CurrentBTBChainPower = 0;
            }

            if (StatsInstance.Combo > 1)
            {
                garbageValue *= 1 + DataGarbage.COMBO_BONUS * (StatsInstance.Combo - 1);
            }

            if (StatsInstance.Combo > 2)
            {
                garbageValue = Math.Max(Math.Log(DataGarbage.COMBO_MINIFIER *
                    (StatsInstance.Combo - 1) *
                    DataGarbage.COMBO_MINIFIER_LOG + 1), garbageValue);
            }

            var l = Math.Floor(garbageValue * GameDataInstance.Options.GarbageMultiplier);
            if (StatsInstance.Combo > 2)
                StatsInstance.CurrentComboPower = Math.Max(StatsInstance.CurrentComboPower, l);

            if (clearLineCount > 0 && StatsInstance.Combo > 1 && StatsInstance.CurrentComboPower >= 7)
            {
                //そもそもAddFireって？
                //AddFire
            }

            //火力の相殺をする
            SousaiAttacks(l);

            //リプレイだと火力送信する必要なし、相殺のみ
            if (clearLineCount > 0)
            {
                //FightLines
            }
            else
            {
                //TakeAllDamage
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
                    if (GameDataInstance.Field[x + y * 10] != (int)MinoKind.Empty)
                        return;

                }

                emptyLineCount++;
            }

            //PC

            SousaiAttacks(DataGarbage.ALL_CLEAR*GameDataInstance.Options.GarbageMultiplier);



        }

        void SousaiAttacks(int lines)
        {
            while (Garbages.Count > 0)
            {
                if (lines > 0)
                {
                    if (Garbages[0].power - lines <= 0)
                    {
                        lines -= Garbages[0].power;
                        Garbages.RemoveAt(0);
                    }

                }
            }
        }
    }
}