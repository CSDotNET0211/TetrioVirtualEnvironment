using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.Constants;

namespace TetrioVirtualEnvironment
{
	public class MinoPosition
	{
		public MinoPosition(Tetrimino.MinoType type, int x, int y, int r)
		{
			this.type = type;
			this.x = x;
			this.y = y;
			this.r = r;
		}

		public MinoPosition(Falling falling)
		{
			type = falling.Type;
			x = falling.X;
			y = (int)falling.Y;
			r = falling.R;
		}

	public Tetrimino.MinoType type;
		public int x;
		public int y;
		public int r;
	}


}
