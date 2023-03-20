using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass
{
    public class EndContext
    {
        //TTRM
        public int? naturalorder { get; set; } = null;
        public User? user { get; set; } = null;
        public bool? active { get; set; } = null;
        public int? wins { get; set; } = null;
        //   public int? inputs { get; set; } = null;
        public int? piecesplaced { get; set; } = null;
        public Points? points { get; set; }


        //TTR
        public double? seed { get; set; } = null;
        public int? lines { get; set; } = null;
        public int? level_lines { get; set; } = null;
        public int? level_lines_needed { get; set; } = null;
        public int? holds { get; set; } = null;
        public int? score { get; set; } = null;
        public int? zenlevel { get; set; } = null;
        public int? zenprogress { get; set; } = null;
        public int? level { get; set; } = null;
        public int? combo { get; set; } = null;
        public int? currentcombopower { get; set; } = null;
        public int? topcombo { get; set; } = null;
        public int? btb { get; set; } = null;
        public int? topbtb { get; set; } = null;
        public int? tspins { get; set; } = null;
        public int? pieceplaced { get; set; } = null;
        public int? kills { get; set; } = null;

        //BOTH
        public int? inputs { get; set; } = null;

    }

}
