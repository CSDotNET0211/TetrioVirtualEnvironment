using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Init(int? newtype)
        {
            Locking = 0;
            ForceLock = false;
            LockResets = 0;
            Floored = false;

            if (newtype == null)
            {
                type = EnvironmentInstance.RefreshNext(GameDataInstance.Next, false);
            }
            else
                type = (int)newtype;

            aox = 0;
            aoy = 0;
            x = 4;
            y = 20 - 2.04;
            HighestY = 20 - 2;
            r = 0;

            SpinType = false;
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

            if (Environment.IsLegalAtPos(type, x, y, r, GameDataInstance.Field))
            {
                Console.WriteLine("You are dead");
            }
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
        public bool SpinType;
        public int LastKick;
        public bool Clamped;
        public int LockResets;
        public double Locking;
        public bool Floored;
        public double HighestY;
    }
}
