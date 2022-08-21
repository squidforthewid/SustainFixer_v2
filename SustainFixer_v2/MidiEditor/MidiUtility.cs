using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace SustainFixer.Midi
{
    public static class MidiUtility
    { 
        /// <summary>
        /// Rounds a note to the nearest multiple of the interval.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="interval"></param>
        public static void RoundNote(this Note note, int interval)
        {
            if (note.Time % interval != 0)
            {
                note.Time = (long)Math.Round((float)note.Time / interval) * interval;
            }

            if (note.Length % interval != 0)
            {
                note.Length = (long)Math.Round((float)note.Length / interval) * interval;
            }
        }
 
        /// <summary>
        /// Shortens a note by the length provided.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="midi"></param>
        public static void ShortenNote(this Note note, MusicalTimeSpan shortenAmt, TempoMap tempoMap)
        {
            // convert length to musical format
            MusicalTimeSpan musicalNoteLength = (MusicalTimeSpan)LengthConverter.ConvertTo(
                note.Length,
                TimeSpanType.Musical,
                note.Time,
                tempoMap);

            // shorten it by shortenAmt
            if (musicalNoteLength >= shortenAmt)
            {
                musicalNoteLength -= shortenAmt;
            }

            // convert musical note length back to a long and set note.Length equal to the new length.
            note.Length = LengthConverter.ConvertFrom(
                musicalNoteLength,
                note.Time,
                tempoMap);
        }

        /// <summary>
        /// </summary>
        /// <param name="note"></param>
        /// <param name="diff"></param>
        /// <returns>whether a note is charted for a given difficulty.</returns>
        public static bool IsOnDifficultyChart(this Note note, Difficulty diff)
        {
            return note.NoteNumber <= (int)diff + 6 && note.NoteNumber >= (int)diff;
        }

        /// <summary>
        /// Check if any element in a list of integers is within a given range of a given integer.
        /// </summary>
        /// <param name="notePositions"></param>
        /// <param name="noteEndTime"></param>
        /// <param name="range"></param>
        /// <param name="midi"></param>
        /// <param name="closeNote"></param>
        /// <returns></returns>
        public static bool ContainsElementWithinRange(
            this List<long> notePositions,
            long noteEndTime,
            MusicalTimeSpan range,
            TempoMap tempoMap)
        {
            long r = LengthConverter.ConvertFrom(range, noteEndTime, tempoMap);

            foreach (long notePosition in notePositions)
            {
                if (Math.Abs(notePosition - noteEndTime) < r)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<long> GetNotePositions(this List<Note> notes)
        {
            List<long> notePositions = new List<long>();

            foreach (Note note in notes)
            {
                notePositions.Add(note.Time);
            }

            return notePositions;
        }

        public static float? ConsistentBPM(this TempoMap tempoMap)
        {
            return (tempoMap.GetTempoChanges().Max(x => x.Value.BeatsPerMinute)
                - tempoMap.GetTempoChanges().Max(x => x.Value.BeatsPerMinute) < 20) ?
                (float)tempoMap.GetTempoChanges().Average(x => x.Value.BeatsPerMinute) : null;
        }
    }
}
