using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class Options
    {
        public Options(int version,int seed,int gravityMargin,float gravityIncrease,int garbageMultiplier,
            int garbageMargin,float garbageIncrease,bool infiniteMovement,int garbageCap,bool allow180,bool allowHardDrop,
            bool? displayHold,int? lockResets
            )
        {
            HasGarbage = false;
            GravityMargin=garbageMargin;
            GravityIncrease=gravityIncrease;
            GarbageMultiplier=garbageMultiplier;
            GarbageMargin=garbageMargin;
            GarbageIncrease=garbageIncrease;
            GarbageMultiplier=garbageMultiplier;
            InfiniteMovement=infiniteMovement;
            Allow180=allow180;
            AllowHardDrop=allowHardDrop;
            DisplayHold=displayHold;
            LockResets=
        }

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
        public int? LockResets;
        public int? LockTime;
    //    public List<PlayerOptions> PlayerOptions;
    }

    

    
}
