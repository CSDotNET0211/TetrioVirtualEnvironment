using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    static public class Extension
    {
        static public int CountCanReceive(this List<Garbage> garbages)
        {
            int count = 0;
            foreach (var garbage in garbages)
            {
                if (garbage.state == Garbage.State.Ready)
                {
                    count++;
                }
            }

            return count;
        }
        static public (int NotConfirmed, int Confirmed,int Ready) GarbageCount(this List<Garbage> garbages)
        {
            int count = 0;
            int countConfirmed = 0;
            int countReady = 0;
            foreach (var garbage in garbages)
            {
                if (garbage.state == Garbage.State.Interaction)
                    count += garbage.power;
                else if(garbage.state == Garbage.State.Interaction_Confirm)
                    countConfirmed += garbage.power;
                else
                    countReady+= garbage.power;
            }

            return (count,countConfirmed,countReady);
        }

    }
}
