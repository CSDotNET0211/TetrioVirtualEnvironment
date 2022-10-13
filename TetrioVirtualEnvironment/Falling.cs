using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class Falling
    {
        public Falling()
        {
            x = 4;
            y = 17;
            r = 0;
            // TODO: 色々初期化
        }

        public void Init(int? newtype)
        {
            Locking = 0;
            ForceLock = false;
            LockResets = 0;
            Floored = false;


            aox = 0;
            aoy = 0;
            x = 4;
            y = 20 - 2.04;
            HighestY = 20 - 2;
            r = 0;
            SpinType = false;
            Sleep = false;
            Last = null;
            GameData.Instance.FallingRotations = 0;
            GameData.Instance.TotalRotations = 0;

            if (newtype != null)
                GameData.Instance.HoldLocked = true;
            else
                GameData.Instance.HoldLocked = false;

            if (Clamped && GameData.Instance.Handling.DCD > 0)
            {
                GameData.Instance.LDas = Math.Min(GameData.Instance.LDas, GameData.Instance.Handling.DAS - GameData.Instance.Handling.DCD);
                GameData.Instance.LDasIter = GameData.Instance.Handling.ARR;
                GameData.Instance.RDas = Math.Min(GameData.Instance.RDas, GameData.Instance.Handling.DAS - GameData.Instance.Handling.DCD);
                GameData.Instance.RDasIter = GameData.Instance.Handling.ARR;
            }

            Clamped = false;

            if (Environment.IsLegalAtPos(type, x, y, r, GameData.Instance.Field))
            {

            }
        }

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
        public int Locking;
        public bool Floored;
        public double HighestY;
    }
}
