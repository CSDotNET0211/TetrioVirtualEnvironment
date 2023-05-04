using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrioVirtualEnvironment.System;

namespace TetrioVirtualEnvironment
{
	public class ClassManager
	{
		public GameData GameData;
		public Environment Environment;

		private Stats? _stats=null;
		public Stats Stats
		{
			get
			{
				_stats??=new Stats();
				return _stats;
			}
		}

	}
}
