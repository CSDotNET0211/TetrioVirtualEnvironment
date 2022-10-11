﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class GameData
    {
        public GameData()
        {
            SubFrame=0;
        }

        public int[] Field;
       public Options Options;
      public  float SubFrame;
        public bool LShift;
        public bool RShift;
        public int LastShift;
       public float  LDas;
       public float  RDas;
     public   PlayerOptions Handling;
        public float LDasIter;
        public float RDasIter;
        public bool SoftDrop;
        public Falling Falling;
        public bool HoldLocked;
    }
}
