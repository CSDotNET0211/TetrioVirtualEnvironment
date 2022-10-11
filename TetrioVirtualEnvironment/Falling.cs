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
            x=4;
            y=17;
            r=0;
            // TODO: 色々初期化
        }


        public int type;
        public int x;
        public float y;
        public int r;
        public int SafeLock;
        public bool ForceLock;
        public bool Sleep;
        public bool DeepSleep;
        public string Last;
        public bool Clamped;
        public int LockResets;
        public int Locking;
    }
}
