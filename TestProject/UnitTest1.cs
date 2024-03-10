using System.Diagnostics;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TetrEnvironment;
using TetrLoader;
using TetrLoader.Enum;
using TetrLoader.JsonClass;
using TetrLoader.JsonClass.Event;

namespace TestProject;

public class Tests
{
	private static int test = 0;

	[SetUp]
	public void Setup()
	{
		test = 1;
	}

	[Test]
	public void Test1()
	{
	}

	[Test]
	public void Test2()
	{
		Assert.AreEqual(test, 1);
	}

	//[Test]
	public void NormalizeFiles()
	{
		string directoryPath = "C:\\Users\\CSDotNET\\RiderProjects\\TetrEnvironment\\TestProject\\TestReplays";
		string[] files = Directory.GetFiles(directoryPath, "*.ttr", SearchOption.AllDirectories);
		files = files.Concat(Directory.GetFiles(directoryPath, "*.ttrm", SearchOption.AllDirectories)).ToArray();


		string pattern = @"^\d+_[^_]+_[^_]+_\d{4}-\d{2}-\d{2}\.(ttr|ttrm)$";
		foreach (string file in files)
		{
			string fileName = Path.GetFileName(file);
			if (!Regex.IsMatch(fileName, pattern))
			{
				string newName = string.Empty;

				var json = File.ReadAllText(file);
				var isMulti = Util.IsMulti(ref json);
				var replayData = ReplayLoader.ParseReplay(ref json, isMulti ? ReplayKind.TTRM : ReplayKind.TTR);
				if (replayData is ReplayDataTTR ttr)
				{
					var eventFull =
						replayData.GetReplayEvents(null, -1).First(x => x.type == EventType.Full) as EventFull;
					newName += eventFull.data.options.version;
				}
				else if (replayData is ReplayDataTTRM ttrm)
				{
					var eventFull =
						replayData.GetReplayEvents(null, -1).First(x => x.type == EventType.Full) as EventFull;
				}


				string newFilePath = Path.Combine(Path.GetDirectoryName(file), newName);
				File.Move(file, newFilePath);
			}
		}

		Assert.Pass();
	}

	private void NormalizeFileName(string filePath)
	{
	}

	private void ParseReplay(string replayFilePath)
	{
		var json = File.ReadAllText(replayFilePath);
		var isMulti = Util.IsMulti(ref json);
		var replayData = ReplayLoader.ParseReplay(ref json, isMulti ? ReplayKind.TTRM : ReplayKind.TTR);
		Replay replay = new Replay(replayData);

		for (int gameIndex = 0; gameIndex < replayData.GetGamesCount(); gameIndex++)
		{
			replay.LoadGame(gameIndex);
			Console.WriteLine("TotalFrame:" + replay.Environments[0].TotalFrame);
			while (replay.NextFrame())
			{
			}

			foreach (var environment in replay.Environments)
			{
				if (environment.DeadFrameDiff is 0 or -1)
					Assert.Pass();
				else
				{
					Console.WriteLine("Player:" + environment.Username);
					Console.WriteLine("DeadFrameDiff:" + environment.DeadFrameDiff);
					Assert.Fail();
				}
			}
		}
	}
}