// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using TetrEnvironment;
using TetrEnvironment.Constants;
using TetrLoader;
using TetrLoader.Enum;
using Environment = TetrEnvironment.Environment;

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool GetConsoleMode(IntPtr handle, out int mode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr GetStdHandle(int handle);


string filePath = @"C:\Users\CSDotNET\Downloads\v16__2HfCA1pM.ttrm";

// ファイルの内容をすべて読み取る


/*	// TestMode
while (true)
{
string path = Console.ReadLine();
using (StreamReader reader = new StreamReader(path))
{
	string content = reader.ReadToEnd();
	var replayData = ReplayLoader.ParseReplay(content, Util.IsMulti(ref content) ? ReplayKind.TTRM : ReplayKind.TTR);
	TestAll(replayData);
}
}*/


START: ;
// PlayMode

var handle = GetStdHandle( -11 );
int mode;
GetConsoleMode( handle, out mode );
SetConsoleMode( handle, mode | 0x4 );
/*
for (int i=0;i<255;i++ )
{
	//Console.
	if ((i + 1) % 10 == 0)
		Console.Write(i);
	Console.Write( "\x1b[48;5;" + i + "m　" );
	if ((i + 1) % 10 == 0)
		Console.WriteLine();
}
Console.ReadLine();
*/




using (StreamReader reader = new StreamReader(filePath))
{
	string content = reader.ReadToEnd();
	var replayData = ReplayLoader.ParseReplay(content, Util.IsMulti(ref content) ? ReplayKind.TTRM : ReplayKind.TTR);
	Replay replay = new Replay(replayData);
	replay.LoadGame(4);

	while (true)
	{
		PrintBoard(replay.Environments);
		var input = Console.ReadLine();
		if (input == "")
		{
			if (!replay.NextFrame())
				break;
		}
		else
			replay.JumpFrame(int.Parse(input));
	}
}

goto START;

Console.ReadKey();

void TestAll(IReplayData data)
{
	var replayCount = data.GetReplayCount();
	Console.WriteLine("テスト開始");
	Console.WriteLine($"ゲーム数:{replayCount}");

	for (int replayIndex = 0; replayIndex < replayCount; replayIndex++)
	{
		Console.WriteLine($"{replayIndex + 1}番目のテスト");
		Console.WriteLine($"予想フレーム数:{data.GetEndEventFrame(0, replayIndex)}/{data.GetEndEventFrame(1, replayIndex)}");
		Replay replay = new Replay(data);
		replay.LoadGame(replayIndex);

		while (true)
		{
			if (!replay.NextFrame())
				break;
		}

		Console.WriteLine($"終了フレーム:{replay.Environments[0].CurrentFrame}/{replay.Environments[1].CurrentFrame}");
	}

	Console.WriteLine("テスト正常終了");
}

void PrintBoard(List<Environment> environments)
{
	for (int playerIndex = 0; playerIndex < environments.Count; playerIndex++)
	{
		string output = "";
		Console.CursorLeft = 0;
		Console.CursorTop = playerIndex * 30;


		//output += "Player" + (playerIndex + 1) + "\r\n";
		output += "Player:" + environments[playerIndex].Username + "\r\n";

		output += "CurrentFrame:";
		output += environments[playerIndex].CurrentFrame + "\r\n";
		output += environments[playerIndex].GameData.Falling.LockResets + "\r\n";
		output += environments[playerIndex].GameData.Falling.Locking.ToString("00") + "\r\n";
		foreach (var next in environments[playerIndex].GameData.Bag)
		{
			output += next.ToString();
		}

		output += "\n";


		var newBoard = (Tetromino.MinoType[])environments[playerIndex].GameData.Board.Clone();

		if (environments[playerIndex].GameData.Falling.Type != Tetromino.MinoType.Empty)
			foreach (var pos in Tetromino.SHAPES[(int)environments[playerIndex].GameData.Falling.Type][
				         environments[playerIndex].GameData.Falling.R])
			{
				newBoard[(int)((pos.x + environments[playerIndex].GameData.Falling.X -
				                Tetromino.DIFFS[(int)environments[playerIndex].GameData.Falling.Type].x) +
				               (int)(pos.y + (int)Math.Ceiling(environments[playerIndex].GameData.Falling.Y) - Tetromino
					               .DIFFS[(int)environments[playerIndex].GameData.Falling.Type].y) * 10)] =
					environments[playerIndex].GameData.Falling.Type;
			}


		for (int y = 19 - 2; y < 40; y++)
		{
			for (int x = 0; x < 10; x++)
			{
				switch (newBoard[x + y * 10])
				{
					case Tetromino.MinoType.Empty:
						output += "\x1b[48;5;" + 0 + "m　" ;
						break;
					
					case Tetromino.MinoType.Z:
						output += "\x1b[48;5;" + 196 + "m　" ;
						break;
					
					case Tetromino.MinoType.S:
						output += "\x1b[48;5;" + 10 + "m　" ;
						break;
					
					case Tetromino.MinoType.O:
						output += "\x1b[48;5;" + 226 + "m　" ;
						break;
					
					case Tetromino.MinoType.J:
						output += "\x1b[48;5;" + 21 + "m　" ;
						break;
					
					case Tetromino.MinoType.I:
						output += "\x1b[48;5;" + 39 + "m　" ;
						break;
						
					case Tetromino.MinoType.L:
						output += "\x1b[48;5;" + 208 + "m　" ;
						break;
					
					case Tetromino.MinoType.Garbage:
						output += "\x1b[48;5;" + 8 + "m　" ;
						break;
					
					case Tetromino.MinoType.T:
						output += "\x1b[48;5;" + 128 + "m　" ;
						break;
					
				}
				
				/*if (newBoard[x + y * 10] == Tetromino.MinoType.Empty)
				{
					output += "□";
				}
				else
				{
					output += "■";
				}*/
			}

			output += "\r\n";
		}

		output += "\r\n";
		output += "\x1b[48;5;" + 0 + "m　" ;
		Console.WriteLine(output);
	}


	//debug
	for (int playerIndex = 0; playerIndex < environments.Count; playerIndex++)
	{
		Console.CursorLeft = 35;
		Console.CursorTop = playerIndex * 31;

		Console.Write("                                                                 ");
		Console.CursorLeft = 35;
		foreach (var garbage in environments[playerIndex].GameData.ImpendingDamage)
		{
			Console.Write($"amt:{garbage.amt} status:{garbage.status} id:{garbage.id}  status:{garbage.active} /");
		}

		Console.CursorLeft = 35;
		Console.CursorTop = playerIndex * 32;

		Console.Write("                                                                 ");
		Console.CursorLeft = 35;
		foreach (var garbage in environments[playerIndex].GameData.GarbageAckNowledgements.Outgoing.ToList())
		{
			Console.Write($"{garbage.Key}:{garbage.Value.Count}/");
		}
	}
}