// See https://aka.ms/new-console-template for more information
using TetrioVirtualEnvironment;
using static TetrioVirtualEnvironment.Environment;
using Environment = TetrioVirtualEnvironment.Environment;

Console.WriteLine("Hello, World!");


Replay replay = new Replay();
replay.Load("data");
while (true)
{
    Print(replay);
    if(!replay.Update())
        break;  
    
}

Console.WriteLine("Replay End");
Console.WriteLine("Press Any Key to Exit...");
Console.ReadKey();

void Print(Replay replay)
{
    Console.CursorLeft = 0;
    Console.CursorTop = 0;

    Console.WriteLine("Player1");
    var tempfield = (int[])replay.Environment.GameData.Field.Clone();
    foreach (var pos in TETRIMINOS[replay.Environment.GameData.Falling.type][replay.Environment.GameData.Falling.r])
    {
        tempfield[(int)((pos.x+ replay.Environment.GameData.Falling.x)+ (int)(pos.y + replay.Environment.GameData.Falling.y)*10)] =(int)MinoKind.Z;
    }

    for (int y = 25; y < 40; y++)
    {
        for (int x = 0; x < 10; x++)
        {
            if (tempfield[x + y * 10] == (int)MinoKind.Empty)
                Console.Write("□");
            else
                Console.Write("■");


        }

        Console.Write("\r\n");
    }

        Console.Write("\r\n");
    Console.WriteLine("Current Frame:"+ replay.CurrentFrame);



}