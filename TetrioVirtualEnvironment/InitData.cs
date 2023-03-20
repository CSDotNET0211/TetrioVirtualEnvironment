using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class InitData
    {
        public InitData()
        {
            Field = null;
            Hold = null;
            Next = null;
            Now = null;
            Garbages = null;
        }

        public InitData(int[] field, int hold, int now, int[] next, int[]? garbages)
        {
            Field = field;
            Hold = hold;
            Now = now;
            Next = next;
            Garbages = garbages;
        }

        public int[]? Field;
        public int? Hold;
        public int? Now;
        public int[]? Next;
        public int[]? Garbages;

    }
}
