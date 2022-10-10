using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using TetrReplayLoaderLib;

namespace TetrioVirtualEnvironment
{
    public struct Vector2
    {
        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;

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


        public GameData GameData;

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
            GameData = new GameData();
        }









        bool IsLegalAtPos()
        {

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

            if ()

        }

        public void ProcessShift(bool value, float subFrameDiff = 1)
        {
            ProcessLShift(value, subFrameDiff);
            ProcessRShift(value, subFrameDiff);

        }

        void ProcessLShift(bool value, float subFrameDiff = 1)
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
                if (IsLegalAtPos())
                {
                    GameData.Falling.x--;
                    GameData.Falling.Last = "move";
                    GameData.Falling.Clamped = false;
                    //if(index==0)
                    // TODO: 落下ミノの更新フラグ？の確認とフラグを立てる

                    if (++GameData.Falling.LockResets < 15 || GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking = 0;

                }
                else
                {
                    GameData.Falling.Clamped = true;

                }
            }
        }

        void ProcessRShift(bool value, float subFrameDiff=1)
        {
            if(!GameData.RShift||GameData.LShift&&GameData.LastShift!=1)
                return;

            var subFrameDiff2=subFrameDiff;
            var o=Math.Max(0,GameData.Handling.DAS-GameData.RDas);

            if(!value)
                GameData.RDas+=subFrameDiff;

            if(GameData.RDas<GameData.Handling.DAS&&!value)
                return;

            subFrameDiff2=Math.Max(0,subFrameDiff2-o);
            if(GameData.Falling.Sleep||GameData.Falling.DeepSleep)
                return;

            var aboutARRValue=1;
            if(!value)
            {
                if(GameData.Options.Version>=15)
                    GameData.RDasIter=subFrameDiff2;
                else
                    GameData.RDasIter=subFrameDiff;

                if(GameData.RDasIter<GameData.Handling.ARR)
                    return;

                if(GameData.Handling.ARR==0)
                    aboutARRValue=10;
                else
                    aboutARRValue=(int)(GameData.RDasIter/GameData.Handling.ARR);

                GameData.RDasIter-=GameData.Handling.ARR*aboutARRValue;
            }

            for(var index=0;index<aboutARRValue;index++)
            {
                if(IsLegalAtPos())
                {
                    GameData.Falling.x++;
                    GameData.Falling.Last="move";
                    GameData.Falling.Clamped=false;

                    // TODO: フラグのやつ

                    if(++GameData.Falling.LockResets<15||GameData.Options.InfiniteMovement)
                        GameData.Falling.Locking=0;

                }
                else
                {
                    GameData.Falling.Clamped=true;
                }
            }
        }

   public     void FallEvent(int? value, float subFrameDiff)
        {

        }

        void SwapHold()
        {

        }


    }
}