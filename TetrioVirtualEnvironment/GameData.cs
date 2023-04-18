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
        /// Contructor for 
        /// </summary>
        /// <param name="envMode"></param>
        /// <param name="eventFull"></param>
        /// <param name="environment"></param>
        /// <param name="gameData"></param>
        /// <param name="nextSkipCount"></param>
        /// <param name="dataForInitialize"></param>
        /// <exception cref="Exception"></exception>
        public GameData(out GameData gamedataForInit, NextGenerateKind envMode, EventFull eventFull, Environment environment, int nextSkipCount, DataForInitialize dataForInitialize)
        {
            gamedataForInit = this;

            if (dataForInitialize.Garbages != null)
            {
                foreach (var garbage in dataForInitialize.Garbages)
                {
                    var kind = garbage % 10;
                    var pos = garbage / 10 % 10;
                    var amount = garbage / 100;

                    environment.Garbages.Add(new Garbage(0, 60, 0, pos, amount, kind == 0 ? Garbage.GarbageKind.InteractionConfirm : Garbage.GarbageKind.Ready));
                }
            }



            if (dataForInitialize.Field == null)
            {
                Board = new int[FIELD_SIZE];
                Array.Fill(Board, (int)MinoKind.Empty);
            }
            else
                Board = (int[]?)dataForInitialize.Field.Clone();

            if (eventFull.options?.seed == null)
                throw new Exception("seed is null");

            environment.Rng.Init(eventFull.options.seed);

            if (envMode == NextGenerateKind.Seed)
            {


                NextBag = new List<int>();
                if (dataForInitialize.Next == null)
                {
                    if (eventFull.options?.nextcount == null)
                        throw new Exception("nextCount is null");

                    Next = new List<int>(new int[(int)eventFull.options?.nextcount]);

                    foreach (var value in NextBag)
                        Console.WriteLine(value.ToString());

                    environment.RefreshNext(Next, eventFull.options.no_szo ?? false);
                    for (int i = 0; i < Next.Count - 1; i++)
                        environment.RefreshNext(Next, false);

                }
                else
                {
                    Next.AddRange(dataForInitialize.Next);
                }

                while (nextSkipCount > environment.GeneratedRngCount)
                    environment.RefreshNext(Next, false);


            }
            else
            {
                Next = new List<int>();
                if (dataForInitialize.Current != (int)MinoKind.Empty)
                    Next.Add((int)dataForInitialize.Current);

                if (dataForInitialize.Next == null)
                {
                }
                else
                {
                    Next.AddRange(dataForInitialize.Next);

                }
            }

            Hold = dataForInitialize.Hold is (int)MinoKind.Empty ? null : dataForInitialize.Hold;

            WaitingFrames = new List<WaitingFrameData>();
            InteractionID = 0;
            Targets = new List<string>();
            NotYetReceivedAttack = 0;
            GarbageID = 0;
            ImpendingDamages = new();
            GarbageActnowledgements = (new Dictionary<string, int?>(), new Dictionary<string, List<GarbageData?>>());

            Options = new Options(eventFull.options);
            SubFrame = 0;
            LShift = false;
            RShift = false;
            SoftDrop = false;
            RDas = 0;
            LDas = 0;
            LastShift = 0;
            if (eventFull.game == null)
                Handling = new PlayerOptions((double)eventFull.options.handling.arr,
               (double)eventFull.options.handling.das, (double)eventFull.options.handling.dcd,
               (double)eventFull.options.handling.sdf,
               (bool)eventFull.options.handling.safelock ? 1 : 0, (bool)eventFull.options.handling.cancel);
            else
                Handling = new PlayerOptions((double)eventFull.game.handling.arr, (double)eventFull.game.handling.das,
                    (double)eventFull.game.handling.dcd, (double)eventFull.game.handling.sdf, (bool)eventFull.game.handling.safelock ? 1 : 0, (bool)eventFull.game.handling.cancel);

            LDasIter = 0;
            RDasIter = 0;
            Falling = new Falling(environment, this);

            Hold = dataForInitialize.Hold;

            Gravity = (double)(eventFull.options.g);
            SpinBonuses = eventFull.options.spinbonuses;

            if (envMode == NextGenerateKind.Array)
                Falling.Init(null, false, environment.NextGenerateMode);
        }


        public string? SpinBonuses { get; }
        public int[] Board { get; internal set; }
        public int CurrentBTBChainPower { get; internal set; }
        public List<int> Next { get; }
        public List<int> NextBag { get; }
        public Options Options { get; }
        public double SubFrame { get; internal set; }
        public bool LShift { get; internal set; }
        public bool RShift { get; internal set; }
        public int LastShift { get; internal set; }
        public double LDas { get; internal set; }
        public double RDas { get; internal set; }
        public PlayerOptions Handling { get; }
        public double LDasIter { get; internal set; }
        public double RDasIter { get; internal set; }
        public bool SoftDrop { get; internal set; }
        public Falling Falling { get; }
        public bool HoldLocked { get; internal set; }
        public int? Hold { get; internal set; }
        public double Gravity { get; internal set; }
        public int FallingRotations { get; internal set; }
        public int TotalRotations { get; internal set; }
        public List<IgeData> ImpendingDamages { get; internal set; }
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
