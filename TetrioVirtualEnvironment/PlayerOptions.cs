using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrioVirtualEnvironment
{
    public class PlayerOptions
    {
        public PlayerOptions(double arr,double das,double dcd,double sdf,double safelock,bool cancel)
        {
            Name="!name!";
            ARR=arr;
            DAS=das;
            DCD=dcd;
            SDF=sdf;
            SafeLock=safelock;
            Cancel=cancel;

        }

        public string Name;
        public double ARR;
        public double DAS;
        public double DCD;
        public double SDF;
        public double SafeLock;
        public bool Cancel;
        
    }
}
