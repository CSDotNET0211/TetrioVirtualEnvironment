using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrLoader
{
	public class ConfigLoader
	{

	}

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Bgmtweak
	{
	}

	public class Controls
	{
		public string style { get; set; }
		public Custom custom { get; set; }
		public double sensitivity { get; set; }
		public string vibration { get; set; }
	}

	public class Custom
	{
		public List<string> moveLeft { get; set; }
		public List<string> moveRight { get; set; }
		public List<string> softDrop { get; set; }
		public List<string> hardDrop { get; set; }
		public List<string> rotateCCW { get; set; }
		public List<string> rotateCW { get; set; }
		public List<string> rotate180 { get; set; }
		public List<string> hold { get; set; }
		public List<string> exit { get; set; }
		public List<string> retry { get; set; }
		public List<string> chat { get; set; }
		public List<object> target1 { get; set; }
		public List<object> target2 { get; set; }
		public List<object> target3 { get; set; }
		public List<object> target4 { get; set; }
		public List<string> menuUp { get; set; }
		public List<string> menuDown { get; set; }
		public List<string> menuLeft { get; set; }
		public List<string> menuRight { get; set; }
		public List<string> menuBack { get; set; }
		public List<string> menuConfirm { get; set; }
		public List<object> openSocial { get; set; }
	}

	public class Electron
	{
		public string loginskip { get; set; }
		public string frameratelimit { get; set; }
		public bool presence { get; set; }
		public bool taskbarflash { get; set; }
		public bool anglecompat { get; set; }
		public bool adblock { get; set; }
	}

	public class Gameoptions
	{
		public bool pro_40l { get; set; }
		public bool pro_40l_alert { get; set; }
		public bool pro_40l_retry { get; set; }
		public bool stride_40l { get; set; }
		public bool pro_blitz { get; set; }
		public bool pro_blitz_alert { get; set; }
		public bool pro_blitz_retry { get; set; }
		public bool stride_blitz { get; set; }
	}

	public class Handling
	{
		public int arr { get; set; }
		public double das { get; set; }
		public int dcd { get; set; }
		public int sdf { get; set; }
		public bool safelock { get; set; }
		public bool cancel { get; set; }
	}

	public class Notifications
	{
		public bool suppress { get; set; }
		public bool forcesound { get; set; }
		public string online { get; set; }
		public string offline { get; set; }
		public string dm { get; set; }
		public string dm_pending { get; set; }
		public string invite { get; set; }
		public string other { get; set; }
	}

	public class Root
	{
		public Controls controls { get; set; }
		public Handling handling { get; set; }
		public Volume volume { get; set; }
		public Video video { get; set; }
		public Gameoptions gameoptions { get; set; }
		public Electron electron { get; set; }
		public Notifications notifications { get; set; }
	}

	public class Video
	{
		public string graphics { get; set; }
		public string caching { get; set; }
		public string actiontext { get; set; }
		public double particles { get; set; }
		public double background { get; set; }
		public string bounciness { get; set; }
		public string shakiness { get; set; }
		public double gridopacity { get; set; }
		public double boardopacity { get; set; }
		public double shadowopacity { get; set; }
		public string zoom { get; set; }
		public bool alwaystiny { get; set; }
		public bool nosuperlobbyanim { get; set; }
		public bool colorshadow { get; set; }
		public bool sidebyside { get; set; }
		public bool spin { get; set; }
		public bool chatfilter { get; set; }
		public object background_url { get; set; }
		public object background_usecustom { get; set; }
		public bool nochat { get; set; }
		public bool hideroomids { get; set; }
		public bool emotes { get; set; }
		public bool emotes_anim { get; set; }
		public bool siren { get; set; }
		public bool powersave { get; set; }
		public bool invert { get; set; }
		public bool nobg { get; set; }
		public bool chatbg { get; set; }
		public bool replaytoolsnocollapse { get; set; }
		public bool kos { get; set; }
		public bool fire { get; set; }
		public bool focuswarning { get; set; }
		public bool hidenetwork { get; set; }
		public bool guide { get; set; }
		public bool lowrescounters { get; set; }
		public bool desktopnotifications { get; set; }
		public bool lowres { get; set; }
		public string webgl { get; set; }
		public int bloom { get; set; }
		public double chroma { get; set; }
		public int flashwave { get; set; }
	}

	public class Volume
	{
		public bool disable { get; set; }
		public double music { get; set; }
		public double sfx { get; set; }
		public double stereo { get; set; }
		public bool others { get; set; }
		public bool attacks { get; set; }
		public bool next { get; set; }
		public bool noreset { get; set; }
		public bool oof { get; set; }
		public bool scrollable { get; set; }
		public Bgmtweak bgmtweak { get; set; }
	}


}
