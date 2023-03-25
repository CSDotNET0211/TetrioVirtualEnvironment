using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
	public static class Extension
    {
        internal static int CountCanReceive(this List<Garbage> garbages)
        {
            int count = 0;
            foreach (var garbage in garbages)
            {
                if (garbage.State == Garbage.StateEnum.Ready)
                {
                    count++;
                }
            }

            return count;
        }
        public static (int NotConfirmed, int Confirmed,int Ready) GarbageCount(this List<Garbage> garbages)
        {
            int count = 0;
            int countConfirmed = 0;
            int countReady = 0;
            foreach (var garbage in garbages)
            {
                if (garbage.State == Garbage.StateEnum.Interaction)
                    count += garbage.Power;
                else if(garbage.State == Garbage.StateEnum.InteractionConfirm)
                    countConfirmed += garbage.Power;
                else
                    countReady+= garbage.Power;
            }

            return (count,countConfirmed,countReady);
        }

    }
}
