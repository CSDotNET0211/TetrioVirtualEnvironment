using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class RNG
    {
     static   long t;
     static   public void Init(int seed)
        {
            t = seed % 2147483647;
            if (t <= 0)
            {
                t += 2147483646;
            }

        }

    static    public void ShuffleArray(int[] array)
        {
            for (var i = array.Length - 1; i != 0; i--)
            {
                var r = (int)(NextFloat() * (i + 1));
                var temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }
        }

    static    long Next()
        {
            t = 16807 * t % 2147483647;
            return t;
        }

   static     float NextFloat()
        {
            return (Next() - 1) / 2147483646f;
        }

    }
}
