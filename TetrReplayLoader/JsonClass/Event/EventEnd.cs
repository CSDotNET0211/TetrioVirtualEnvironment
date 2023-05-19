
namespace TetrReplayLoader.JsonClass.Event
{

	public class EventEnd:Event
	{
	public EventEnd(int? id,int frame,string type,EventEndData data):base(id,frame,type){
	this.data= data;
	}

		public new EventEndData data { get; set; }
	}

	public class EventEndData
    {
        public string? reason { get; set; } = null;
        public Export? export { get; set; } = null;
    }
}
