
using TetrioVirtualEnvironment.Constants;

namespace TetrioVirtualEnvironment
{
	public class Rng
	{
		private double _t;
		public double Seed { get; private set; }

		public static Tetrimino.MinoType[] GetNextAt(double seed, int skipCount, int length)
		{
			skipCount -= 5;

			Tetrimino.MinoType[] result = new Tetrimino.MinoType[length];
			var rng = new Rng();
			rng.Init(seed);

			var list = new List<Tetrimino.MinoType>();
			for (int i = 0;; i++)
			{
				if (list.Count == 0)
				{
					var array = (Tetrimino.MinoType[])Tetrimino.MINOTYPES.Clone();
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
			_t = seed % 2147483647;
			if (_t <= 0)
			{
				_t += 2147483646;
			}

		}

		public void ShuffleArray(Tetrimino.MinoType[] array)
		{
			for (var i = array.Length - 1; i != 0; i--)
			{
				var r = (int)(NextFloat() * (i + 1));
				(array[i], array[r]) = (array[r], array[i]);
			}
		}

		private double Next()
		{
			_t = 16807 * _t % 2147483647;
			return _t;
		}

		private double NextFloat()
		{
			return (Next() - 1) / 2147483646f;
		}

	}
}
