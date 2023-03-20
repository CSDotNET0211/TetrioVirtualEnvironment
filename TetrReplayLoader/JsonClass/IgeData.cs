using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass
{

    public class IgeData
    {
        public int id { get; set; }
        public int frame { get; set; }
        public string type { get; set; }
        public GarbageData data { get; set; }
        public string sender { get; set; }
        public int sent_frame { get; set; }
        public int? cid { get; set; }
        public int lines { get; set; }
        public int column { get; set; }

    }
}
