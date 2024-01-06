﻿// See https://aka.ms/new-console-template for more information

using System.Data.Common;
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

var handle = GetStdHandle(-11);
int mode;
GetConsoleMode(handle, out mode);
SetConsoleMode(handle, mode | 0x4);

Console.WriteLine("Enter replay filepath.");
string input = Console.ReadLine();

using (StreamReader reader = new StreamReader(input))
{
	string content = reader.ReadToEnd();
	var replayData =
		ReplayLoader.ParseReplay(content, Util.IsMulti(ref content) ? ReplayKind.TTRM : ReplayKind.TTR);
	Replay replay = new Replay(replayData);

	while (true)
	{
		Console.Clear();
		Console.WriteLine("Select game to play.");
		for (int i = 0; i < replayData.GetGamesCount(); i++)
		{
			Console.WriteLine(i);
		}


		replay.LoadGame(int.Parse(Console.ReadLine()));
		Console.Clear();
		while (true)
		{
			//PrintBoard(replay.Environments, true);
			//	input = Console.ReadLine();
			if (input == "" || true)
			{
				if (!replay.NextFrame())
					break;
			}
			else
				replay.JumpFrame(int.Parse(input));
		}
	}
}


void PrintBoard(List<Environment> environments, bool clearConsole)
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

		output += "CurrentFrame:";
		output += environments[playerIndex].CurrentFrame + "\r\n";

		for (int i = 0; i < environments[playerIndex].PressingKeys.Length; i++)
		{
			output += ((KeyType)i).ToString() + ":" + (environments[playerIndex].PressingKeys[i] ? "1" : "0");
			output += " ";
		}

		output += "\r\n";
		output += "Garbage ";
		foreach (var garbage in environments[playerIndex].GameData.ImpendingDamage)
		{
			output += garbage.id + " " + garbage.amt + " " + garbage.status.ToString() + " " + garbage.active + " / ";
		}

		output += "\r\n";
		output += "WaitingFrames  ";
		foreach (var waitingframe in environments[playerIndex].GameData.WaitingFrames)
		{
			output += waitingframe.target + " " + waitingframe.type.ToString() + " / ";
		}

		output += "\r\n";

		output += "Sleep:" + (environments[playerIndex].GameData.Falling.Sleep ? "1" : "0") + " ";
		output += "DeepSleep:" + (environments[playerIndex].GameData.Falling.DeepSleep ? "1" : "0") + "\r\n";

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
						output += "\x1b[48;5;" + 0 + "m　";
						break;

					case Tetromino.MinoType.Z:
						output += "\x1b[48;5;" + 196 + "m　";
						break;

					case Tetromino.MinoType.S:
						output += "\x1b[48;5;" + 10 + "m　";
						break;

					case Tetromino.MinoType.O:
						output += "\x1b[48;5;" + 226 + "m　";
						break;

					case Tetromino.MinoType.J:
						output += "\x1b[48;5;" + 21 + "m　";
						break;

					case Tetromino.MinoType.I:
						output += "\x1b[48;5;" + 39 + "m　";
						break;

					case Tetromino.MinoType.L:
						output += "\x1b[48;5;" + 208 + "m　";
						break;

					case Tetromino.MinoType.Garbage:
						output += "\x1b[48;5;" + 8 + "m　";
						break;

					case Tetromino.MinoType.T:
						output += "\x1b[48;5;" + 128 + "m　";
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
		output += "\x1b[48;5;" + 0 + "m　";
		Console.WriteLine(output);
	}


	//debug
	return;
	for (int playerIndex = 0; playerIndex < environments.Count; playerIndex++)
	{
		Console.CursorLeft = 35;
		Console.CursorTop = playerIndex * 31;

		Console.Write("                                                                 ");
		Console.CursorLeft = 35;
		foreach (var garbage in environments[playerIndex].GameData.ImpendingDamage)
		{
			Console.Write(
				$"amt:{garbage.amt} status:{garbage.status} id:{garbage.id}  status:{garbage.active} /");
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