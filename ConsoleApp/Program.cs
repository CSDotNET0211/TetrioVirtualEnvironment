// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.IO;
using System.Text;
using TetrioVirtualEnvironment;
using static System.Net.Mime.MediaTypeNames;
using static TetrioVirtualEnvironment.Environment;
using static TetrReplayLoaderLib.TetrLoader;
using Environment = TetrioVirtualEnvironment.Environment;



Console.WriteLine("読み込むファイルを選択してください。");
string filePath = Console.ReadLine();
StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
var rawjson = reader.ReadToEnd();
Replay replay;

replay = new Replay(rawjson);

int replayIndex;
int playerIndex;
Select:;


if (Path.GetExtension(filePath) == ".ttr")
{
    Console.WriteLine("ttmファイルを検出しました。");

    replayIndex = 0;
    playerIndex = 0;

}
else
{
    Console.WriteLine("たぶんttrmファイルを検出しました。");

    var replayCount = GetReplayCount(replay.ReplayData, replay.ReplayKind);
    Console.WriteLine(replayCount + "個のゲームを検出しました。");
    for (int i = 0; i < replayCount; i++)
    {
        var stats1 = GetReplayStats(replay.ReplayData, replay.ReplayKind, 0, i);
        var stats2 = GetReplayStats(replay.ReplayData, replay.ReplayKind, 0, i);

        Console.WriteLine($"{i}/    {stats1.PPS.ToString("F")}PPS {stats1.APM.ToString("F")}APM {stats1.VS.ToString("F")}VS\r\n" +
            $"           {stats2.PPS.ToString("F")}PPS {stats2.APM.ToString("F")}APM {stats2.VS.ToString("F")}VS");
    }

    replayIndex = int.Parse(Console.ReadLine());

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



    if (!replay.Update())
    {
        Print(replay);
        goto Select;
        break;
    }



    Print(replay);
    //var input = Console.ReadLine();
    //if (input != "")
    //{
    //    Console.Clear();
    //    replay.SkipFrame(int.Parse(input));
    //}

    nextFrame += period;
}




Console.WriteLine("Replay End");
Console.WriteLine("Press Any Key to Exit...");
Console.ReadKey();

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
        var tempfield = (int[])replay.Environments[playerIndex].GameData.Field.Clone();

        if (replay.Environments[playerIndex].GameData.Falling.type != -1)
            foreach (var pos in TETRIMINOS[replay.Environments[playerIndex].GameData.Falling.type][replay.Environments[playerIndex].GameData.Falling.r])
            {
                tempfield[(int)((pos.x + replay.Environments[playerIndex].GameData.Falling.x - TETRIMINO_DIFF[replay.Environments[playerIndex].GameData.Falling.type].x) +
                    (int)(pos.y + replay.Environments[playerIndex].GameData.Falling.y - TETRIMINO_DIFF[replay.Environments[playerIndex].GameData.Falling.type].y) * 10)] = (int)MinoKind.Z;
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
