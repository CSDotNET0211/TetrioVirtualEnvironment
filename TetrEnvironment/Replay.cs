using TetrLoader;
using TetrLoader.JsonClass;

namespace TetrEnvironment;

/// <summary>
/// basically, manage replay over this class
/// </summary>
public class Replay
{
	public enum ReplayStatusEnum
	{
		Initialized,
		Loaded,
		End
	}

	public ReplayStatusEnum ReplayStatus;

	public readonly IReplayData ReplayData;

	public List<Environment> Environments { get; private set; }
	public string[] Usernames { get; private set; }
	public int LoadedIndex { get; private set; }

	public Replay(IReplayData replayData)
	{
		LoadedIndex = -1;
		Environments = new List<Environment>();
		ReplayData = replayData;
		ReplayStatus = ReplayStatusEnum.Initialized;
		Usernames = ReplayData.GetUsernames();
	}

	/// <summary>
	/// Load game you select to play
	/// </summary>
	/// <param name="gameIndex"></param>
	/// <exception cref="Exception"></exception>
	public void LoadGame(int gameIndex)
	{
		Environments.Clear();

		if (Usernames.Length > 2)
			throw new Exception("over 3 players replay is not supported");

		for (int playerIndex = 0; playerIndex < Usernames.Length; playerIndex++)
		{
			var events = ReplayData.GetReplayEvents(Usernames[playerIndex], gameIndex);
			(ReplayData as ReplayDataTTRM)?.ProcessReplayData((ReplayData as ReplayDataTTRM), events);
				Environments.Add(
				new Environment(events, ReplayData.GetGameType()));
		}

		LoadedIndex = gameIndex;
		ReplayStatus = ReplayStatusEnum.Loaded;
	}

	/// <summary>
	/// Jump to the frame
	/// </summary>
	/// <param name="frame">frame to jump</param>
	public void JumpFrame(int frame)
	{
		if (Environments[0].CurrentFrame > frame)
		{
			foreach (var environment in Environments)
				environment.Reset();

			while (frame > Environments[0].CurrentFrame)
			{
				if (!NextFrame())
					break;
			}

			Console.Clear();
		}
		else
		{
			while (frame > Environments[0].CurrentFrame)
			{
				if (!NextFrame())
					break;
			}

			Console.Clear();
		}
	}

	/// <summary>
	/// Step to next frame
	/// </summary>
	/// <returns>next frame available</returns>
	public bool NextFrame()
	{
		int endEnvCount = 0;

		foreach (var environment in Environments)
		{
			if (!environment.NextFrame())
				endEnvCount++;
		}

		if (endEnvCount == Environments.Count)
		{
			ReplayStatus = ReplayStatusEnum.End;
			return false;
		}

		return true;
	}
}