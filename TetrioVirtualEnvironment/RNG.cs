using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class RNG
    {
        double t;
        public double Seed { get; private set; }
        public int CalledCount { get; private set; }

        static public int[] GetNextAt(double seed, int skipCount, int length)
        {
            int[] result = new int[length];
            var rng = new RNG();
            rng.Init(seed);

            var list = new List<int>();
            for (int i = 0; ; i++)
            {
                if (list.Count == 0)
                {
                    var array = (int[])Environment.MINOTYPES.Clone();
                    rng.ShuffleArray(array);
                    list.AddRange(array);
                }
                
                if (i >= skipCount)
                {
                    result[i - skipCount] = list[0];
                    if (i - skipCount == length - 1)
                        return result;
                }

                list.RemoveAt(0);

                
            }
        }

        public void Init(double seed)
        {
            Seed = seed;
            CalledCount = 0;
            t = seed % 2147483647;
            if (t <= 0)
            {
                t += 2147483646;
            }

        }

        public void ShuffleArray(int[] array)
        {
            for (var i = array.Length - 1; i != 0; i--)
            {
                var r = (int)(NextFloat() * (i + 1));
                var temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }
        }

        double Next()
        {
            t = 16807 * t % 2147483647;
            return t;
        }

        double NextFloat()
        {
            CalledCount++;
            return (Next() - 1) / 2147483646f;
        }

    }
}
