using TetrEnvironment.Struct;

namespace TetrEnvironment.Constants.Kickset;

public abstract class KicksetBase
{
	public abstract Vector2[]? ADDITIONAL_OFFSETS { get; protected set; }

	public abstract Vector2[] ADDITIONAL_OFFSETS_EMPTY { get; protected set; }

	public abstract int[]? SPAWN_ROTATION { get; protected set; }
	public abstract Dictionary<int, Vector2[]> KICKSET_TABLE{ get; protected set; }
	public abstract Dictionary<int, Vector2[]> KICKSET_TABLE_I{ get; protected set; }
}