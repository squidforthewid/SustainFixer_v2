using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    public class Tempo
    {
        public long Time { get; set; }
        public float BeatsPerMinute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="beatsPerMinute"></param>
        public Tempo(long time, float beatsPerMinute)
        {
            Time = time;
            BeatsPerMinute = beatsPerMinute;
        }
    }
}
