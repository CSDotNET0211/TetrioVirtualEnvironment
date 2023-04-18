using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class WaitingFrameData
    {
        public int target { get; internal set; }
        public string type { get; internal set; }
        public object AdditionalData { get; internal set; }
    }
}
