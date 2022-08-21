﻿using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;
using Console = SustainFixer.Debug;

namespace SustainFixer.Midi
{
    public enum Difficulty
    {
        Expert = 96,
        Hard = 84,
        Medium = 72,
        Easy = 60
    }

    internal class MidiEditor
    {
        public static void ProcessMidFile(string path)
        {
            Console.WriteLine($"Processing {path}...", ConsoleColor.Green);

            // cache midi file
            MidiFile midi = MidiFile.Read(path);
            TempoMap tempoMap = midi.GetTempoMap();
            int ticksPer192ndNote = ((TicksPerQuarterNoteTimeDivision)midi.TimeDivision).ToInt16() / 48;

            float? bpm = tempoMap.ConsistentBPM();
            
            var trackChunks = midi.GetTrackChunks();

            for (int i = 1; i < trackChunks.Count(); i++)
            {
                ProcessTrackChunk(trackChunks.ElementAt(i), tempoMap, ticksPer192ndNote, bpm);
            }

            midi.Write(path, true);
        }

        /// <summary>
        /// Processes a given track of a midi file.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="midi"></param>
        static void ProcessTrackChunk(TrackChunk track, TempoMap tempoMap, int ticksPer192ndNote, float? bpm)
        {
            string trackName = string.Empty;

            try
            {
                trackName = ((SequenceTrackNameEvent)track.GetTimedEvents().ElementAt(0).Event).Text;
                //Console.WriteLine($"Processing Track: {trackName}");
            }
            catch
            {
                Console.WriteLine(
                    "Something is wrong with the track name.",
                    ConsoleColor.Red);
                Console.ReadLine();
            }
            finally
            {
                if (trackName.ToLower() != "part vocals")
                {
                    // round every note to nearest 1/192 note
                    track.ProcessNotes(note => note.RoundNote(ticksPer192ndNote));

                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Expert, bpm);
                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Hard, bpm);
                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Medium, bpm);
                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Easy, bpm);
                }
            }
        }

        /// <summary>
        /// Process all of the notes of a track charted for a given difficulty.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="midi"></param>
        /// <param name="difficulty"></param>
        static void ProcessTrackDifficulty(TrackChunk track, TempoMap tempoMap, Difficulty difficulty, float? bpm)
        {
            // cache note positions
            List<long> notePositions = track.GetNotes().Where(note => 
                note.IsOnDifficultyChart(difficulty)).ToList().GetNotePositions();

            // shorten note based on BPM
            track.ProcessNotes(note =>
            {
                if (note.Length > 1
                && note.IsOnDifficultyChart(difficulty)
                && notePositions.ContainsElementWithinRange(note.EndTime, MusicalTimeSpan.OneNinetySecond, tempoMap))
                {
                    MusicalTimeSpan shortenAmt;

                    if (bpm != null)
                    {
                        shortenAmt = (bpm >= 140) ? MusicalTimeSpan.Sixteenth :
                            (bpm >= 100) ? MusicalTimeSpan.TwentyFourth :
                            MusicalTimeSpan.ThirtySecond;
                    }
                    else
                    {
                        Tempo tempo = tempoMap.GetTempoAtTime(note.TimeAs<ITimeSpan>(tempoMap));
                        shortenAmt = (tempo.BeatsPerMinute >= 140) ? MusicalTimeSpan.Sixteenth :
                            (tempo.BeatsPerMinute >= 100) ? MusicalTimeSpan.TwentyFourth :
                            MusicalTimeSpan.ThirtySecond;
                    }

                    note.ShortenNote(shortenAmt, tempoMap);
                    //note.ShortenNote(MusicalTimeSpan.ThirtySecond, tempoMap);
                    // TODO: check for performance hit
                }
            });
        }
    }
}
