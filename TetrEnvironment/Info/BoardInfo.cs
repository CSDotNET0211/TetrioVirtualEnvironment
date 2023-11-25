﻿using System.ComponentModel.DataAnnotations;
using TetrEnvironment.Constants;

namespace TetrEnvironment.Info;

public class BoardInfo
{
	private readonly Environment　_manager;

	//default size for now
	public readonly int Width = 10;
	public readonly int Height = 40;
	public readonly int VisibleHeight = 20;


	public BoardInfo(Environment manager)
	{
		_manager = manager;
	}

	public void ColumnWidth()
	{
	}

	/// <summary>
	/// get completed lines
	/// </summary>
	public List<int> GetFullLines()
	{
		List<int> completedLine = new List<int>();
		for (int y = 0; y < Height; y++)
		{
			bool flag = true;
			for (int x = 0; x < Width; x++)
			{
				if (_manager.GameData.Board[x + y * Width] == Tetromino.MinoType.Empty)
				{
					flag = false;
					break;
				}
			}

			if (flag)
				completedLine.Add(y);
		}

		return completedLine;
	}

	public void SetupBoard(out Tetromino.MinoType[]? board)
	{
		board = new Tetromino.MinoType[Width * Height];
		for (int i = 0; i < board.Length; i++)
			board[i] = Tetromino.MinoType.Empty;
	}

	public void LoadMap(Tetromino.MinoType[]? board)
	{
	}

	public void BoardToMap()
	{
	}

	public bool IsLegalAtPos(Tetromino.MinoType type, int x, double y, int r)
	{
		//NOTE: dx dyは0だから省略
		var positions = Tetromino.SHAPES[(int)type][r];
		var diff = Tetromino.DIFFS[(int)type];

		for (int index = 0; index < positions.Length; index++)
		{
			if (IsOccupied(x + ((int)positions[index].x - (int)diff.x),
				    y + (positions[index].y - diff.y)))
				return false;
		}

		return true;

		foreach (var position in positions)
		{
			var testx = (int)(position.x + x - diff.x);
			var testy = (int)(position.y + y - diff.y);
			if (!(testx >= 0 && testx < Width &&
			      testy >= 0 && testy < Height &&
			      _manager.GameData.Board[testx + testy * Width] == Tetromino.MinoType.Empty
			    ))
				return false;
		}

		return true;
		/*
		var position = 10 + Math.Ceiling(y) * (2 * Width) + x;
		bool l = true;
		for(int s=0;s<Tetromino.MINOTYPES)
		*/
	}

	public void PushActiveToStack()
	{
		PushTetrominoToStack(_manager.GameData.Falling.Type,
			_manager.GameData.Falling.X,
			_manager.GameData.Falling.Y,
			_manager.GameData.Falling.R);
	}


	public void PushTetrominoToStack(Tetromino.MinoType type, int x, double y, int r)
	{
		var data = Tetromino.SHAPES[(int)type][r];
		r = 0;

		for (int blockIndex = 0; blockIndex < data.Length; blockIndex++)
		{
			int testx = (int)data[blockIndex].x - (int)Tetromino.DIFFS[(int)type].x;
			int testy = (int)data[blockIndex].y - (int)Tetromino.DIFFS[(int)type].y;
			r = Math.Max((int)Math.Ceiling(y) + testy, r);
			_manager.GameData.Board[((x + testx) + ((int)Math.Ceiling(y) + testy) * Width)] =
				_manager.GameData.Falling.Type;
		}

/*
		foreach (var pos in Tetromino.SHAPES[(int)type][r])
		{
			r = Math.Max((int)(Math.Ceiling(y) + (pos.y)), r);

			_manager.GameData.Board[(int)((pos.x + x - Tetromino.DIFFS[(int)type].x) +
			                              (int)(Math.Ceiling(pos.y) + y - Tetromino.DIFFS[(int)type].y) * 10)]
				= type;
		}*/
	}

	public void RemoveLinesFromStack(List<int> completedLines)
	{
		//TODO: やれ
		//下から消す
		foreach (var value in completedLines)
		{
			for (int y = value; y >= 0; y--)
			{
				for (int x = 0; x < Width; x++)
				{
					if (y - 1 == -1)
						_manager.GameData.Board[x + y * Width] = Tetromino.MinoType.Empty;
					else
						_manager.GameData.Board[x + y * Width] =
							_manager.GameData.Board[x + (y - 1) * Width];
				}
			}
		}
	}

	//TODO: make it better performance
	public bool PushGarbageLine(int column, int size)
	{
		var newBoard = new List<Tetromino.MinoType>(_manager.GameData.Board);
		var data = MaybeCalcHolePos(column, size);
		List<Tetromino.MinoType> newColumn = new List<Tetromino.MinoType>();

		for (int xIndex = 0; xIndex < _manager.BoardInfo.Width; xIndex++)
		{
			var pushData = Tetromino.MinoType.Garbage;

			if (data.Contains(xIndex))
			{
				pushData = Tetromino.MinoType.Empty;
			}
			else
			{
				//	if(data.Contains(xIndex+1)||xIndex==_manager.BoardInfo.Width-1)
			}

			newColumn.Add(pushData);
		}

		var ypos = HighestLineOfPerma();

		newBoard.RemoveRange(0, Width);
		newBoard.InsertRange((ypos - 1) * Width, newColumn.ToArray());
		_manager.GameData.Board = newBoard.ToArray();
		return true;
	}

	//perma line is not supported
	public int HighestLineOfPerma()
	{
		return Height;
	}

	public HashSet<int> MaybeCalcHolePos(int column, int size)
	{
		var garbageholes = new HashSet<int>();
		garbageholes.Add(column);

		int nIndex = 1;
		int formula = 0;
		for (; nIndex < size; nIndex++)
			formula = column + (formula + 1) >= _manager.BoardInfo.Width ? -1 :
				Math.Sign(formula) >= 0 ? formula + 1 : formula - 1;

		garbageholes.Add(column + formula);
		return garbageholes;
	}

	public bool IsOccupied(int x, double y)
	{
		//NOTE: 本来はy軸そのものが存在するかもチェックしてる
		//↑やっぱこれないと配列外参照
		if (x < 0 ||
		    x >= Width ||
		    y < 0 ||
		    y >= Height ||
		    //   !(_manager.GameData.Board.Length / Width > (int)Math.Ceiling(y) &&
		    (x + (int)Math.Ceiling(y) * Width) >= _manager.GameData.Board.Length ||
		    _manager.GameData.Board[x + (int)Math.Ceiling(y) * Width] !=
		    Tetromino.MinoType.Empty)
			return true;

		return false;
	}

	public void AreWeToppedYet()
	{
		//none
	}
}