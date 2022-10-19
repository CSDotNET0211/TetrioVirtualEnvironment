using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoaderLib;
using static System.Net.Mime.MediaTypeNames;
using static TetrioVirtualEnvironment.Environment;

namespace TetrioVirtualEnvironment
{
    public class GameData
    {
        public GameData(EventFullOptions optionsdata, bool is_ttrm,Environment environment,ref GameData gameData)
        {
            gameData =this;

            Field = new int[FIELD_WIDTH * FIELD_HEIGHT];
            for (int x = 0; x < FIELD_WIDTH; x++)
            {
                for (int y = 0; y < FIELD_HEIGHT; y++)
                {
                    Field[x + y * 10] = (int)MinoKind.Empty;
                }
            }

            environment.RNG.Init(optionsdata.seed);
            Next = new int[(int)optionsdata.nextcount];
            NextBag = new List<int>();



            environment.RefreshNext(Next,optionsdata.no_szo==null?false:(bool)optionsdata.no_szo);
            for (int i = 0; i < Next.Length - 1; i++)
                environment.RefreshNext(Next, false);

            Options = new Options(optionsdata);
            SubFrame = 0;
            LShift = false;
            RShift = false;
            SoftDrop = false;
            RDas = 0;
            LDas = 0;
            LastShift = 0;
            Handling = new PlayerOptions((double)optionsdata.handling.arr, (double)optionsdata.handling.das,
                (double)optionsdata.handling.dcd, (double)optionsdata.handling.sdf, (bool)optionsdata.handling.safelock ? 1 : 0, (bool)optionsdata.handling.cancel);
            LDasIter = 0;
            RDasIter = 0;
            Falling = new Falling(environment, this);
            Hold = null;

            Gravity = (double)optionsdata.g;
            SpinBonuses= optionsdata.spinbonuses;

        }

        public string SpinBonuses;
        public int[] Field;
        public int CurrentBTBChainPower;
        public int[] Next;
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
