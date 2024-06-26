﻿using TetrEnvironment.Constants;
using TetrLoader.Enum;

namespace TetrEnvironment.Info;

public class BagInfo
{
	private readonly int[] BAG7_ARRAY = new int[] { 3, 2, 1, 1 };
	private Environment _manager;
	public int PulledCount { get; private set; }

	public BagInfo(Environment manager)
	{
		_manager = manager;
		PulledCount = 0;
	}

	public Tetromino.MinoType PullFromBag()
	{
		for (; _manager.GameData.Bag.Count < 14;)
			PopulateBag();
		PulledCount++;

		return _manager.GameData.Bag.Dequeue();
	}

	public void PopulateBag()
	{
		List<Tetromino.MinoType> list = new List<Tetromino.MinoType>();

		switch (_manager.GameData.Options.BagType)
		{
			case BagType.TotalMayhem:
				for (int i = 0; i < 7; i++)
				{
					list.Add(Tetromino.MINOTYPES[
						(int)_manager.GameData.Rng.NextFloat() * Tetromino.MINOTYPES.Length]);
				}

				break;

			case BagType.Classic:
				for (int i = 0; i < 7; i++)
				{
					var newIndex = (int)(_manager.GameData.Rng.NextFloat() * (Tetromino.MINOTYPES.Length + 1));
					if (newIndex == _manager.GameData.LastGenerated || newIndex >= Tetromino.MINOTYPES.Length)
						newIndex = (int)_manager.GameData.Rng.NextFloat() * Tetromino.MINOTYPES.Length;

					_manager.GameData.LastGenerated = newIndex;
					list.Add(Tetromino.MINOTYPES[newIndex]);
				}

				break;

			case BagType.Pairs:
				var minos = (Tetromino.MinoType[])Tetromino.MINOTYPES.Clone();
				_manager.GameData.Rng.ShuffleArray(minos);
				var array = new Tetromino.MinoType[6];
				array[0] = minos[0];
				array[1] = minos[0];
				array[2] = minos[0];
				array[3] = minos[1];
				array[4] = minos[1];
				array[5] = minos[1];
				_manager.GameData.Rng.ShuffleArray(array);
				list = array.ToList();
				break;

			case BagType.Bag14:
				var newList = new List<Tetromino.MinoType>();
				newList.AddRange((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone());
				newList.AddRange((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone());
				var newArray = newList.ToArray();
				_manager.GameData.Rng.ShuffleArray(newArray);

				list = newArray.ToList();
				break;

			case BagType.Bag7OO:
				throw new NotImplementedException();
				minos = (Tetromino.MinoType[])Tetromino.MINOTYPES.Clone();
				break;

			case BagType.Bag7Plus1:
				newList = new List<Tetromino.MinoType>();
				newList.AddRange((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone());
				newList.Add(Tetromino.MINOTYPES[(int)(_manager.GameData.Rng.NextFloat() * Tetromino.MINOTYPES.Length)]);
				_manager.GameData.Rng.ShuffleArray(newList);
				list = newList;
				break;

			case BagType.Bag7Plus2:
				newList = new List<Tetromino.MinoType>();
				newList.AddRange((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone());
				newList.Add(Tetromino.MINOTYPES[(int)(_manager.GameData.Rng.NextFloat() * Tetromino.MINOTYPES.Length)]);
				newList.Add(Tetromino.MINOTYPES[(int)(_manager.GameData.Rng.NextFloat() * Tetromino.MINOTYPES.Length)]);
				_manager.GameData.Rng.ShuffleArray(newList);
				list = newList;
				break;

			case BagType.Bag7PlusX:
				//TODO: performance
				newList = new List<Tetromino.MinoType>();
				int n;
				n = BAG7_ARRAY.Length > _manager.GameData.BagId ? 
					BAG7_ARRAY[_manager.GameData.BagId] : 0;
				if (_manager.GameData.BagEx.Count < n)
				{
					_manager.GameData.BagEx.Clear();
					_manager.GameData.BagEx.AddRange((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone());
					_manager.GameData.Rng.ShuffleArray(_manager.GameData.BagEx);
				}

				newList.AddRange((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone());
				newList.AddRange(_manager.GameData.BagEx.Take(n));
				_manager.GameData.BagEx.RemoveRange(0, n);
				_manager.GameData.Rng.ShuffleArray(newList);

				list = newList;
				break;

			default:
				list = ((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone()).ToList();
				_manager.GameData.Rng.ShuffleArray(list);
				break;
		}

		foreach (var mino in list)
			_manager.GameData.Bag.Enqueue(mino);
		_manager.GameData.BagId++;
	}
}