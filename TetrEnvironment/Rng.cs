using TetrEnvironment.Constants;

namespace TetrEnvironment;

public class Rng
{
	private double _t;
	public double Seed { get; private set; }

	public Rng()
	{
	}

	public void Init(double seed)
	{
		Seed = seed;
		_t = seed % 2147483647;
		if (_t <= 0)
			_t += 2147483646;
	}

	public double Next()
	{
		_t = 16807 * _t % 2147483647;
		return _t;
	}

	public double NextFloat()
	{
		return (Next() - 1) / 2147483646;
	}

	public void ShuffleArray(Tetromino.MinoType[] array)
	{
		int length = array.Length;
		if (length == 0)
			return;

		for (; --length > 0;)
		{
			var swapIndex = (int)(NextFloat() * (length + 1));
			(array[length], array[swapIndex]) = (array[swapIndex], array[length]);
		}
	}

	public void ShuffleArray(List<Tetromino.MinoType> list)
	{
		int length = list.Count;
		if (length == 0)
			return;

		for (; --length > 0;)
		{
			var swapIndex = (int)(NextFloat() * (length + 1));
			(list[length], list[swapIndex]) = (list[swapIndex], list[length]);
		}
	}
}