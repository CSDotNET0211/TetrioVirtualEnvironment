﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrReplayLoaderLib;

namespace TetrioVirtualEnvironment
{
    public class GameData
    {
        public GameData(object replaydata,bool isMulti)
        {
            if(isMulti)
            {

            }
            else
            {
                ReplayDataSolo data=(ReplayDataSolo)replaydata;


            }


            Instance=this;
        }
        static public GameData Instance;

        public int[] Field;
       public Options Options;
      public double SubFrame;
        public bool LShift;
        public bool RShift;
        public int LastShift;
       public double LDas;
       public double RDas;
     public   PlayerOptions Handling;
        public double LDasIter;
        public double RDasIter;
        public bool SoftDrop;
        public Falling Falling;
        public bool HoldLocked;
        public double Gravity;
        public int FallingRotations;
        public int TotalRotations;
       
    }
}
