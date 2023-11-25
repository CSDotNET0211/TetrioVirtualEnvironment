using TetrLoader;

namespace TetrEnvironment;

public class Replay
{
	private readonly IReplayData _replayData;

	//TODO: プロパティ使ってフレーム取得
	public List<Environment> Environments { get; private set; }

	public Replay(IReplayData replayData)
	{
		Environments = new List<Environment>();
		_replayData = replayData;
	}

	//TODO: boardとreplay比較して、usernameの違いを見る
	public void LoadGame(int gameIndex)
	{
		Environments.Clear();
		int playerCount = _replayData.GetPlayerCount();
		if (playerCount > 2)
			throw new Exception("over 3 players replay is not supported");

		for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
		{
			Environments.Add(new Environment(
				_replayData.GetReplayEvents(playerIndex, gameIndex)));
		}
	}

	public void JumpFrame(int frame)
	{
		if (Environments[0].CurrentFrame > frame)
		{
			foreach (var environment in Environments)
				environment.Reset();

			while (frame != Environments[0].CurrentFrame)
			{
				NextFrame();
			}
		}
		else
		{
			while (frame != Environments[0].CurrentFrame)
			{
				NextFrame();
			}
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