﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    /// <summary>
    /// A note from a chart.
    /// </summary>
    public class Note
    {
        public long Time { get; set; }
        public short Fret { get; set; }
        public long Length { get; set; }
        public bool IsSustained => Length != 0;
        public long EndTime => Time + Length;

        #region Constructor

        public Note(long time, short fret, long length)
        {
            Time = time;
            Fret = fret;
            Length = length;
        }

        #endregion
    }
}
