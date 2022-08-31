using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace SustainFixer.Midi
{
    public enum Difficulty
    {
        Expert = 96,
        Hard = 84,
        Medium = 72,
        Easy = 60
    }

    /// <summary>
    /// Class for processing changes in midi files.
    /// </summary>
    internal class MidiEditor
    {
        #region Public functions

        /// <summary>
        /// Processes a <see cref="MidiFile"/> found at the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path of the file to be processed.</param>
        public static void ProcessMidFile(string path)
        {
            // set the read settings to bypass irrelevant exceptions
            ReadingSettings settings = new ReadingSettings();
            settings.InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits;
            settings.NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore;
            settings.InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore; // TODO: currently we are ignoring this rule because it still
                                                                            // works in Clone Hero, but in the future we should check the end of
            // cache midi file                                              // the chunk for a missing EndOfTrackEvent, as the file cannot be 
            MidiFile midi = MidiFile.Read(path, settings);                  // opened by Moonscraper if it's missing.
            
            // cache data from midi file for convenient access
            TempoMap tempoMap = midi.GetTempoMap();
            int ticksPer192ndNote = ((TicksPerQuarterNoteTimeDivision)midi.TimeDivision).ToInt16() / 48;
            float? meanBPM = tempoMap.ConsistentBPM(20);            
            var trackChunks = midi.GetTrackChunks();

            // process all track chunks found in the midifile, except for the first, which is the Events track.
            for (int i = 1; i < trackChunks.Count(); i++)
            {
                ProcessTrackChunk(trackChunks.ElementAt(i), tempoMap, ticksPer192ndNote, meanBPM);
            }

            // overwrite the midi file
            midi.Write(path, true);           
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Processes a given track of a midi file.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="midi"></param>
        static void ProcessTrackChunk(TrackChunk track, TempoMap tempoMap, int ticksPer192ndNote, float? meanBPM)
        {
            // cache the track name - if this fails, it will be caught in the FileProcessor and be reported as a bad file.
            string trackName = ((SequenceTrackNameEvent)track.GetTimedEvents().First(x => 
                x.Event is SequenceTrackNameEvent).Event).Text;

            // process the track, unless it is the vocal track (which does not contain sustains in need of fixing)
            if (trackName.ToLower() != "part vocals")
            {
                // round every note in the track to nearest 1/192 note
                track.ProcessNotes(note => note.RoundNote(ticksPer192ndNote));

                // as all difficulties for an instrument are charted on a single track, we need to process each difficulty independently.
                ProcessTrackDifficulty(track, tempoMap, Difficulty.Expert, meanBPM);
                ProcessTrackDifficulty(track, tempoMap, Difficulty.Hard, meanBPM);
                ProcessTrackDifficulty(track, tempoMap, Difficulty.Medium, meanBPM);
                ProcessTrackDifficulty(track, tempoMap, Difficulty.Easy, meanBPM);
            }
        }

        /// <summary>
        /// Process all of the notes of a track charted for a given difficulty.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="midi"></param>
        /// <param name="difficulty"></param>
        static void ProcessTrackDifficulty(TrackChunk track, TempoMap tempoMap, Difficulty difficulty, float? meanBPM)
        {
            // cache note positions
            List<long> notePositions = track.GetNotes().Where(note => 
                note.IsOnDifficultyChart(difficulty)).ToList().GetNotePositions();

            // shorten note based on BPM
            track.ProcessNotes(note =>
            {
                if (note.Length > 1 // note is sustained
                && note.IsOnDifficultyChart(difficulty) // isolate the notes from the given difficulty
                && notePositions.ContainsElementWithinRange(note.EndTime, MusicalTimeSpan.OneTwentyEighth, tempoMap, // only shorten a note
                                                        out long nextNoteTime)) // if it ends within 1/128 note of when another begins
                {
                    MusicalTimeSpan shortenAmt;
                    double bpm;

                    // (meanBPM is null here if the song's bpm is inconsistent)
                    if (meanBPM != null)
                    {
                        // As the BPM is consistent, we can simply use the average BPM to calulate the ideal shorten amount.
                        bpm = (float)meanBPM;
                    }
                    else
                    {
                        // As the BPM is inconsistent, we need to get the BPM at the time of the note.
                        Tempo tempo = tempoMap.GetTempoAtTime(note.TimeAs<ITimeSpan>(tempoMap));
                        bpm = tempo.BeatsPerMinute;
                    }

                    // The faster the BPM, the more we shorten the note by. 
                    shortenAmt = (meanBPM >= 140) ? MusicalTimeSpan.Sixteenth :
                        (meanBPM >= 100) ? MusicalTimeSpan.TwentyFourth :
                        MusicalTimeSpan.ThirtySecond;

                    // round the end time of the sustain exactly to the time of the note it's close to
                    if (nextNoteTime != note.EndTime) note.Length = nextNoteTime - note.Time;

                    note.ShortenNote(shortenAmt, tempoMap);
                }
            });
        }

        #endregion
    }
}
