
namespace TetrReplayLoader.JsonClass
{
    /// <summary>
    /// リプレイファイルのデータ
    /// </summary>
    public class ReplayDataTTR
    {
        public User? user { get; set; } = null;
        //TTR
        public string? _id { get; set; } = null;
        public string? shortid { get; set; } = null;
        public bool ismulti { get; set; } = false;
        public EndContext? endcontext { get; set; } = null;

        public string? ts { get; set; } = null;
        public string? gametype { get; set; } = null;
        public bool? verified { get; set; } = null;
        /// <summary>
        /// 試合ごとのデータ群
        /// </summary>
        public ReplayEvent? data { get; set; } = null;
        public string? back { get; set; } = null;

        //TTRM
    }
}
