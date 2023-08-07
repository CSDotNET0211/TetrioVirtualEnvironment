namespace TetrioVirtualEnvironment;

public class EventManager
{
	public EventManager()
	{
	}

	
	public struct AttackData
	{
		public AttackData(int garbageClearCount, int stackClearCount, int attackLine)
		{
			GarbageClearCount = garbageClearCount;
			StackClearCount = stackClearCount;
			AttackLine = attackLine;
		}

		public int GarbageClearCount;
		public int StackClearCount;
		public int AttackLine;
	}

	public event EventHandler? OnPiecePlaced;
	public event EventHandler? OnPieceCreated;
	public event EventHandler? OnAttackLined;

	public void Trigger_OnPiecePlaced(object obj, EventArgs e)
		=> OnPiecePlaced?.Invoke(obj, e);


	public void Trigger_OnAttackLined(int garbageClearCount, int stackClearCount, int attackLine, EventArgs e)
		=> OnAttackLined(new AttackData(garbageClearCount, stackClearCount, attackLine), e);

	public void Trigger_OnPieceCreated(object obj, EventArgs e)
		=> OnPieceCreated?.Invoke(obj, e);
}