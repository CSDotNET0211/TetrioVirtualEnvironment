using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    internal class Options
    {
        public int Version;
        public int Seed;
        public bool HasGarbage;
        public int GravityMargin;
        public float GravityIncrease;
        public int GarbageMultiplier;
        public int GarbageMargin;
        public float GarbageIncrease;
        public bool InfiniteMovement;
        public int GarbageCap;
        public bool Allow180;
        public bool AllowHardDrop;
        public bool? DisplayHold;
        public List<PlayerOptions> PlayerOptions;
    }

    
}
