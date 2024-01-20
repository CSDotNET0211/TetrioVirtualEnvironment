﻿using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TetrEnvironment.Constants.Kickset;
using TetrEnvironment.Info;
using TetrLoader.Enum;
using TetrLoader.JsonClass.Event;

namespace TetrEnvironment;

public class Environment
{
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
	private readonly EventFullData _eventFull;

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
	public bool IsSelfControlMode { get; private set; }
	public GameData GameData { get; private set; }
	public CustomStats CustomStats { get; private set; }
	internal readonly KicksetBase Kickset;

	#endregion


	/// <summary>
	/// normally this is managed by Replay class
	/// </summary>
	/// <param name="events">events by IReplayData.GetReplayEvents</param>
	/// <param name="gametype">for blitz, others will be ignored</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <exception cref="Exception"></exception>
	public Environment(IReadOnlyList<Event>? events, GameType? gametype)
	{
		IsSelfControlMode = false;
		_manager = this;
		_events = events;
		//todo:implement it in Options
		Kickset = new KicksetSRSPlus();

		if (events != null &&
		    events.FirstOrDefault(@event => @event.type == EventType.End) is EventEnd eventEnd)
			TotalFrame = (int)eventEnd.frame;
		else //TODO: EventExit instead of EventEnd?
			throw new Exception("no end event detected!");

		try
		{
			_eventFull = (events.First(@event => @event.type == EventType.Full) as EventFull).data;
		}
		catch (Exception e)
		{
			throw new Exception(
				"some of games in this replay has no FULL event! it will be ignored in TETR.IO. please consider to remove spetific game.");
		}

		_gameType = gametype;
		Reset();

		Username = _eventFull.options.username;
	}

	/// <summary>
	/// for self control 
	/// </summary>
	/// <param name="fullData"></param>
	public Environment(EventFullData fullData)
	{
		IsSelfControlMode = true;
		TotalFrame = -1;
		_manager = this;
		_eventFull = fullData;
		Reset();
	}

	private void SolveWithDI(ServiceProvider provider)
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

	private void InitDI(EventFullData fullData, ref ServiceProvider provider)
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
		service.AddSingleton<Stats>();
		service.AddSingleton<LineInfo>();
		service.AddSingleton<EventFullData>(fullData);
		service.AddSingleton<Environment>(this);
		service.AddSingleton<Handling>(new Handling()
		{
			ARR = fullData.options.handling.arr ?? 5,
			DAS = fullData.options.handling.das ?? 12,
			SDF = fullData.options.handling.sdf ?? 20,
			DCD = fullData.options.handling.dcd ?? 20,
			Cancel = fullData.options.handling.cancel ?? false,
			SafeLock = fullData.options.handling.safelock == true ? 1 : 0,
		});

#if DEBUG
		if (fullData.options?.handling?.arr == null)
			Debug.WriteLine("options.handling is initialized by default. it is wrong in most case.");
#endif

		provider = service.BuildServiceProvider();
	}


	public bool NextFrame()
	{
		if (_manager.FrameInfo.CurrentFrame > TotalFrame)
			return false;

		GameData.SubFrame = 0;
		_manager.FrameInfo.PullEvents(_events);
		CurrentFrame++;

		_manager.HandleInfo.ProcessShift(false, 1 - GameData.SubFrame);
		_manager.FallInfo.FallEvent(null, 1 - GameData.SubFrame);
		_manager.WaitingFrameInfo.ExcuteWaitingFrames();

		//unsafe waiting

		if (_manager.GameData.Options.GravityIncrease > 0 &&
		    CurrentFrame > _manager.GameData.Options.GravityMargin)
			_manager.GameData.Gravity += _manager.GameData.Options.GravityIncrease / 60;

		if (_manager.GameData.Options.GarbageIncrease > 0 &&
		    CurrentFrame > _manager.GameData.Options.GarbageMargin)
			_manager.GameData.Options.GarbageMultiplier += _manager.GameData.Options.GarbageIncrease / 60;

		if (_manager.GameData.Options.GarbageCapIncrease > 0)
			_manager.GameData.Options.GarbageCap += _manager.GameData.Options.GarbageCapIncrease / 60;


		return true;
	}

	/// <summary>
	/// for self control
	/// </summary>
	/// <param name="event"></param>
	public void NextFrame(Queue<Event> events)
	{
		if (!IsSelfControlMode)
			throw new Exception("This NextFrame is for self control, please consider using normal NextFrame.");

		GameData.SubFrame = 0;

		while (events.Count != 0)
		{
			var @event = events.Dequeue();
			_manager.FrameInfo.PullEvent(@event);
		}

		CurrentFrame++;

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


	public void Reset()
	{
		InitDI(_eventFull, ref _provider);
		SolveWithDI(_provider);
		GameData = new GameData();
		GameData.Initialize(_provider, _gameType);
		PressingKeys = new bool[8];
	}
}