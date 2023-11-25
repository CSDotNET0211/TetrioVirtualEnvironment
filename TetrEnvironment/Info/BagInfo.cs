using System.Dynamic;
using TetrEnvironment.Constants;
using TetrLoader.Enum;

namespace TetrEnvironment.Info;

public class BagInfo
{
	private Environment _manager;

	public BagInfo(Environment manager)
	{
		_manager = manager;
	}

	public Tetromino.MinoType PullFromBag()
	{
		for (; _manager.GameData.Bag.Count < 14;)
		{
			PopulateBag();
		}

		var value = _manager.GameData.Bag[0];
		_manager.GameData.Bag.RemoveAt(0);
		return value;
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
				var a = (Tetromino.MinoType[])Tetromino.MINOTYPES.Clone();
				_manager.GameData.Rng.ShuffleArray(a);
				var array = new Tetromino.MinoType[6];
				array[0] = a[0];
				array[1] = a[0];
				array[2] = a[0];
				array[3] = a[1];
				array[4] = a[1];
				array[5] = a[1];
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

			case BagType.Bag7_oo:
				throw new NotImplementedException();
				a = (Tetromino.MinoType[])Tetromino.MINOTYPES.Clone();
				break;

			default:
				list = ((Tetromino.MinoType[])Tetromino.MINOTYPES.Clone()).ToList();
				_manager.GameData.Rng.ShuffleArray(list);
				break;
		}

		_manager.GameData.Bag.AddRange(list);
	}
}