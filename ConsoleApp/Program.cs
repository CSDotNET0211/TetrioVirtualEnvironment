﻿using System.Text;
using TetrReplayLoader;
using TetrioVirtualEnvironment;
using Environment = TetrioVirtualEnvironment.Environment;
using static TetrioVirtualEnvironment.Environment;

Console.WriteLine("Select file to replay.");
string filePath = Console.ReadLine();
//string filePath = @"C:\Users\CSDotNET\Downloads\bug.ttrm";
StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
var rawJson = reader.ReadToEnd();
Replay replay;

replay = new Replay(rawJson);

int replayIndex;
int playerIndex;

Select:;

if (ReplayLoader.IsMulti(rawJson))
{
    Console.WriteLine("detected TTRM");
    var replayCount = replay.ReplayData.GetReplayCount();

    Console.WriteLine(replayCount + "Games Found.");
    for (int i = 0; i < replayCount; i++)
    {
        var stats1 = replay.ReplayData.GetReplayStats(0, i);
        var stats2 = replay.ReplayData.GetReplayStats(1, i);

        Console.WriteLine($"{i}	{stats1.PPS:F}PPS {stats1.APM:F}APM {stats1.VS:F}VS" + "	/ " +
                          $"{stats2.PPS:F}PPS {stats2.APM:F}APM {stats2.VS:F}VS");
    }

    replayIndex = int.Parse(Console.ReadLine());
}
else
{
    Console.WriteLine("detected TTR");

    replayIndex = 0;
    playerIndex = 0;
}


replay.LoadGame(replayIndex);

Console.Clear();

double nextFrame = (double)System.Environment.TickCount;
float period = 1000f / 60f;


while (true)
{
    double tickCount = System.Environment.TickCount;

    if (tickCount < nextFrame)
    {
        if (nextFrame - tickCount > 1)
            Thread.Sleep((int)(nextFrame - tickCount));

        continue;
    }

    if (System.Environment.TickCount >= nextFrame + period)
    {
        nextFrame += period;
        continue;
    }

    //replay.Environments

    if (!replay.Update())
    {
        Print(replay);
        goto Select;
    }


    Print(replay);
    //Step by step
    //var input = Console.ReadLine();
    //if (input != "")
    //{
    //    Console.Clear();
    //    replay.JumpFrame(int.Parse(input));
    //}

    nextFrame += period;
}


void Print(Replay replay)
{
    // Console.Clear();

    for (int playerIndex = 0; playerIndex < replay.Environments.Count; playerIndex++)
    {
        string output = "";
        Console.CursorLeft = 0;
        Console.CursorTop = playerIndex * 30;


        output += "Player" + (playerIndex + 1) + "\r\n";
        output += "CurrentFrame:";
        output += replay.Environments[playerIndex].CurrentFrame + "\r\n";
        var tempfield = (MinoKind?[])replay.Environments[playerIndex].GameData.Board.Clone();

        if (replay.Environments[playerIndex].GameData.Falling.Type != null)
            foreach (var pos in Environment.ConstData.TETRIMINOS_SHAPES[(int)replay.Environments[playerIndex].GameData.Falling.Type][replay.Environments[playerIndex].GameData.Falling.R])
            {
                tempfield[(int)((pos.x + replay.Environments[playerIndex].GameData.Falling.X - Environment.ConstData.TETRIMINO_DIFFS[(int)replay.Environments[playerIndex].GameData.Falling.Type].x) +
                    (int)(pos.y + replay.Environments[playerIndex].GameData.Falling.Y - Environment.ConstData.TETRIMINO_DIFFS[(int)replay.Environments[playerIndex].GameData.Falling.Type].y) * 10)] = (int)Environment.MinoKind.Z;
            }


        for (int y = 15; y < 40; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (tempfield[x + y * 10] == null)
                    output += "□";
                else
                    output += "■";


            }

            output += "\r\n";
        }
        output += "\r\n";

        Console.WriteLine(output);

    }

    for (int playerIndex = 0; playerIndex < replay.Environments.Count; playerIndex++)
    {
        Console.CursorLeft = 35;
        Console.CursorTop = playerIndex * 31;

        Console.Write("                                                                 ");
        Console.CursorLeft = 35;
        foreach (var garbage in replay.Environments[playerIndex].GameData.ImpendingDamages)
        {
            Console.Write($"line:{garbage.lines} active:{garbage.active} id:{garbage.id} /");
        }
    }

}
