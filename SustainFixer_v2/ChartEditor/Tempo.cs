using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainFixer.Chart
{
    public class Tempo
    {
        public long Time { get; set; }
        public float BeatsPerMinute { get; set; }

        public Tempo(long time, float beatsPerMinute)
        {
            Time = time;
            BeatsPerMinute = beatsPerMinute;
        }
    }
}
