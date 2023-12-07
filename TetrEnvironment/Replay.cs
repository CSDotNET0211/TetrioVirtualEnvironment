using TetrLoader;
using TetrLoader.JsonClass;

namespace TetrEnvironment;

public class Replay
{
	private readonly IReplayData _replayData;

	//TODO: プロパティ使ってフレーム取得
	public List<Environment> Environments { get; private set; }
	public string[] Usernames { get; private set; }

	public Replay(IReplayData replayData)
	{
		Environments = new List<Environment>();
		_replayData = replayData;
	}

	public void LoadGame(int gameIndex)
	{
		Usernames = _replayData.GetUsernames();

		Environments.Clear();

		if (Usernames.Length > 2)
			throw new Exception("over 3 players replay is not supported");

		for (int playerIndex = 0; playerIndex < Usernames.Length; playerIndex++)
		{
			var events = _replayData.GetReplayEvents(Usernames[playerIndex], gameIndex);
			(_replayData as ReplayDataTTRM)?.ProcessReplayData((_replayData as ReplayDataTTRM), events);
			Environments.Add(new Environment(events));
		}
	}

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

	public bool NextFrame()
	{
		var flag = false;
		foreach (var environment in Environments)
		{
			if (!environment.NextFrame())
				flag = true;
		}

		if (flag)
			return false;
		else
			return true;
	}
}