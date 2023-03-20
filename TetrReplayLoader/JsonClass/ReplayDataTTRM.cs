using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrReplayLoader.JsonClass
{
    /// <summary>
    /// リプレイファイルのデータ
    /// </summary>
    public class ReplayDataTTRM
    {
        public string? _id { get; set; } = null;
        public string? shortid { get; set; } = null;
        public bool ismulti { get; set; } = false;
        public List<EndContext>? endcontext { get; set; } = null;

        public string? ts { get; set; } = null;
        public string? gametype { get; set; } = null;
        public bool? verified { get; set; } = null;
        /// <summary>
        /// 試合ごとのデータ群
        /// </summary>
        public List<PlayDataTTRM>? data { get; set; } = null;
        public string? back { get; set; } = null;
    }

}
