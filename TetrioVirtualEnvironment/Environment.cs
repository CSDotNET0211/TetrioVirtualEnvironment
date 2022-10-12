using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TetrReplayLoaderLib;

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

    public class Environment
    {
        public const int FIELD_WIDTH = 10;
        public const int FIELD_HEIGHT = 40;
        public const int FIELD_VIEW_HEIGHT = 20;

        //minokind rotation vec
      static  public  Vector2[][][] TETRIMINOS;
        static  public readonly string[] MINOTYPES = new string[] { "z", "l", "o", "s", "i", "j", "t", };

        public readonly Dictionary<string, Vector2[]> KICKSET_SRSPLUS;
        public readonly Dictionary<string, Vector2[]> KICKSET_SRSPLUSI;
        public GameData GameData;

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



        public Environment()
        {
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
            GameData = new GameData();



        }









       static public bool IsLegalAtPos(int type, int x, double y, int rotation, int[] field)
        {
            var positions = TETRIMINOS[type][rotation];

            foreach (var position in positions)
            {
                if (!(x >= 0 && x < FIELD_WIDTH &&
                  y >= 0 && y < FIELD_HEIGHT &&
                     field[x + (int)y * 10] == (int)MinoKind.Empty))
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
                    GameData.LDas = @event.hoisted ? GameData.Handling.DAS - GameData.Handling.DCD : 0;
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
                if (@event.key == "hardDrop" && GameData.Options.AllowHardDrop && GameData.Falling.SafeLock == 0)
                {
                    FallEvent(int.MaxValue, 1);
                }

                if (@event.key == "hold")
                {
                    if (!GameData.HoldLocked && (GameData.Options.DisplayHold == null || (bool)GameData.Options.DisplayHold))
                    {
                        SwapHold();
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

            if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x - GameData.Falling.aox, GameData.Falling.y - GameData.Falling.aoy, GameData.Falling.r, GameData.Field))
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

                if (IsTspin())
                {
                    // DOTO: t.hm.H.holderstate.dr += o / 4 * t.lm.H.lastdT; これ何

                }

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
                var i2 = (int)(GameData.Falling.y) + 0.1 + kicktableTest.y - GameData.Falling.aoy;

                int templockresets;
                if (GameData.Options.LockResets != null)
                    templockresets = (int)GameData.Options.LockResets;
                else
                    templockresets = 15;

                if (!GameData.Options.InfiniteMovement && GameData.TotalRotations > templockresets + 15)
                {
                    i2 = GameData.Falling.y + kicktableTest.y - GameData.Falling.aoy;
                }

                if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x+(int)kicktableTest.x- GameData.Falling.aox,i2, GameData.Falling.r, GameData.Field))
                {
                    GameData.Falling.x += (int)kicktableTest.x - GameData.Falling.aox;
                    GameData.Falling.aox = 0;
                    GameData.Falling.aoy = 0;
                    GameData.Falling.r = rotation;
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

                    if (IsTspin())
                    {
                        // DOTO: t.hm.H.holderstate.dr += o / 4 * t.lm.H.lastdT;
                    }

                    // DOTO: 更新フラグ true
                    return;
                }
            }


        }

        public void ProcessShift(bool value, double subFrameDiff = 1)
        {
            ProcessLShift(value, subFrameDiff);
            ProcessRShift(value, subFrameDiff);

        }

        void ProcessLShift(bool value, double subFrameDiff = 1)
        {
            if (!GameData.LShift || GameData.RShift && GameData.LastShift != -1)
                return;

            var subFrameDiff2 = subFrameDiff;
            var dasSomething = Math.Max(0, GameData.Handling.DAS - GameData.LDas);

            if (!value)
                GameData.LDas += subFrameDiff;

            if (GameData.LDas < GameData.Handling.DAS && !value)
                return;

            subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);
            if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
                return;

            var aboutARRValue = 1;
            if (!value)
            {
                if (GameData.Options.Version >= 15)
                    GameData.LDasIter = subFrameDiff2;
                else
                    GameData.LDasIter = subFrameDiff;

                if (GameData.LDasIter < GameData.Handling.ARR)
                    return;

                if (GameData.Handling.ARR == 0)
                    aboutARRValue = 10;
                else
                    aboutARRValue = (int)(GameData.LDasIter / GameData.Handling.ARR);

                GameData.LDasIter -= GameData.Handling.ARR * aboutARRValue;
            }

            for (var index = 0; index < aboutARRValue; index++)
            {
                if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x-1, GameData.Falling.y, GameData.Falling.r, GameData.Field))
                {
                    GameData.Falling.x--;
                    GameData.Falling.Last = "move";
                    GameData.Falling.Clamped = false;

                    // TODO: 落下ミノの更新フラグ？の確認とフラグを立てる

                    if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking = 0;

                }
                else
                {
                    GameData.Falling.Clamped = true;
                    //TODO: holderstate.dxのやつ
                }
            }
        }

        void ProcessRShift(bool value, double subFrameDiff = 1)
        {
            if (!GameData.RShift || GameData.LShift && GameData.LastShift != 1)
                return;

            var subFrameDiff2 = subFrameDiff;
            var o = Math.Max(0, GameData.Handling.DAS - GameData.RDas);

            if (!value)
                GameData.RDas += subFrameDiff;

            if (GameData.RDas < GameData.Handling.DAS && !value)
                return;

            subFrameDiff2 = Math.Max(0, subFrameDiff2 - o);
            if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
                return;

            var aboutARRValue = 1;
            if (!value)
            {
                if (GameData.Options.Version >= 15)
                    GameData.RDasIter = subFrameDiff2;
                else
                    GameData.RDasIter = subFrameDiff;

                if (GameData.RDasIter < GameData.Handling.ARR)
                    return;

                if (GameData.Handling.ARR == 0)
                    aboutARRValue = 10;
                else
                    aboutARRValue = (int)(GameData.RDasIter / GameData.Handling.ARR);

                GameData.RDasIter -= GameData.Handling.ARR * aboutARRValue;
            }

            for (var index = 0; index < aboutARRValue; index++)
            {
                if (IsLegalAtPos(GameData.Falling.type, GameData.Falling.x+1, GameData.Falling.y, GameData.Falling.r, GameData.Field))
                {
                    GameData.Falling.x++;
                    GameData.Falling.Last = "move";
                    GameData.Falling.Clamped = false;

                    // TODO: フラグのやつ

                    if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking = 0;

                }
                else
                {
                    GameData.Falling.Clamped = true;
                    //TODO: holderstateのやつ
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
                if (GameData.Options.Version >= 15 && GameData.Handling.SDF == 41)
                    subframeGravity = 400 * subFrameDiff;
                else if (!(GameData.Options.Version >= 15) && GameData.Handling.SDF == 21)
                    subframeGravity = 20 * subFrameDiff;
                else
                {
                    subframeGravity *= GameData.Handling.SDF;
                    int tempvalue;
                    if (GameData.Options.Version >= 13)
                        tempvalue = 1;
                    else
                        tempvalue = 0;

                    if (Math.Max(subframeGravity, tempvalue) > 0)
                        subframeGravity = 0.05 * GameData.Handling.SDF;
                    else
                        subframeGravity = 0.42;
                }
            }

            if (value != null)
            {
                subframeGravity = (int)value;

            }

            int templockresets;
            if (GameData.Options.LockResets != null)
                templockresets = (int)GameData.Options.LockResets;
            else
                templockresets = 15;


            if (!GameData.Options.InfiniteMovement &&
                GameData.Falling.LockResets >= templockresets &&
                !IsLegalAtPos(GameData.Falling.type, GameData.Falling.x, GameData.Falling.y+1, GameData.Falling.r, GameData.Field))
            {
                subframeGravity = 20;
                GameData.Falling.ForceLock = true;
            }


            if (!GameData.Options.InfiniteMovement &&
                GameData.FallingRotations > templockresets + 15)
            {
                subframeGravity += 0.5 * subFrameDiff *
                    (GameData.FallingRotations - (templockresets + 15));
            }

            var r = subframeGravity;

            for (; subframeGravity > 0;)
            {
                var ceiledValue = Math.Ceiling(GameData.Falling.y);
                if (!HardDrop(Math.Min(1, subframeGravity), r))
                {
                    if (value != null)
                        GameData.Falling.ForceLock = true;
                    FunctionA(value != 0, (int)subFrameDiff);
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
            var fallingy_kouho = Math.Floor(Math.Pow(10, 13) * GameData.Falling.y) / Math.Pow(10, 13) + value;

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

                if (fallingy_kouho > value || GameData.Options.InfiniteMovement)
                    GameData.Falling.LockResets = 0;
                GameData.FallingRotations = 0;

                if (value2 >= int.MaxValue)
                {
                    //スコア足す
                }
            }

            return true;
        }

        void FunctionA(bool value = false, int a = 1)
        {
            if (GameData.Options.Version >= 15)
                GameData.Falling.Locking += a;
            else
                GameData.Falling.Locking += 1;

            if (!GameData.Falling.Floored)
                GameData.Falling.Floored = true;

        }

        void SwapHold()
        {

        }

        bool IsTspin()
        {
            return false;
        }
    }
}