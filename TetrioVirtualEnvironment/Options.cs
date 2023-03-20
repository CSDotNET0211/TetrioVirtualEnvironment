using TetrReplayLoader.JsonClass.Event;

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
            //TODO: これも取得させる
            GarbageMultiplier = 1;
            GarbageMargin = options.garbagemargin == null ? 0 : (int)options.garbagemargin; 
            GarbageIncrease = options.garbageincrease == null ? 0 : (double)options.garbageincrease;
            Allow180 = (bool)options.allow180;
            AllowHardDrop = options.allow_harddrop == null ? true : (bool)options.allow_harddrop;
            DisplayHold = options.display_hold;
            LockResets = options.lockresets == null ? 15 : (int)options.lockresets;
            LockTime = options.locktime == null ? 30 : (int)options.locktime;
            GarbageCap = options.garbagecap == null ? 8 : (int)options.garbagecap;
            GarbageCapIncrease = options.garbagecapincrease == null ? 0 : (int)options.garbagecapincrease;
            GarbageSpeed = options.garbagespeed == null ? 0 : (int)options.garbagespeed;
            BTBChaining = options.b2bchaining == null ? true : (bool)options.b2bchaining;

        }

        public int Version;
        public bool BTBChaining;
        public int Seed;
        public bool HasGarbage;
        public int GravityMargin;
        public double GravityIncrease;
        public double GarbageMultiplier;
        public int GarbageMargin;
        public int GarbageSpeed;
        public double GarbageIncrease;
        public bool InfiniteMovement;
        public double GarbageCap;
        public double GarbageCapIncrease;
        public bool Allow180;
        public bool AllowHardDrop;
        public bool? DisplayHold;
        public int? LockResets;
        public int? LockTime;

    }




}
