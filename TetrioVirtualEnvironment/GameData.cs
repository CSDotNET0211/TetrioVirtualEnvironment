using TetrioVirtualEnvironment.Constants;
using TetrLoader.Enum;
using static TetrioVirtualEnvironment.Environment;
using TetrLoader.JsonClass;
using TetrLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class GameData
	{
		public GameData(Environment environment)
		{
			_environment = environment;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="envMode"></param>
		/// <param name="eventFull"></param>
		/// <param name="classManager"></param>
		/// <param name="nextSkipCount"></param>
		/// <param name="initData"></param>
		/// <exception cref="Exception"></exception>
		public void Init(NextGenerateKind envMode, EventFullData eventFull,
			int nextSkipCount, FieldData initData)
		{

			Next = new Queue<Tetrimino.MinoType>();
			Hold = Tetrimino.MinoType.Empty;
			ImpendingDamages = new List<IgeData?>();
			NextBag = new List<Tetrimino.MinoType>();
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
			Falling = new Falling(_environment);
			Gravity =(eventFull.options.g??999);
			SpinBonuses = eventFull.options.spinbonuses ?? SpinBonusesType.TSpins;


			GarbageActnowledgements = (new Dictionary<string, int?>(), new Dictionary<string, List<GarbageData?>>());

			InitializeField(envMode, initData, eventFull, nextSkipCount, _environment);
			InitializeHandling(eventFull);


			if (envMode == NextGenerateKind.Array)
				Falling.Init(null, false, _environment.NextGenerateMode);
		}

		private void InitializeHandling(EventFullData eventFull)
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

		private void InitializeField(NextGenerateKind envMode, in FieldData data, EventFullData initGameData,
			int nextSkipCount, Environment environment)
		{
			void InitBoard(FieldData initData)
			{
				if (initData.Board == null)
				{
					Board = new Tetrimino.MinoType[FIELD_WIDTH * FIELD_HEIGHT];
					Array.Fill(Board, Tetrimino.MinoType.Empty);
				}
				else
				{
					Board = (Tetrimino.MinoType[])initData.Board.Clone();
				}
			}

			void InitNext(FieldData initData, NextGenerateKind mode)
			{
				Next = new Queue<Tetrimino.MinoType>();

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
						Next.Enqueue(Tetrimino.MinoType.Empty);

					environment.RefreshNext(Next, initGameData.options.no_szo ?? false);

					//初回生成はszoフラグを考慮して上で先行して行っているため、一通り生成したものを循環させるには本来の数-1する
					for (int i = 0; i < Next.Count - 1; i++)
						environment.RefreshNext(Next, false);

					//ネクストの乱数を指定した位置まで回してシフトさせる
					while (nextSkipCount > environment.GeneratedRngCount)
						environment.RefreshNext(Next, false);
				}
			}

			InitBoard(data);
			InitNext(data, envMode);


			//	if (data.Hold != null)
			Hold = data.Hold;

			if (data.Garbages != null)
				ImpendingDamages = data.Garbages;
		}

		private readonly Environment _environment;
		public SpinBonusesType SpinBonuses { get; private set; }
		public Tetrimino.MinoType[] Board { get; internal set; }
		public int CurrentBTBChainPower { get; internal set; }
		public Queue<Tetrimino.MinoType> Next { get; private set; }
		public List<Tetrimino.MinoType> NextBag { get; private set; }
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
		public Tetrimino.MinoType Hold { get; internal set; }
		public double Gravity { get; internal set; }
		public int FallingRotations { get; internal set; }
		public int TotalRotations { get; internal set; }
		public List<IgeData?> ImpendingDamages { get; internal set; }

		public (Dictionary<string, int?> Incoming, Dictionary<string, List<GarbageData?>> Outgoing)
			GarbageActnowledgements { get; internal set; }

		public int GarbageID { get; internal set; }
		public int NotYetReceivedAttack { get; internal set; }
		public List<string> Targets { get; internal set; }
		public int InteractionID { get; internal set; }
		public List<WaitingFrameData> WaitingFrames { get; internal set; }
	}
}