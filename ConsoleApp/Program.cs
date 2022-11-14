// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.IO;
using System.Text;
using TetrioVirtualEnvironment;
using static System.Net.Mime.MediaTypeNames;
using static TetrioVirtualEnvironment.Environment;
using Environment = TetrioVirtualEnvironment.Environment;



Console.WriteLine("読み込むファイルを選択してください。");
string filePath = Console.ReadLine();
StreamReader reader = new StreamReader(filePath, Encoding.UTF8);



if (Path.GetExtension(filePath) == ".ttr")
{
    Console.WriteLine("ttmファイルを検出しました。");
    Console.WriteLine("Spaceキーを押してリプレイを再生します。");



}
else
{
    Console.WriteLine("ttrmファイルを検出しました。");
    Console.WriteLine("Spaceキーを押してリプレイを再生します。");
}

Console.ReadKey();
Console.Clear();
Replay replay = new Replay(reader.ReadToEnd(), Path.GetExtension(filePath) == ".ttr");

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
        break;
    }

    Print(replay);
    Console.ReadKey();

    nextFrame += period;
}




Console.WriteLine("Replay End");
Console.WriteLine("Press Any Key to Exit...");
Console.ReadKey();

void Print(Replay replay)
{
    Console.CursorLeft = 0;
    Console.CursorTop = 0;

    string output = "";

    for (int playerIndex = 0; playerIndex < replay.PlayerCount; playerIndex++)
    {
        output += "Player" + (playerIndex + 1) + "\r\n";
        output+="CurrentFrame:";
        output += replay.Environments[playerIndex].CurrentFrame+"\r\n";
        var tempfield = (int[])replay.Environments[playerIndex].GameDataInstance.Field.Clone();

        if (replay.Environments[playerIndex].GameDataInstance.Falling.type != -1)
            foreach (var pos in TETRIMINOS[replay.Environments[playerIndex].GameDataInstance.Falling.type][replay.Environments[playerIndex].GameDataInstance.Falling.r])
            {
                tempfield[(int)((pos.x + replay.Environments[playerIndex].GameDataInstance.Falling.x - TETRIMINO_DIFF[replay.Environments[playerIndex].GameDataInstance.Falling.type].x) +
                    (int)(pos.y + replay.Environments[playerIndex].GameDataInstance.Falling.y - TETRIMINO_DIFF[replay.Environments[playerIndex].GameDataInstance.Falling.type].y) * 10)] = (int)MinoKind.Z;
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

        int cursorTop = 10 + (playerIndex * 30);

        for (int i = 0; i < 5; i++)
        {
            Console.CursorLeft = 30;
            Console.CursorTop = cursorTop;

            if (i >= replay.Environments[playerIndex].DownKeys.Count)
                Console.Write("                      ");
            else
                Console.Write(replay.Environments[playerIndex].DownKeys[i]);

            cursorTop++;
        }
    }

    Console.WriteLine(output);

  
   
    /*

    output += "\r\n";
    output += "Current Frame:" + replay.CurrentFrame + "\r\n";
    output += "Current Time:" + replay.CurrentFrame / 60f;
    Console.WriteLine(output);

    int cursorTop = 10;
    Console.CursorLeft = 30;
    Console.CursorTop = cursorTop;

    Console.WriteLine("TotalRotations " + replay.Environments[0].GameDataInstance.TotalRotations + "     ");
    Console.CursorLeft = 30;
    Console.WriteLine("Falling.SafeLock " + replay.Environments[0].GameDataInstance.Falling.SafeLock + "     ");
    Console.CursorLeft = 30;
    Console.WriteLine("Falling.Locking " + replay.Environments[0].GameDataInstance.Falling.Locking + "                        ");
    Console.CursorLeft = 30;
    Console.WriteLine("Falling.Clamped " + replay.Environments[0].GameDataInstance.Falling.Clamped + "     ");
    Console.CursorLeft = 30;
    Console.WriteLine("Falling.Floored " + replay.Environments[0].GameDataInstance.Falling.Floored + "     ");
    Console.CursorLeft = 30;
    Console.WriteLine("Falling.LockResets " + replay.Environments[0].GameDataInstance.Falling.LockResets + "     ");
    Console.CursorLeft = 30;

    Console.CursorTop++;
    if (replay.Environments[0].GameDataInstance.Hold == null)
        Console.WriteLine("HOLD *NONE*     ");
    else
        Console.WriteLine("HOLD " + (MinoKind)replay.Environments[0].GameDataInstance.Hold + "     ");
    Console.CursorLeft = 30;
    Console.Write("NEXT ");
    foreach (var next in replay.Environments[0].GameDataInstance.Next)
        Console.Write((MinoKind)next + " ");
    Console.Write("\r\n");
  
    Console.CursorLeft = 30;  */


}
