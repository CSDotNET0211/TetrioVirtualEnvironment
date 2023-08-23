namespace TetrioVirtualEnvironment
{
	public class PlayerOptions
	{
		public PlayerOptions(double arr, double das, double dcd, int sdf, double safelock, bool cancel)
		{
			ARR = arr;
			DAS = das;
			DCD = dcd;
			SDF = sdf;
			SafeLock = safelock;
			Cancel = cancel;

		}

		public double ARR { get; internal set; }
		public double DAS { get; internal set; }
		public double DCD { get; internal set; }
		public int SDF { get; internal set; }
		public double SafeLock { get; internal set; }
		public bool Cancel { get; internal set; }

	}
}
