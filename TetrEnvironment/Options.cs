using TetrLoader.Enum;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment;

public class Options
{
	public int Version { get; internal set; }
	public bool Levels { get; internal set; }
	public double GBase { get; internal set; }
	public double GSpeed { get; internal set; }
	public double? LevelSpeed { get; internal set; }
	public bool MasterLevels { get; internal set; }
	public bool LevelStatic { get; internal set; }
	public double? LevelStaticSpeed { get; internal set; }
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
	public int BoardHeight { get; internal set; }
	public bool ClipListenIDs { get; internal set; }

	public Options(EventFullOptionsData fullOptionsData)
	{
		Levels = fullOptionsData.levels ?? false;
		GBase = fullOptionsData.gbase ?? 0.65;
		GSpeed = fullOptionsData.gspeed ?? 0.007;
		LevelSpeed = fullOptionsData.levelspeed ?? 0.42;
		MasterLevels = fullOptionsData.masterlevels ?? false;
		BoardHeight = fullOptionsData.boardheight ?? 20;
		Version = fullOptionsData.version;
		GravityIncrease = fullOptionsData.gincrease ?? 0;
		GravityMargin = fullOptionsData.gmargin ?? 0;
		Gravity = fullOptionsData.g ?? 0;
		GarbageIncrease = fullOptionsData.garbageincrease ?? 0;
		GarbageMargin = fullOptionsData.garbagemargin ?? 0;
		GarbageMultiplier = fullOptionsData.garbagemultiplier ?? 1;
		GarbageCapIncrease = fullOptionsData.garbagecapincrease ?? 0;
		GarbageCap = fullOptionsData.garbagecap ?? 8;
		InfiniteMovement = fullOptionsData.infinitemovement ?? false;
		InfiniteHold = fullOptionsData.infinitehold ?? false;
		LockResets = fullOptionsData.lockresets ?? 15;
		Allow180 = fullOptionsData.allow180 ?? true;
		AllowHardDrop = fullOptionsData.allow_harddrop ?? true;
		DisplayHold = fullOptionsData.display_hold ?? true;
		LockTime = fullOptionsData.locktime ?? 30;
		SpinBonuses = fullOptionsData.spinbonuses ?? SpinBonusesType.TSpins;
		ComboTable = fullOptionsData.combotable ?? ComboTableType.Multiplier;
		BTBChaining = fullOptionsData.btbchaining ?? true;
		GarbageTargetBonus = fullOptionsData.garbagetargetbonus ?? GarbageTargetBonusType.None;
		GarbageAttackCap = fullOptionsData.garbageattackcap ?? false;
		GarbageBlocking = fullOptionsData.garbageblocking ?? GarbageBlockingType.ComboBlocking;
		Passthrough = fullOptionsData.passthrough ?? PassthroughType.Limited;
		GarbageSpeed = fullOptionsData.garbagespeed ?? 20;
		BagType = fullOptionsData.bagtype ?? BagType.Bag7;
		GarbagePhase = (fullOptionsData.garbagephase as int?) ?? 0;
		GarbageQueue = fullOptionsData.garbagequeue ?? false;
		GarbageHoleSize = fullOptionsData.garbageholesize ?? 1;
		AllClears = fullOptionsData.allclears ?? true;
		LineClearAre = fullOptionsData.lineclear_are ?? 0;
		Are = fullOptionsData.are ?? 0;
		GarbageEntry = fullOptionsData.garbageentry ?? GarbageEntryType.Instant;
		GarbageAre = (int)(Math.Max(1, fullOptionsData.garbageare ?? 5));
		Shielded = fullOptionsData.shielded ?? false;
		HasGarbage = fullOptionsData.hasgarbage ?? true;
		GarbageCapMax = fullOptionsData.garbagecapmax ?? 40;
		ClipListenIDs = true;
	}
}