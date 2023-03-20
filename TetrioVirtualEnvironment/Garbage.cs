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
        /// Interaction:            Arrived Garbage
        /// Interaction_Confirm:    
        /// </summary>
        public enum State
        {
            Interaction,
            Interaction_Confirm,
            Ready,
            Attack
        }

        public Garbage(int interaction_frame, int confirmed_frame, int sent_frame, int posX, int power, State state)
        {
            this.interaction_frame = interaction_frame;
            this.confirmed_frame = confirmed_frame;
            this.sent_frame = sent_frame;
            this.posX = posX;
            this.power = power;
            this.state = state;
        }

        public int confirmed_frame;
        public int interaction_frame;
        public int sent_frame;
        public int posX;
        public int power;
        public State state;

    }
}
