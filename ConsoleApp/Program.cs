﻿using System.Text;
using TetrioVirtualEnvironment;
using static TetrioVirtualEnvironment.Environment;
using static TetrReplayLoader.TetrLoader;



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


if (Path.GetExtension(filePath) == ".ttr")
{
    Console.WriteLine("detected TTR");

    replayIndex = 0;
    playerIndex = 0;

}
else if (Path.GetExtension(filePath) == ".ttrm")
{
    Console.WriteLine("detected TTRM");

    var replayCount = GetReplayCount(replay.ReplayData, replay.ReplayKind);
    Console.WriteLine(replayCount + "個のゲームを検出しました。");
    for (int i = 0; i < replayCount; i++)
    {
        var stats1 = GetReplayStats(replay.ReplayData, replay.ReplayKind, 0, i);
        var stats2 = GetReplayStats(replay.ReplayData, replay.ReplayKind, 0, i);

        Console.WriteLine($"{i}/    {stats1.PPS:F}PPS {stats1.APM:F}APM {stats1.VS:F}VS\r\n" +
            $"           {stats2.PPS:F}PPS {stats2.APM:F}APM {stats2.VS:F}VS");
    }

    replayIndex = int.Parse(Console.ReadLine());

}else
    throw new Exception();


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



    if (!replay.Update())
    {
        Print(replay);
        goto Select;
    }


    Print(replay);
    //Step by step
    var input = Console.ReadLine();
    if (input != "")
    {
        Console.Clear();
        replay.JumpFrame(int.Parse(input));
    }

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
        output += replay.Environments[playerIndex].CurrentFrame+ "\r\n";
        var tempfield = (int[])replay.Environments[playerIndex].GameData.Field.Clone();

        if (replay.Environments[playerIndex].GameData.Falling.Type != -1)
            foreach (var pos in TETRIMINOS[replay.Environments[playerIndex].GameData.Falling.Type][replay.Environments[playerIndex].GameData.Falling.R])
            {
                tempfield[(int)((pos.x + replay.Environments[playerIndex].GameData.Falling.X - TETRIMINO_DIFF[replay.Environments[playerIndex].GameData.Falling.Type].x) +
                    (int)(pos.y + replay.Environments[playerIndex].GameData.Falling.Y - TETRIMINO_DIFF[replay.Environments[playerIndex].GameData.Falling.Type].y) * 10)] = (int)MinoKind.Z;
            }


        for (int y = 15; y < 40; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (tempfield[x + y * 10] == (int)MinoKind.Empty)
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
        Console.CursorLeft = 12;
        Console.CursorTop = playerIndex * 30;

        var garbage = replay.Environments[playerIndex].Garbages.GarbageCount();
        Console.WriteLine("予告:" + garbage.NotConfirmed + " / 準備:" + garbage.Confirmed + " / 確定:" + garbage.Ready);
        Console.WriteLine("SafeLock:" + replay.Environments[playerIndex].GetData("safelock"));
    }

}
