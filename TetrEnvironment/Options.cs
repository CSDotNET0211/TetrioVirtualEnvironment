using TetrLoader.Enum;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment;

public class Options
{
	public int Version { get; internal set; }
	public double GravityIncrease { get; internal set; }
	public int GravityMargin { get; internal set; }
	public double Gravity { get; internal set; }
	public double GarbageIncrease { get; internal set; }
	public int GarbageMargin { get; internal set; }
	public double GarbageMultiplier { get; internal set; }
	public double GarbageCapIncrease { get; internal set; }
	public double GarbageCap { get; internal set; }
	public bool InfiniteMovement { get; internal set; }
	public bool InfiniteHold { get; internal set; }
	public int? LockResets { get; internal set; }
	public bool Allow180 { get; internal set; }
	public bool AllowHardDrop { get; internal set; }
	public bool? DisplayHold { get; internal set; }
	public int? LockTime { get; internal set; }
	public SpinBonusesType SpinBonuses { get; internal set; }
	public ComboTableType ComboTable { get; internal set; }
	public bool BTBChaining { get; internal set; }
	public GarbageTargetBonusType GarbageTargetBonus { get; internal set; }
	public bool GarbageAttackCap { get; internal set; }
	public GarbageBlockingType GarbageBlocking { get; internal set; }
	public PassthroughType Passthrough { get; internal set; }
	public int GarbageSpeed { get; internal set; }
	public BagType BagType { get; internal set; }
	public int GarbagePhase { get; internal set; }
	public bool GarbageQueue { get; internal set; }
	public int GarbageHoleSize { get; internal set; }
	public bool AllClears { get; internal set; }
	public int LineClearAre { get; internal set; }
	public int Are { get; internal set; }
	public GarbageEntryType GarbageEntry { get; internal set; }
	public int GarbageAre { get; internal set; }
	public bool Shielded { get; internal set; }
	public bool HasGarbage { get; internal set; }
	public int GarbageCapMax { get; internal set; }

	public bool ClipListenIDs { get; internal set; }

	public Options(EventFullData fullData)
	{
		Version = fullData.options.version;
		GravityIncrease = fullData.options.gincrease ?? 0;
		GravityMargin = fullData.options.gmargin ?? 0;
		Gravity = fullData.options.g ?? 0;
		GarbageIncrease = fullData.options.garbageincrease ?? 0;
		GarbageMargin = fullData.options.garbagemargin ?? 0;
		GarbageMultiplier = fullData.options.garbagemultiplier ?? 1;
		GarbageCapIncrease = fullData.options.garbagecapincrease ?? 0;
		GarbageCap = fullData.options.garbagecap ?? 8;
		InfiniteMovement = fullData.options.infinitemovement ?? false;
		InfiniteHold = fullData.options.infinitehold ?? false;
		LockResets = fullData.options.lockresets ?? 15;
		Allow180 = fullData.options.allow180 ?? true;
		AllowHardDrop = fullData.options.allow_harddrop ?? true;
		DisplayHold = fullData.options.display_hold ?? true;
		LockTime = fullData.options.locktime ?? 30;
		SpinBonuses = fullData.options.spinbonuses ?? SpinBonusesType.TSpins;
		ComboTable = fullData.options.combotable ?? ComboTableType.Multiplier;
		BTBChaining = fullData.options.btbchaining ?? true;
		GarbageTargetBonus = fullData.options.garbagetargetbonus ?? GarbageTargetBonusType.None;
		GarbageAttackCap = fullData.options.garbageattackcap ?? false;
		GarbageBlocking = fullData.options.garbageblocking ?? GarbageBlockingType.ComboBlocking;
		Passthrough = fullData.options.passthrough ?? PassthroughType.Limited;
		GarbageSpeed = fullData.options.garbagespeed ?? 20;
		BagType = fullData.options.bagtype ?? BagType.Bag7;
		GarbagePhase = (fullData.options.garbagephase as int?) ?? 0;
		GarbageQueue = fullData.options.garbagequeue ?? false;
		GarbageHoleSize = fullData.options.garbageholesize ?? 1;
		AllClears = fullData.options.allclears ?? true;
		LineClearAre = fullData.options.lineclear_are ?? 0;
		Are = fullData.options.are ?? 0;
		GarbageEntry = fullData.options.garbageentry ?? GarbageEntryType.Instant;
		GarbageAre = (int)(Math.Max(1, fullData.options.garbageare ?? 5));
		Shielded = fullData.options.shielded ?? false;
		HasGarbage = fullData.options.hasgarbage ?? true;
		GarbageCapMax = fullData.options.garbagecapmax ?? 40;
		ClipListenIDs = true;
	 
	}
}