using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainFixer.Chart
{
    public class Note
    {
        public enum Fret
        {
            Green, // 0
            Red, // 1
            Yellow, // 2
            Blue, // 3
            Orange, // 4
            Forced, // 5
            Tap, // 6
            Open // 7
        }

        public long Time { get; set; }
        public long Length { get; set; }
        public bool IsSustained
        {
            get { return Length != 0; }
        }
        public long EndTime
        {
            get { return Time + Length; }
        }
        public Fret fret;

        public Note(long time, Fret fret, long length)
        {
            Time = time;
            this.fret = fret;
            Length = length;
        }
    }
}
