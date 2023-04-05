using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
	/// <summary>
	/// TETR.IO Garbage Structure
	/// </summary>
	public class Garbage
	{
		/// <summary>
		/// Interaction:            Arrived Garbage but that just show up in the game. After receiving InteractionConfirm, Changed to Ready mode by 20 frames. You can already cancel this by attack power.
		/// Interaction_Confirm:    Count down started to become Ready state.
		/// Ready:					If you drop piece in this state, garbaege will appear.
		/// Attack:					Interrupt garbage.
		/// </summary>	
		public enum GarbageKind
		{
			Interaction,
			InteractionConfirm,
			Ready,
			Attack
		}

		public Garbage(int interactionFrame, int confirmedFrame, int sentFrame, int posX, int power, GarbageKind state)
		{
			InteractionFrame = interactionFrame;
			ConfirmedFrame = confirmedFrame;
			SentFrame = sentFrame;
			PosX = posX;
			Power = power;
			State = state;
		}

		public int ConfirmedFrame { get; internal set; }
		public int InteractionFrame { get; internal set; }
		public int SentFrame { get; internal set; }
		public int PosX { get; internal set; }
		public int Power { get; internal set; }
		public GarbageKind State { get; internal set; }

	}
}
