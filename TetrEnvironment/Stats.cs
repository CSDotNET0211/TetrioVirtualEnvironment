namespace TetrEnvironment;

public class Stats
{
	public int Combo { get; internal set; }
	public int TopCombo { get; internal set; }
	public int BTB { get; internal set; }
	public int TopBTB { get; internal set; }
	public double CurrentComboPower { get; internal set; }

	public Stats()
	{
		Combo = 0;
		TopCombo = 0;
		BTB = 0;
		TopBTB = 0;
		CurrentComboPower = 0;
	}
}