using System;
using System.Diagnostics;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass;
using TetrReplayLoader.JsonClass.Event;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment
{
	public class GameData
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="envMode"></param>
		/// <param name="eventFull"></param>
		/// <param name="classManager"></param>
		/// <param name="nextSkipCount"></param>
		/// <param name="dataForInitialize"></param>
		/// <exception cref="Exception"></exception>
		public void Init(NextGenerateKind envMode, EventFullData eventFull, ClassManager classManager,
			int nextSkipCount, DataForInitialize dataForInitialize)
		{

			Next = new Queue<MinoKind?>();
			Hold = null;
			ImpendingDamages = new List<IgeData?>();
			NextBag = new List<int>();
			WaitingFrames = new List<WaitingFrameData>();
			InteractionID = 0;
			Targets = new List<string>();
			NotYetReceivedAttack = 0;
			GarbageID = 0;
			Options = new Options(eventFull.options);
			SubFrame = 0;
			LShift = false;
			RShift = false;
			SoftDrop = false;
			RDas = 0;
			LDas = 0;
			LastShift = 0;
			LDasIter = 0;
			RDasIter = 0;
			Falling = new Falling(classManager.Environment, this);
			Gravity = (double)(eventFull.options.g);
			SpinBonuses = eventFull.options.spinbonuses;
			GarbageActnowledgements = (new Dictionary<string, int?>(), new Dictionary<string, List<GarbageData?>>());

			classManager.Environment.Rng.Init(eventFull.options.seed);

			InitWithInitializer(envMode, dataForInitialize, eventFull, nextSkipCount, classManager);

			InitHandling(eventFull);

			if (envMode == NextGenerateKind.Array)
				Falling.Init(null, false, classManager.Environment.NextGenerateMode);
		}

		private void InitHandling(EventFullData eventFull)
		{
			if (eventFull.game == null)
				Handling = new PlayerOptions((double)eventFull.options?.handling.arr,
					(double)eventFull.options?.handling.das, (double)eventFull.options?.handling.dcd,
					(double)eventFull.options?.handling.sdf,
					(bool)eventFull.options?.handling.safelock ? 1 : 0, (bool)eventFull.options?.handling.cancel);
			else
				Handling = new PlayerOptions((double)eventFull.game.handling.arr, (double)eventFull.game.handling.das,
					(double)eventFull.game.handling.dcd, (double)eventFull.game.handling.sdf,
					(bool)eventFull.game.handling.safelock ? 1 : 0, (bool)eventFull.game.handling.cancel);
		}

		private void InitWithInitializer(NextGenerateKind envMode, in DataForInitialize data, EventFullData initGameData, int nextSkipCount, ClassManager classManager)
		{

			void InitBoard(DataForInitialize initData)
			{
				if (initData.Board == null)
				{
					Board = new MinoKind?[FIELD_SIZE];
					Array.Fill(Board, null);
				}
				else
				{
					Board = (MinoKind?[])initData.Board.Clone();
				}

			}

			void InitNext(DataForInitialize initData, NextGenerateKind mode)
			{
				Next = new Queue<MinoKind?>();

				//			if (envMode == NextGenerateKind.Array && initData.Current != null)
				//			Next.Enqueue(initData.Current);

				if (mode == NextGenerateKind.Array)
				{
					foreach (var next in initData.Next)
						Next.Enqueue(next);
				}
				else if (mode == NextGenerateKind.Seed)
				{
					if (initGameData.options?.nextcount == null)
						throw new Exception("nextCount is undefined");

					if (initGameData.options?.nextcount < 1)
						Console.WriteLine("nextCount is less than 1. This replay may not play properly.");

					for (int nextInitIndex = 0; nextInitIndex < (int)initGameData.options?.nextcount; nextInitIndex++)
						Next.Enqueue(null);

					classManager.Environment.RefreshNext(Next, initGameData.options.no_szo ?? false);

					//初回生成はszoフラグを考慮して上で先行して行っているため、一通り生成したものを循環させるには本来の数-1する
					for (int i = 0; i < Next.Count - 1; i++)
						classManager.Environment.RefreshNext(Next, false);

					//ネクストの乱数を指定した位置まで回してシフトさせる
					while (nextSkipCount > classManager.Environment.GeneratedRngCount)
						classManager.Environment.RefreshNext(Next, false);
				}
			}

			InitBoard(data);
			InitNext(data,envMode);


			if (data.Hold != null && data.Hold != null)
				Hold = data.Hold;

			if (data.Garbages != null)
				ImpendingDamages = data.Garbages;
		}


		public string? SpinBonuses { get; private set; }
		public MinoKind?[] Board { get; internal set; }
		public int CurrentBTBChainPower { get; internal set; }
		public Queue<MinoKind?> Next { get; private set; }
		public List<int> NextBag { get; private set; }
		public Options Options { get; private set; }
		public double SubFrame { get; internal set; }
		public bool LShift { get; internal set; }
		public bool RShift { get; internal set; }
		public int LastShift { get; internal set; }
		public double LDas { get; internal set; }
		public double RDas { get; internal set; }
		public PlayerOptions Handling { get; private set; }
		public double LDasIter { get; internal set; }
		public double RDasIter { get; internal set; }
		public bool SoftDrop { get; internal set; }
		public Falling Falling { get; private set; }
		public bool HoldLocked { get; internal set; }
		public MinoKind? Hold { get; internal set; }
		public double Gravity { get; internal set; }
		public int FallingRotations { get; internal set; }
		public int TotalRotations { get; internal set; }
		public List<IgeData?> ImpendingDamages { get; internal set; }
		public (Dictionary<string, int?> Incoming, Dictionary<string, List<GarbageData?>> Outgoing) GarbageActnowledgements
		{
			get;
			internal set;
		}
		public int GarbageID { get; internal set; }
		public int NotYetReceivedAttack { get; internal set; }
		public List<string> Targets { get; internal set; }
		public int InteractionID { get; internal set; }
		public List<WaitingFrameData> WaitingFrames { get; internal set; }
	}
}
