using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoaderLib;

namespace TetrioVirtualEnvironment
{
    public class Options
    {
        public Options(EventFullOptions options)
        {
            Version = (int)options.version;
            Seed = (int)options.seed;
            HasGarbage = options.hasgarbage == null ? false : (bool)options.hasgarbage;
            GravityMargin = options.gmargin == null ? 0 : (int)options.gmargin;
            GravityIncrease = options.gincrease == null ? 0 : (double)options.gincrease;
            InfiniteMovement = options.infinitemovement == null ? false : (bool)options.infinitemovement;
            GarbageMultiplier = 1;
            GarbageMargin = 0;
            GarbageIncrease = 0;
            Allow180 = (bool)options.allow180;
            AllowHardDrop = options.allow_harddrop == null ? true : (bool)options.allow_harddrop;
            DisplayHold = options.display_hold;
            LockResets = options.lockresets == null ? 15 : (int)options.lockresets;
            LockTime = options.locktime == null ? 30 : (int)options.locktime;
            GarbageCap = options.garbagecap == null ? 8 : (int)options.garbagecap;
            GarbageSpeed = options.garbagespeed == null ? 0 : (int)options.garbagespeed;
        }

        public int Version;
        public int Seed;
        public bool HasGarbage;
        public int GravityMargin;
        public double GravityIncrease;
        public int GarbageMultiplier;
        public int GarbageMargin;
        public int GarbageSpeed;
        public double GarbageIncrease;
        public bool InfiniteMovement;
        public int GarbageCap;
        public bool Allow180;
        public bool AllowHardDrop;
        public bool? DisplayHold;
        public int? LockResets;
        public int? LockTime;

    }




}
