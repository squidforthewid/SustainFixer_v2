using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    /// <summary>
    /// A tempo event.
    /// </summary>
    public class Tempo
    {
        public long Time { get; set; }
        public float BeatsPerMinute { get; set; }

        #region Constructors

        public Tempo(long time, float beatsPerMinute)
        {
            Time = time;
            BeatsPerMinute = beatsPerMinute;
        }

        #endregion
    }
}
