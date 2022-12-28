using TetrReplayLoaderLib;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment
{
    public class GameData
    {
        public GameData(EventFull eventfull, Environment environment, ref GameData gameData, int nextSkipCount, InitData initData)
        {
            gameData = this;

            if (initData.Field == null)
            {
                Field = new int[FIELD_WIDTH * FIELD_HEIGHT];
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    for (int y = 0; y < FIELD_HEIGHT; y++)
                    {
                        Field[x + y * 10] = (int)MinoKind.Empty;
                    }
                }
            }
            else
            {
                Field = initData.Field;
            }

            environment.RNG.Init(eventfull.options.seed);

            NextBag = new List<int>();
            if (initData.Next == null)
            {
                Next = new List<int>(new int[(int)eventfull.options.nextcount]);
                environment.RefreshNext(Next, eventfull.options.no_szo == null ? false : (bool)eventfull.options.no_szo);
                for (int i = 0; i < Next.Count - 1; i++)
                    environment.RefreshNext(Next, false);

            }
            else
            {
                Next.AddRange(initData.Next);
            }





            while (nextSkipCount > environment.RNG.CalledCount)
                environment.RefreshNext(Next, false);


            Options = new Options(eventfull.options);
            SubFrame = 0;
            LShift = false;
            RShift = false;
            SoftDrop = false;
            RDas = 0;
            LDas = 0;
            LastShift = 0;
            if(eventfull.game==null)
                Handling = new PlayerOptions((double)eventfull.options.handling.arr,
               (double)eventfull.options.handling.das, (double)eventfull.options.handling.dcd,
               (double)eventfull.options.handling.sdf,
               (bool)eventfull.options.handling.safelock ? 1 : 0, (bool)eventfull.options.handling.cancel);
            else
            Handling = new PlayerOptions((double)eventfull.game.handling.arr, (double)eventfull.game.handling.das,
                (double)eventfull.game.handling.dcd, (double)eventfull.game.handling.sdf, (bool)eventfull.game.handling.safelock ? 1 : 0, (bool)eventfull.game.handling.cancel);

            LDasIter = 0;
            RDasIter = 0;
            Falling = new Falling(environment, this);

            Hold = initData.Hold;

            Gravity = (double)(eventfull.options.g);
            SpinBonuses = eventfull.options.spinbonuses;

        }



        public GameData(EventFullOptions optionsdata, Environment environment, ref GameData gameData,
            InitData initData)
        {
            gameData = this;

            if (initData.Field == null)
            {
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    for (int y = 0; y < FIELD_HEIGHT; y++)
                    {
                        Field[x + y * 10] = (int)MinoKind.Empty;
                    }
                }
            }
            else
                Field = (int[])initData.Field.Clone();

            Next = new List<int>();
            if (initData != null && initData.Now != (int)MinoKind.Empty)
                Next.Add((int)initData.Now);

            if (initData.Next == null)
            {
            }
            else
            {
                Next.AddRange(initData.Next);

            }



            Options = new Options(optionsdata);
            Options.InfiniteMovement = true;
            SubFrame = 0;
            LShift = false;
            RShift = false;
            SoftDrop = false;
            RDas = 0;
            LDas = 0;
            LastShift = 0;
            Handling = new PlayerOptions((double)optionsdata.handling.arr,
                (double)optionsdata.handling.das, (double)optionsdata.handling.dcd,
                (double)optionsdata.handling.sdf,
                (bool)optionsdata.handling.safelock ? 1 : 0, (bool)optionsdata.handling.cancel);
            LDasIter = 0;
            RDasIter = 0;
            Falling = new Falling(environment, this);

            if (initData.Hold != null && initData.Hold == (int)MinoKind.Empty)
                Hold = null;
            else
                Hold = initData.Hold;

            Gravity = (double)optionsdata.g;
            SpinBonuses = optionsdata.spinbonuses;


            Falling.Init(null, environment.EnvironmentMode);
        }

        public string SpinBonuses;
        public int[] Field;
        public int CurrentBTBChainPower;
        public List<int> Next;
        public List<int> NextBag;
        public Options Options;
        public double SubFrame;
        public bool LShift;
        public bool RShift;
        public int LastShift;
        public double LDas;
        public double RDas;
        public PlayerOptions Handling;
        public double LDasIter;
        public double RDasIter;
        public bool SoftDrop;
        public Falling Falling;
        public bool HoldLocked;
        public int? Hold;
        public double Gravity;
        public int FallingRotations;
        public int TotalRotations;

    }
}
