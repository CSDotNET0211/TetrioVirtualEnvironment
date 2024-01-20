using Microsoft.Extensions.DependencyInjection;
using TetrEnvironment.Constants;
using TetrEnvironment.Info;
using TetrEnvironment.Struct;
using TetrLoader.Enum;
using TetrLoader.Ige;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment;

public class GameData
{
	public GameData()
	{
	}

	public void Initialize(ServiceProvider provider,GameType? gametype)
	{
		Options = provider.GetService<Options>();
		Handling = provider.GetService<Handling>();
		SubFrame = 0;
		WaitingFrames = new List<WaitingFrameData>();
		Gravity = Options.Gravity;
		provider.GetService<BoardInfo>().SetupBoard(out _board);
		Bag = new Queue<Tetromino.MinoType>();
		Hold = Tetromino.MinoType.Empty;
		HoldLocked = false;

		//next rng
		var eventFullData = provider.GetService<EventFullOptionsData>();
		Rng = new Rng();
		Rng.Init(eventFullData.seed);
		provider.GetService<Environment>().BagInfo.PopulateBag();
		if (eventFullData.no_szo == true && Bag.Count != 0)
		{
			for (int i = 0;
			     i < Bag.Count && (Bag.Peek() is Tetromino.MinoType.S or Tetromino.MinoType.Z or Tetromino.MinoType.O);
			     i++)
				Bag.Enqueue(Bag.Dequeue());
		}

		LastGenerated = null;
		Falling = provider.GetService<Falling>();
		FallingRotations = 0;
		TotalRotations = 0;
		LDas = 0;
		LDasIter = 0;
		LShift = false;
		RDas = 0;
		RDasIter = 0;
		RShift = false;
		LastShift = 0;
		SoftDrop = false;
		Stats = provider.GetService<Stats>();
		Enemies = new List<string>();
		Targets = new List<string>();
		InteractionId = 0;
		ImpendingDamage = new List<GarbageData>();
		GarbageId = 0;
		GarbageBonus = 0;
		GarbageAckNowledgements =
			(new Dictionary<string, int?>(), new Dictionary<string, List<GarbageData>>());
		LastReceivedCount = 0;
		GarbageAreLockedUntil = 0;

		if (gametype == GameType.Blitz)
		{
			//TODO: startinglevel
			provider.GetService<Environment>().LineInfo.LevelLines(0,true,1);
		
		}
	}

	public Stats Stats { get; internal set; }
	public double SubFrame { get; internal set; }
	public bool LShift { get; internal set; }
	public bool RShift { get; internal set; }
	public int LastShift { get; internal set; }
	public double LDas { get; internal set; }
	public double RDas { get; internal set; }
	public Handling Handling { get; private set; }
	public Falling Falling { get; private set; }
	public double LDasIter { get; internal set; }
	public double RDasIter { get; internal set; }
	public int FallingRotations { get; internal set; }
	public int TotalRotations { get; internal set; }
	public bool SoftDrop { get; internal set; }
	public bool HoldLocked { get; internal set; }
	public double Gravity { get; internal set; }
	public Options Options { get; internal set; }
	private Tetromino.MinoType[] _board;

	public Tetromino.MinoType[] Board
	{
		get => _board;
		set => _board = value;
	}

	//not used
	public Offensive lastoffensive { get; internal set; }
	public int CurrentBTBChainPower { get; internal set; }
	public List<string> Enemies { get; internal set; } = new List<string>();
	public List<string> Targets { get; internal set; } = new List<string>();
	public int GarbageBonus { get; internal set; }
	public List<GarbageData> ImpendingDamage { get; internal set; }
	public int InteractionId { get; internal set; }

	public (Dictionary<String, int?> Incoming, Dictionary<string, List<GarbageData>> Outgoing) GarbageAckNowledgements
	{
		get;
		internal set;
	}

	public List<WaitingFrameData> WaitingFrames { get; internal set; }
	public Rng Rng { get; internal set; }
	public int? LastGenerated { get; internal set; }
	public Queue<Tetromino.MinoType> Bag { get; internal set; }
	public Tetromino.MinoType Hold { get; internal set; }
	public int GarbageId { get; internal set; }
	public int LastReceivedCount { get; internal set; }
	public int GarbageAreLockedUntil { get; internal set; }
}