using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TetrEnvironment.Constants.Kickset;
using TetrEnvironment.Info;
using TetrLoader.Enum;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment;

public class Environment
{
	#region Enum

	public enum GameModeEnum
	{
		SelfControl,
		Normal,
		EventBased
	}

	#endregion

	#region Infos

	public GarbageInfo GarbageInfo { get; private set; }
	public HandleInfo HandleInfo { get; private set; }
	public FallInfo FallInfo { get; private set; }
	public WaitingFrameInfo WaitingFrameInfo { get; private set; }
	public BoardInfo BoardInfo { get; private set; }
	public BagInfo BagInfo { get; private set; }
	public FrameInfo FrameInfo { get; private set; }
	public TargetInfo TargetInfo { get; private set; }
	public LineInfo LineInfo { get; private set; }

	#endregion

	#region Fields

	private readonly IReadOnlyList<Event> _events;
	private readonly GameType? _gameType;
	private readonly Environment _manager;
	private ServiceProvider _provider;
	private readonly CustomInjection _customInjection;

	/// <summary>
	/// Game settings
	/// </summary>
	private readonly EventFullOptionsData _eventFullOptions;

	/// <summary>
	/// Game start value
	/// </summary>
	private readonly EventFullGameData _eventFullGame;

	#endregion

	#region Property

	public int CurrentFrame
	{
		get => _manager.FrameInfo.CurrentFrame;
		set => _manager.FrameInfo.CurrentFrame = value;
	}

	public string? Username { get; private set; }
	public bool[] PressingKeys { get; private set; }
	public int TotalFrame { get; private set; }
	public readonly GameModeEnum GameMode;
	public GameData GameData { get; private set; }
	public CustomStats CustomStats { get; private set; }
	public KicksetBase Kickset { get; private set; }
	public bool IsDead { get; internal set; }

	#endregion


	/// <summary>
	/// normally this is managed by Replay class
	/// </summary>
	/// <param name="events">events by IReplayData.GetReplayEvents</param>
	/// <param name="gametype">for blitz, others will be ignored</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <exception cref="Exception"></exception>
	public Environment(IReadOnlyList<Event>? events, GameType? gametype, EventFullGameData? initializeData = null)
	{
		GameMode = GameModeEnum.Normal;
		_manager = this;
		_events = events;

		if (events != null &&
		    events.FirstOrDefault(@event => @event.type == EventType.End) is EventEnd eventEnd)
			TotalFrame = (int)eventEnd.frame;
		else //TODO: EventExit instead of EventEnd?
			throw new Exception("no end event detected!");

		try
		{
			_eventFullOptions = (events.First(@event => @event.type == EventType.Full) as EventFull).data.options;
		}
		catch (Exception e)
		{
			throw new Exception(
				"some of games in this replay has no Full event! it will be ignored in TETR.IO. please consider to remove spetific game.");
		}

		initializeData ??= new EventFullGameData();
		_eventFullGame = initializeData;
		_gameType = gametype;
		_customInjection = new CustomInjection();
		Reset();

		Username = _eventFullOptions.username;
	}

	/// <summary>
	/// for self control 
	/// </summary>
	/// <param name="fullOptionsData"></param>
	public Environment(EventFullOptionsData fullOptionsData,
		EventFullGameData? fullGameData = null,
		CustomInjection? customInjection = null)
	{
		GameMode = GameModeEnum.SelfControl;
		TotalFrame = -1;
		_manager = this;
		_eventFullOptions = fullOptionsData;
		fullGameData ??= new EventFullGameData();
		_eventFullGame = fullGameData;
		customInjection ??= new CustomInjection();
		_customInjection = customInjection;
		Reset();

		Username = _eventFullOptions.username;
		GameData.Falling.DeepSleep = false;
		FallInfo.Next(null);
	}

	public Environment(EventFullOptions options)
	{
		throw new Exception();
		GameMode = GameModeEnum.EventBased;
		_manager = this;
	}

	private void SolveFieldWithDI(ServiceProvider provider)
	{
		GarbageInfo = provider.GetService<GarbageInfo>() ?? throw new InvalidOperationException();
		HandleInfo = provider.GetService<HandleInfo>() ?? throw new InvalidOperationException();
		FallInfo = provider.GetService<FallInfo>() ?? throw new InvalidOperationException();
		WaitingFrameInfo = provider.GetService<WaitingFrameInfo>() ?? throw new InvalidOperationException();
		BoardInfo = provider.GetService<BoardInfo>() ?? throw new InvalidOperationException();
		FrameInfo = provider.GetService<FrameInfo>() ?? throw new InvalidOperationException();
		TargetInfo = provider.GetService<TargetInfo>() ?? throw new InvalidOperationException();
		BagInfo = provider.GetService<BagInfo>() ?? throw new InvalidOperationException();
		CustomStats = provider.GetService<CustomStats>() ?? throw new InvalidOperationException();
		LineInfo = provider.GetService<LineInfo>() ?? throw new InvalidOperationException();
	}

