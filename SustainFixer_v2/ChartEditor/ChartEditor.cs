using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    internal class ChartEditor
    {
        public static void ProcessChartFile(string path)
        {
            // cache chart file
            ChartFile chart = ChartFile.Read(path);

            for (int i = 0; i < chart.sections.Count; i++)
            {
                ProcessSection(chart.sections[i], chart);
            }

            chart.Write(path);
        }


        private static void ProcessSection(Section section, ChartFile chart)
        {
            try
            {
                //Console.WriteLine($"Processing Section: {section.sectionName}");
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
                if (section.sectionName.ToLower() != "song" &&
                    section.sectionName.ToLower() != "synctrack" &&
                    section.sectionName.ToLower() != "events")
                {
                    int x = section.ProcessNotes(note => note.RoundNoteToNearest(chart.OneNinetySecond));

                    // cache note positions
                    List<long> notePositions = section.notes.GetNotePositions();

                    // shorten note based on BPM
                    section.ProcessNotes(note =>
                    {
                        if (note.Length > 1
                        && notePositions.ContainsElementWithinRange(note.EndTime, chart.OneNinetySecond))
                        {
                            long shortenAmt;

                            if (chart.consistentBPM != null)
                            {
                                shortenAmt = (chart.consistentBPM >= 140) ? chart.Sixteenth :
                                    (chart.consistentBPM >= 100) ? chart.TwentyFourth :
                                    chart.ThirtySecond;
                            }
                            else
                            {
                                Tempo tempo = chart.tempoMap.GetTempoAtTime(note.Time);

                                shortenAmt = (tempo.BeatsPerMinute >= 140) ? chart.Sixteenth :
                                    (tempo.BeatsPerMinute >= 100) ? chart.TwentyFourth :
                                    chart.ThirtySecond;
                            }

                            note.ShortenNote(shortenAmt);
                        }
                    });
                }
            }
        }
    }
}
