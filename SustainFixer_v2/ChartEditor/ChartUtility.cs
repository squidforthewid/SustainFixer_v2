﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    /// <summary>
    /// Class containing extension functions related to editing .chart files.
    /// </summary>
    static class ChartUtility
    {
        #region Note extensions

        /// <summary>
        /// Rounds the position and sustain length of a note to the nearest multiple of an interval.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="precisionInterval">Recommended 4 to snap to nearest 1/192.</param>
        /// <returns></returns>
        public static void RoundNoteToNearest(this Note note, long interval)
        {
            // round time
            if (note.Time % interval != 0)
            {
                note.Time = (long)Math.Round((float)note.Time / interval) * interval;
            }

            // round length
            if (note.Length % interval != 0)
            {
                note.Length = (long)Math.Round((float)note.Length / interval) * interval;
            }
        }

        /// <summary>
        /// Shortens a note by the length provided.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="shortenAmt">Amount to shorten by.</param>
        public static void ShortenNote(this Note note, long shortenAmt)
        {
            if (note.Length >= shortenAmt)
            {
                note.Length -= shortenAmt;
            }
        }

        /// <summary>
        /// Converts a Note object to a standard .chart line, as a string.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public static string ToLine(this Note note)
        {
            return $"{note.Time} = N {note.Fret} {note.Length}";
        }

        #endregion

        #region string extensions

        /// <summary>
        /// Converts a standard .chart line into a Note object.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Note ToNote(this string line) //Example note event format: 5000 = N 1 0
        {
            string[] substrings = line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            long time = long.Parse(substrings[0]);
            short fret = short.Parse(substrings[3]);
            long length = long.Parse(substrings[4]);

            return new Note(time, fret, length);
        }

        /// <summary>
        /// Converts a standard .chart line into a Tempo object.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Tempo ToTempo(this string line)
        {
            string[] substrings = line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            long time = long.Parse(substrings[0]);
            float bpm = float.Parse(substrings[3]) / 1000f;

            return new Tempo(time, bpm);
        }

        /// <summary>
        /// Returns true if the line passed in is written in the correct note line format.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsNoteEvent(this string line) // Example note event format: 5000 = N 1 0
        {
            try
            {
                string[] substrings = line.Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                return
                    long.TryParse(substrings[0], out long x) &&
                    substrings[2] == "N" &&
                    short.TryParse(substrings[3], out short y) &&
                    (y == 0 || y == 1 || y == 2 || y == 3 || y == 4 || y == 7) &&
                    long.TryParse(substrings[4], out long z);
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the line passed in is written in the correct tempo line format.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsTempoEvent(this string line) //Example note event format: 5000 = B 120
        {
            try
            {
                string[] substrings = line.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                return
                    long.TryParse(substrings[0], out long x) &&
                    substrings[2] == "B" &&
                    long.TryParse(substrings[3], out long y);
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        #endregion

        #region List extensions

        /// <summary>
        /// Check if a list of integers contains any element within a given range of a given value.
        /// </summary>
        /// <param name="notePositions"></param>
        /// <param name="noteEndTime"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool ContainsElementWithinRange(
            this List<long> notePositions,
            long noteEndTime,
            long range,
            out long nextNoteTime)
        {
            foreach (var notePosition in notePositions)
            {
                if (Math.Abs(notePosition - noteEndTime) < range)
                {
                    nextNoteTime = notePosition;
                    return true;
                }
            }

            nextNoteTime = noteEndTime; // Placeholder value since if false, this var will never be accessed anyway.
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempoMap"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Tempo GetTempoAtTime(this List<Tempo> tempoMap, long time)
        {
            return tempoMap.Last(x => x.Time < time);
        }

        #endregion

        #region Tempo extensions

        /// <summary>
        /// Converts a Tempo object to a standard .chart line, as a string.
        /// </summary>
        /// <param name="tempo"></param>
        /// <returns></returns>
        public static string ToLine(this Tempo tempo)
        {
            return $"{tempo.Time} = B {tempo.BeatsPerMinute * 1000}";
        }

        #endregion

        #region Section extensions

        /// <summary>
        /// Converts a section into a string for .chart files.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string SectionToString(this Section section)
        {
            StringBuilder sb = new StringBuilder(string.Empty);

            foreach (Note note in section.notes)
            {
                sb = sb.AppendLine($"  {note.ToLine()}");
            }

            return sb.ToString();
        }

        #endregion
    }
}

