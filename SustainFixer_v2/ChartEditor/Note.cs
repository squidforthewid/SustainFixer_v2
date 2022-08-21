using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainFixer.Chart
{
    public class Note
    {
        public long Time { get; set; }
        public short Fret { get; set; }
        public long Length { get; set; }
        public bool IsSustained
        {
            get { return Length != 0; }
        }
        public long EndTime
        {
            get { return Time + Length; }
        }

        public Note(long time, short fret, long length)
        {
            Time = time;
            Fret = fret;
            Length = length;
        }
    }
}
