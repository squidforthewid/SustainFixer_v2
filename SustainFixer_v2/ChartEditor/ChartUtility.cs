using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainFixer.Chart // TODO: decide how much of this we're using/keeping...
{
    /// <summary>
    /// Class containing useful methods for manipulating and extracting information from a chart.
    /// </summary>
    static class ChartUtility
    {
        /// <summary>
        /// Rounds an integer to the nearest multiple of an interval.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static long RoundToNearest(this long i, int interval)
        {
            return (long)Math.Round((float)i / interval) * interval;
        }

        /// <summary>
        /// Rounds the position and sustain length of a note to the nearest multiple of an interval.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="precisionInterval">Recommended 4 to snap to nearest 1/192.</param>
        /// <returns></returns>
        public static void RoundNoteToNearest(this Note note, int interval)
        {
            if (note.Time % interval != 0)
            {
                note.Time = note.Time.RoundToNearest(interval);
            }

            if (note.Length % interval != 0)
            {
                note.Length = note.Length.RoundToNearest(interval);
            }
        }

        public static void ShortenNote(this Note note, long shortenAmt) // TEMP VAR (?) //
        {
            if (note.Length >= shortenAmt)
            {
                note.Length -= shortenAmt;
            }
        }

        /// <summary>
        /// Converts a standard .chart line into a Note object.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Note ToNote(this string line)
        {
            string[] substrings = line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            int position = int.Parse(substrings[0]);
            Note.Fret fret = (Note.Fret)Enum.ToObject(typeof(Note.Fret), int.Parse(substrings[3]));
            int length = int.Parse(substrings[4]);

            return new Note(position, fret, length);
        }

        /// <summary>
        /// Converts a Note object to a standard .chart line, as a string.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public static string ToLine(this Note note)
        {
            return $"{note.Time} = N {(int)note.fret} {note.Length}";
        }

        public static string ToString(this Section section)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Note note in section.notes)
            {
                sb.AppendLine(note.ToLine());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns true if the line passed in is written in the correct note line format.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsNoteEvent(this string line) //Example note event format: 5000 = N 1 0
        {
            string[] substrings = line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            return
                int.TryParse(substrings[0], out int x) &&
                substrings[2] == "N" &&
                int.TryParse(substrings[3], out int y) &&
                (y == 0 || y == 1 || y == 2 || y == 3 || y == 4 || y == 7) &&
                int.TryParse(substrings[4], out int z);
        }

        public static bool ContainsElementWithinRange(this List<long> notePositions, long noteEndTime, long range) // TEMP VAR (?) //
        {
            foreach (var notePosition in notePositions)
            {
                if (Math.Abs(notePosition - noteEndTime) < range)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a list of note positions.
        /// </summary>
        /// <returns></returns>
        public static List<long> GetNotePositions(this List<Note> notes)
        {
            List<long> notePositions = new List<long>();

            foreach (Note note in notes)
            {
                notePositions.Add(note.Time);
            }

            return notePositions;
        }
    }
}

