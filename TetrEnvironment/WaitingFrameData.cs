using TetrEnvironment.Info;

namespace TetrEnvironment;

public class WaitingFrameData
{
	public int target { get; internal set; }
	public GarbageInfo.WaitingFrameType type { get; internal set; }
	public object data { get; internal set; }
}