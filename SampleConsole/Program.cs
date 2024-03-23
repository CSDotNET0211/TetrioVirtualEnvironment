using System.Data.Common;
using System.Runtime.InteropServices;
using TetrEnvironment;
using TetrEnvironment.Constants;
using TetrLoader;
using TetrLoader.Enum;
using Environment = TetrEnvironment.Environment;

//setup for color blocks
[DllImport("kernel32.dll", SetLastError = true)]
static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool GetConsoleMode(IntPtr handle, out int mode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr GetStdHandle(int handle);

var handle = GetStdHandle(-11);
int mode;
GetConsoleMode(handle, out mode);
SetConsoleMode(handle, mode | 0x4);

string input;
do
{
	Console.WriteLine("Enter replay filepath.");
	input = Console.ReadLine();
} while (!File.Exists(input));

using (StreamReader reader = new StreamReader(input))
{
	string content = reader.ReadToEnd();
	//parse json to IReplayData
	var replayData =
		ReplayLoader.ParseReplay(ref content, Util.IsMulti(ref content) ? ReplayKind.TTRM : ReplayKind.TTR);

	Replay replay = new Replay(replayData);

	while (true)
	{
		Console.Clear();
		Console.WriteLine("Select game to play.");
		for (int i = 0; i < replayData.GetGamesCount(); i++)
			Console.WriteLine(i);

		//select game to play
		replay.LoadGame(int.Parse(Console.ReadLine()));
		Console.Clear();
		while (true)
		{
			int inputInt = -1;
			PrintBoard(replay.Environments, false);

			//	input = Console.ReadLine();
			input = string.Empty;
			int.TryParse(input, out inputInt);

			if (input == string.Empty)
			{
				//step next frame
				if (!replay.NextFrame())
					break;
			}
			else
				replay.JumpFrame(inputInt);
		}
	}
}


void PrintBoard(List<Environment> environments, bool clearConsole, bool detail = false)
{
	if (clearConsole)
		Console.Clear();

	for (int playerIndex = 0; playerIndex < environments.Count; playerIndex++)
	{
		string output = "";
		Console.CursorLeft = 0;
		Console.CursorTop = playerIndex * 30;


		//output += "Player" + (playerIndex + 1) + "\r\n";
		output += "Player:" + environments[playerIndex].Username + "\r\n";
		output += "stack:" + environments[playerIndex].CustomStats.stackCleared + "\r\n";
		output += "garbage:" + environments[playerIndex].CustomStats.garbageCleared + "\r\n";
		output += "CurrentFrame:";
		output += environments[playerIndex].CurrentFrame + "\r\n";

		if (detail)
		{
			for (int i = 0; i < environments[playerIndex].PressingKeys.Length; i++)
			{
				output += ((KeyType)i).ToString() + ":" + (environments[playerIndex].PressingKeys[i] ? "1" : "0");
				output += " ";
			}

			output += "\r\n";
			output += "Garbage ";
			foreach (var garbage in environments[playerIndex].GameData.ImpendingDamage)
			{
				output += garbage.id + " " + garbage.amt + " " + garbage.status.ToString() + " " + garbage.active +
				          " / ";
			}

			output += "\r\n";
			output += "WaitingFrames  ";
			foreach (var waitingframe in environments[playerIndex].GameData.WaitingFrames)
			{
				output += waitingframe.target + " " + waitingframe.type.ToString() + " / ";
			}

			output += "\r\n";

			output += "Sleep:" + (environments[playerIndex].GameData.Falling.Sleep ? "1" : "0") + " ";
			output += "May20G:" + (environments[playerIndex].GameData.Handling.May20G ? "1" : "0") + " ";
			output += "GravityMay20G:" + (environments[playerIndex].GameData.Options.GravityMay20G ? "1" : "0") + " ";
			output += "DeepSleep:" + (environments[playerIndex].GameData.Falling.DeepSleep ? "1" : "0") + "\r\n";
			output += "Level:" + (environments[playerIndex].GameData.Stats.Level) + "\r\n";
			output += "LevelLinesNeeded:" + (environments[playerIndex].GameData.Stats.LevelLinesNeeded) + "\r\n";
		}

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
				               (int)(pos.y + (int)Math.Ceiling(environments[playerIndex].GameData.Falling.Y) -
				                     Tetromino
					                     .DIFFS[(int)environments[playerIndex].GameData.Falling.Type].y) *
				               10)] =
					environments[playerIndex].GameData.Falling.Type;
			}


		for (int y = 19 - 2; y < 40; y++)
		{
			for (int x = 0; x < 10; x++)
			{
				switch (newBoard[x + y * 10])
				{
					case Tetromino.MinoType.Empty:
						output += "\x1b[48;5;" + 0 + "m  ";
						break;

					case Tetromino.MinoType.Z:
						output += "\x1b[48;5;" + 196 + "m  ";
						break;

					case Tetromino.MinoType.S:
						output += "\x1b[48;5;" + 10 + "m  ";
						break;

					case Tetromino.MinoType.O:
						output += "\x1b[48;5;" + 226 + "m  ";
						break;

					case Tetromino.MinoType.J:
						output += "\x1b[48;5;" + 21 + "m  ";
						break;

					case Tetromino.MinoType.I:
						output += "\x1b[48;5;" + 39 + "m  ";
						break;

					case Tetromino.MinoType.L:
						output += "\x1b[48;5;" + 208 + "m  ";
						break;

					case Tetromino.MinoType.Garbage:
						output += "\x1b[48;5;" + 8 + "m  ";
						break;

					case Tetromino.MinoType.T:
						output += "\x1b[48;5;" + 128 + "m  ";
						break;
				}
			}

			output += "\x1b[48;5;" + 0 + "m  ";
			output += "\r\n";
		}

		output += "\x1b[48;5;" + 0 + "m  ";
		output += "\r\n";
		Console.WriteLine(output);
	}
}