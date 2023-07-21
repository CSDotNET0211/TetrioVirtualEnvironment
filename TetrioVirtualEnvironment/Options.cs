using TetrReplayLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class Options
	{
		public enum GarbageBlockingType
		{
			ComboBlocking,
			LimitedBlocking,
			None
		}

		public enum PassthroughKind
		{
			Full,
			Limited,
			Zero,
			Consistent
		}

		public Options(EventFullOptionsData options)
		{
			GarbageCapMax = options.garbagecapmax ?? 40;
			Version = options.version;
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

			switch (options.garbageblocking)
			{
				case "combo blocking":
					GarbageBlocking = GarbageBlockingType.ComboBlocking;
					break;
				case "limited blocking":
					GarbageBlocking = GarbageBlockingType.LimitedBlocking;
					break;
				case "none":
					GarbageBlocking = GarbageBlockingType.None;
					break;
				default:
					GarbageBlocking = GarbageBlockingType.ComboBlocking;
					break;
			}

			if (options.passthrough == null)
				Passthrough = PassthroughKind.Limited;
			else
			{
				var passthroughStr = options.passthrough.ToString();
				if (passthroughStr == "true")
					Passthrough = PassthroughKind.Full;
				else if (passthroughStr == "false")
					Passthrough = PassthroughKind.Limited;
				else
				{
					switch (passthroughStr)
					{
						case "zero":
							Passthrough = PassthroughKind.Zero;
							break;
						case "consistent":
							Passthrough = PassthroughKind.Consistent;
							break;
						case "limited":
							Passthrough = PassthroughKind.Limited;
							break;
						case "full":
							Passthrough = PassthroughKind.Full;
							break;
						default:
							Passthrough = PassthroughKind.Limited;
							break;
					}
				}
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
		public PassthroughKind Passthrough { get; internal set; }
		public GarbageBlockingType GarbageBlocking { get; internal set; }
		public bool NoLockout { get; private set; }
	}
}