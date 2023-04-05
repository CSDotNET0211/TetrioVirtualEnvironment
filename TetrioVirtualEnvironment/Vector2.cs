
namespace TetrioVirtualEnvironment
{ 
	/// <summary>
  /// Simple Structure having x,y ConstData.
  /// </summary>
    public struct Vector2
    {
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x;
        public double y;

        public static readonly Vector2 Zero = new(0, 0);
        public static readonly Vector2 One = new(1, 1);
        public static readonly Vector2 Mone = new(-1, -1);
        public static readonly Vector2 X1 = new(1, 0);
        public static readonly Vector2 Mx1 = new(-1, 0);
        public static readonly Vector2 X2 = new(2, 0);
        public static readonly Vector2 Mx2 = new(-2, 0);
        public static readonly Vector2 Y1 = new(0, 1);
        public static readonly Vector2 My1 = new(0, -1);
        public static readonly Vector2 Y2 = new(0, 2);
        public static readonly Vector2 My2 = new(0, -2);

        public static Vector2 operator +(Vector2 obj, Vector2 obj2)
        {
            return new Vector2(obj.x + obj2.x, obj.y + obj2.y);
        }

        public static Vector2 operator -(Vector2 obj, Vector2 obj2)
        {
            return new Vector2(obj.x - obj2.x, obj.y - obj2.y);
        }
        public static Vector2 operator *(Vector2 obj, Vector2 obj2)
        {
            return new Vector2(obj.x * obj2.x, obj.y * obj2.y);
        }

    }
}
