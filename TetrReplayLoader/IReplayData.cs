using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass.Event;

namespace TetrLoader
{
	public interface IReplayData
	{
		 List<Event>? GetReplayEvents(int playerIndex, int replayIndex);
		 int GetPlayerCount();
		 Stats GetReplayStats(int playerIndex, int replayIndex);
		  int GetGameTotalFrames(int replayIndex);
		 string GetUsername(int playerIndex);
		 int GetReplayCount();









	}


}
