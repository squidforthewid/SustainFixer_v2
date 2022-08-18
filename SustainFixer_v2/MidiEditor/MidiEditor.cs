using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Console = SustainFixer.Debug;

namespace SustainFixer.Midi
{
    internal class MidiEditor
    {
        public enum Difficulty
        {
            Expert = 96,
            Hard = 84,
            Medium = 72,
            Easy = 60
        }

        public static void ProcessMidFile(string path)
        {
            Console.WriteLine($"Processing {path}...", ConsoleColor.Green);

            // cache midi file
            MidiFile midi = MidiFile.Read(path);
            TempoMap tempoMap = midi.GetTempoMap();
            int ticksPer192ndNote = ((TicksPerQuarterNoteTimeDivision)midi.TimeDivision).ToInt16() / 48;
            
            var trackChunks = midi.GetTrackChunks();

            for (int i = 1; i < trackChunks.Count(); i++)
            {
                ProcessTrackChunk(trackChunks.ElementAt(i), tempoMap, ticksPer192ndNote);
            }

            midi.Write(path, true);
        }

        /// <summary>
        /// Processes a given track of a midi file.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="midi"></param>
        static void ProcessTrackChunk(TrackChunk track, TempoMap tempoMap, int ticksPer192ndNote)
        {
            string trackName = "";

            try
            {
                trackName = ((SequenceTrackNameEvent)track.GetTimedEvents().ElementAt(0).Event).Text;
                Console.WriteLine($"Processing Track: {trackName}");
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
                    track.ProcessNotes(note => RoundNote(note, ticksPer192ndNote));

                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Expert);
                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Hard);
                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Medium);
                    ProcessTrackDifficulty(track, tempoMap, Difficulty.Easy);
                }
            }
        }

        /// <summary>
        /// Process all of the notes of a track charted for a given difficulty.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="midi"></param>
        /// <param name="difficulty"></param>
        static void ProcessTrackDifficulty(TrackChunk track, TempoMap tempoMap, Difficulty difficulty)
        {
            // cache note positions
            List<long> notePositions = new List<long>();
            foreach (Note note in track.GetNotes().Where(note => IsOnDifficultyChart(note, difficulty)))
            {
                notePositions.Add(note.Time);
            }

            // shorten note by 1/32 note
            track.ProcessNotes(note =>
            {
                if (note.Length > 1
                && IsOnDifficultyChart(note, difficulty)
                && CheckForElementWithinRange(notePositions, note.EndTime, MusicalTimeSpan.SixtyFourth / 3, tempoMap))
                {
                    ShortenNote(note, MusicalTimeSpan.ThirtySecond, tempoMap);
                }
            });
        }

        // TODO: rename to something cleaner
        // TODO: move to ChartUtility/ChartEditorTools and convert to extension method 
        /// <summary>
        /// Rounds a note to the nearest multiple of the interval.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="interval"></param>
        static void RoundNote(Note note, int interval)
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

        // TODO: rename to something cleaner
        // TODO: move to ChartUtility/ChartEditorTools and convert to extension method 
        /// <summary>
        /// Shortens a note by the length provided.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="midi"></param>
        static void ShortenNote(Note note, MusicalTimeSpan shortenAmt, TempoMap tempoMap)
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

        // TODO: rename to something cleaner
        // TODO: move to ChartUtility/ChartEditorTools and convert to extension method 
        /// <summary>
        /// </summary>
        /// <param name="note"></param>
        /// <param name="diff"></param>
        /// <returns>whether a note is charted for a given difficulty.</returns>
        static bool IsOnDifficultyChart(Note note, Difficulty diff)
        {
            return note.NoteNumber <= (int)diff + 6 && note.NoteNumber >= (int)diff;
        }

        // TODO: rename to something cleaner
        // TODO: move to ChartUtility/ChartEditorTools and convert to extension method 
        /// <summary>
        /// Check if any element in a list of integers is within a given range of a given integer.
        /// </summary>
        /// <param name="notePositions"></param>
        /// <param name="noteEndTime"></param>
        /// <param name="range"></param>
        /// <param name="midi"></param>
        /// <param name="closeNote"></param>
        /// <returns></returns>
        static bool CheckForElementWithinRange(
            List<long> notePositions,
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
    }
}
