using System.Runtime;
using TetrReplayLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class Options
	{
		public Options(EventFullOptionsData options)
		{
			GarbageCapMax = options.garbagecapmax ?? 40;
			Version = (int)options.version;
			Seed = (int)options.seed;
			HasGarbage = options.hasgarbage ?? false;
			GravityMargin = options.gmargin ?? 0;
			GravityIncrease = options.gincrease ?? 0;
			InfiniteMovement = options.infinitemovement ?? false;
			GarbageMultiplier = options.garbagemultiplier ?? 1;
			GarbageMargin = options.garbagemargin ?? 0;
			GarbageIncrease = options.garbageincrease ?? 0;
			Allow180 = options.allow180 ?? false;
			AllowHardDrop = options.allow_harddrop ?? true;
			DisplayHold = options.display_hold;
			LockResets = options.lockresets ?? 15;
			LockTime = options.locktime ?? 30;
			GarbageCap = options.garbagecap ?? 8;
			GarbageCapIncrease = options.garbagecapincrease ?? 0;
			GarbageSpeed = options.garbagespeed ?? 0;
			BTBChaining = options.b2bchaining ?? true;
			NoLockout = options.nolockout ?? true;
			GarbageBlocking = options.garbageblocking ?? "combo blocking";
			if (options.passthrough == null)
				Passthrough = "limited";
			else
			{
				if (options.passthrough.ToString() == "true")
					Passthrough = "full";
				else if (options.passthrough.ToString() == "false")
					Passthrough = "limited";
				else
					Passthrough = (options.passthrough.ToString() ?? "limited");

			}


		}

		public int Version { get; internal set; }
		public bool BTBChaining { get; internal set; }
		public int Seed { get; internal set; }
		public bool HasGarbage { get; internal set; }
		public int GravityMargin { get; internal set; }
		public double GravityIncrease { get; internal set; }
		public double GarbageMultiplier { get; internal set; }
		public int GarbageMargin { get; internal set; }
		public int GarbageSpeed { get; internal set; }
		public double GarbageIncrease { get; internal set; }
		public bool InfiniteMovement { get; internal set; }
		public double GarbageCap { get; internal set; }
		public double GarbageCapMax { get; internal set; }
		public double GarbageCapIncrease { get; internal set; }
		public bool Allow180 { get; internal set; }
		public bool AllowHardDrop { get; internal set; }
		public bool? DisplayHold { get; internal set; }
		public int? LockResets { get; internal set; }
		public int? LockTime { get; internal set; }

		public bool ClipListenIDs { get; internal set; } = true;
		public string Passthrough { get; internal set; }
		public string GarbageBlocking { get; internal set; }
		public bool NoLockout { get; private set; }
	}




}
