using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.Constants;
using TetrLoader.JsonClass;
using TetrLoader.JsonClass;

namespace TetrioVirtualEnvironment
{
	public static class Extension
	{
		public static int[] GetGarbagesAsArray(this List<IgeData?> env)
		{
			List<int> garbagesArray = new List<int>();

			foreach (var garbage in env)
			{
				var temp = 1;

				if (garbage.active)
					temp *= 1;
				else
					temp *= 0;

				temp += garbage.data.column * 10;

				temp += garbage.lines * 100;

				garbagesArray.Add(temp);
			}
			return garbagesArray.ToArray();
		}

		public static IgeData?[] Clone(this IgeData?[] env)
		{
			List<IgeData?> garbagesList = new();

			foreach (var garbage in env)
			{
				garbagesList.Add(garbage.Clone());
			}

			return garbagesList.ToArray();
		}

		public static List<IgeData?> Clone(this List<IgeData?> env)
		{
			List<IgeData?> garbagesList = new();

			foreach (var garbage in env)
			{
				garbagesList.Add(garbage.Clone());
			}

			return garbagesList;
		}

		public static void ToIntArray(this Tetrimino.MinoType[]? array, out int[]? resultArray)
		{
			if (array == null)
			{
				resultArray = null;
				return;
			}

			resultArray = new int[array.Length];

			for (int i = 0; i < array.Length; i++)
				resultArray[i] = (int)array[i];
		}

		public static int[] ToIntArray(this Vector2[] array)
		{
			int[] resultArray = new int[array.Length * 2];

			for (int i = 0; i < array.Length; i++)
			{
				resultArray[i * 2] = (int)array[i].x;
				resultArray[i * 2 + 1] = (int)array[i].y;
			}

			return resultArray;
		}

	}

}
