﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.Constants;
using static TetrioVirtualEnvironment.Environment;

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
			this.type = falling.Type;
			this.x = falling.X;
			this.y = (int)falling.Y;
			this.r = falling.R;
		}

	public Tetrimino.MinoType type;
		public int x;
		public int y;
		public int r;
	}


}
