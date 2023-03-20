using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{ /// <summary>
  /// Simple Structure having x,y Data.
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

        public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 one = new Vector2(1, 1);
        public static readonly Vector2 mone = new Vector2(-1, -1);
        public static readonly Vector2 x1 = new Vector2(1, 0);
        public static readonly Vector2 mx1 = new Vector2(-1, 0);
        public static readonly Vector2 x2 = new Vector2(2, 0);
        public static readonly Vector2 mx2 = new Vector2(-2, 0);
        public static readonly Vector2 y1 = new Vector2(0, 1);
        public static readonly Vector2 my1 = new Vector2(0, -1);
        public static readonly Vector2 y2 = new Vector2(0, 2);
        public static readonly Vector2 my2 = new Vector2(0, -2);

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
