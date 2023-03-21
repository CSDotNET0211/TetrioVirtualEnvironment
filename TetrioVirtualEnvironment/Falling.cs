using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment
{
   public class Falling
    {
        public Falling(Environment environment, GameData gameData)
        {
            Sleep = true;
            DeepSleep = false;
            Locking = 0;
            LockResets = 0;
            ForceLock = false;
            Floored = false;
            Clamped = false;
            SafeLock = 0;
            x = 4;
            y = 14;
            r = 0;
            type = -1;
            HighestY = 14;
            Last = null;
            LastKick = 0;
            LastRotation = "none";
            irs = 0;
            ihs = 0;
            aox = 0;
            aoy = 0;

            EnvironmentInstance = environment;
            GameDataInstance = gameData;
        }

        public Falling(int type,int x,int y, int r)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.r = r;

        }

     

        public void Init(int? newtype,EnvironmentModeEnum environmentMode)
        {
            Locking = 0;
            ForceLock = false;
            LockResets = 0;
            Floored = false;

            if (newtype == null)
            {
                if(environmentMode==EnvironmentModeEnum.Limited)
                {
                    if (EnvironmentInstance._gameData.Next.Count != 0)
                    {
                        type = EnvironmentInstance._gameData.Next[0];
                        EnvironmentInstance._gameData.Next.RemoveAt(0);
                    }else
                    {
                        type=(int)MinoKind.Empty;
                        return;
                    }


                }
                else
                type = EnvironmentInstance.RefreshNext(GameDataInstance.Next, false);
            }
            else
                type = (int)newtype;

            aox = 0;
            aoy = 0;
            x = 4;
            y = 20 - 2.04+1;
            HighestY = 20 - 2;
            r = 0;

            SpinType = null;
            Sleep = false;
            Last = null;
            GameDataInstance.FallingRotations = 0;
            GameDataInstance.TotalRotations = 0;

            if (newtype != null)
                GameDataInstance.HoldLocked = true;
            else
                GameDataInstance.HoldLocked = false;

            if (Clamped && GameDataInstance.Handling.DCD > 0)
            {
                GameDataInstance.LDas = Math.Min(GameDataInstance.LDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
                GameDataInstance.LDasIter = GameDataInstance.Handling.ARR;
                GameDataInstance.RDas = Math.Min(GameDataInstance.RDas, GameDataInstance.Handling.DAS - GameDataInstance.Handling.DCD);
                GameDataInstance.RDasIter = GameDataInstance.Handling.ARR;
            }

            Clamped = false;

            if (!IsLegalAtPos(type, x, y, r, GameDataInstance.Field))
            {
                Console.WriteLine("You are dead");
            }


            EnvironmentInstance.CallOnPieceCreated();



        }

        public Environment EnvironmentInstance;
        public GameData GameDataInstance;
        public int type;
        public int x;
        public int aox;
        public double y;
        public int aoy;
        public int r;
        public int irs;
        public int ihs;
        public int SafeLock;
        public bool ForceLock;
        public bool Sleep;
        public bool DeepSleep;
        public string Last;
        public string LastRotation;
        public string? SpinType;
        public int LastKick;
        public bool Clamped;
        public int LockResets;
        public double Locking;
        public bool Floored;
        public double HighestY;
    }

    public static class FallingEx {
        public static Falling Clone(this Falling falling)
        {
            var value = new Falling(falling.type, falling.x, (int)falling.y, falling.r);
            return value;
        }
    }
}