	private void InitDI(EventFullOptionsData fullDataOptions,
		EventFullGameData fullDataGame,
		CustomInjection customInjection,
		ref ServiceProvider provider)
	{
		ServiceCollection service = new ServiceCollection();
		service.AddSingleton<BagInfo>();
		service.AddSingleton<FallInfo>();
		service.AddSingleton<GarbageInfo>();
		service.AddSingleton<BoardInfo>();
		service.AddSingleton<Falling>();
		service.AddSingleton<FrameInfo>();
		service.AddSingleton<GarbageInfo>();
		service.AddSingleton<HandleInfo>();
		service.AddSingleton<LineInfo>();
		service.AddSingleton<TargetInfo>();
		service.AddSingleton<WaitingFrameInfo>();
		service.AddSingleton<CustomStats>();
		service.AddSingleton<Options>();
		service.AddSingleton<CustomInjection>(customInjection);
		service.AddSingleton<Stats>();
		service.AddSingleton<LineInfo>();
		service.AddSingleton<EventFullOptionsData>(fullDataOptions);
		service.AddSingleton<EventFullGameData>(fullDataGame);
		service.AddSingleton<Environment>(this);
		service.AddSingleton<Handling>(new Handling()
		{
			ARR = fullDataOptions.handling.arr ?? 5,
			DAS = fullDataOptions.handling.das ?? 12,
			SDF = fullDataOptions.handling.sdf ?? 20,
			DCD = fullDataOptions.handling.dcd ?? 20,
			Cancel = fullDataOptions.handling.cancel ?? false,
			SafeLock = fullDataOptions.handling.safelock == true ? 1 : 0,
			May20G = fullDataOptions.handling.may20g ?? true
		});

#if DEBUG
		if (fullDataOptions.handling?.arr == null)
			Debug.WriteLine("options.handling is initialized by default. it is wrong in most case.");
#endif

		provider = service.BuildServiceProvider();
	}

	/// <summary>
	/// for normal
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public bool NextFrame()
	{
		if (GameMode != GameModeEnum.Normal)
			throw new Exception("This NextFrame function is for normal.");


		if (_manager.FrameInfo.CurrentFrame > TotalFrame)
			return false;

		GameData.SubFrame = 0;
		_manager.FrameInfo.PullEvents(_events);
		CurrentFrame++;

		InternalNextFrame();

		return true;
	}

	/// <summary>
	/// for self control or event-based
	/// </summary>
	/// <param name="events"></param>
	public void NextFrame(Queue<Event>? events)
	{
		if (GameMode != GameModeEnum.SelfControl)
			throw new Exception("This NextFrame function is for self control　or event-based.");

		GameData.SubFrame = 0;

		while (events != null && events.Count != 0)
		{
			var @event = events.Dequeue();
			_manager.FrameInfo.PullEvent(@event);
		}

		CurrentFrame++;
		InternalNextFrame();
	}


	private void InternalNextFrame()
	{
		_manager.HandleInfo.ProcessShift(false, 1 - GameData.SubFrame);
		_manager.FallInfo.FallEvent(null, 1 - GameData.SubFrame);
		_manager.WaitingFrameInfo.ExcuteWaitingFrames();

		if (_manager.GameData.Options.GravityIncrease > 0 &&
		    CurrentFrame > _manager.GameData.Options.GravityMargin)
			_manager.GameData.Gravity += _manager.GameData.Options.GravityIncrease / 60;

		if (_manager.GameData.Options.GarbageIncrease > 0 &&
		    CurrentFrame > _manager.GameData.Options.GarbageMargin)
			_manager.GameData.Options.GarbageMultiplier += _manager.GameData.Options.GarbageIncrease / 60;

		if (_manager.GameData.Options.GarbageCapIncrease > 0)
			_manager.GameData.Options.GarbageCap += _manager.GameData.Options.GarbageCapIncrease / 60;
	}

	public void Initialize()
	{
	}

	public void Reset()
	{
		IsDead = false;
		Kickset = new KicksetSRSPlus();
		InitDI(_eventFullOptions, _eventFullGame, _customInjection, ref _provider);
		SolveFieldWithDI(_provider);
		GameData = new GameData();
		GameData.Initialize(_provider, _gameType);
		PressingKeys = new bool[8];
	}
}