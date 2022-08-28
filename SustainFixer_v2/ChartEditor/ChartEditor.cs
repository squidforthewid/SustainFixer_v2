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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="chart"></param>
        private static void ProcessSection(Section section, ChartFile chart)
        {
            if (section.sectionName.ToLower() != "song" &&
                section.sectionName.ToLower() != "synctrack" &&
                section.sectionName.ToLower() != "events")
            {
                section.ProcessNotes(note => note.RoundNoteToNearest(chart.OneNinetySecond));

                // cache note positions
                List<long> notePositions = section.notes.GetNotePositions();

                // shorten note based on BPM
                section.ProcessNotes(note =>
                {
                    if (note.Length > 1
                    && notePositions.ContainsElementWithinRange(note.EndTime, chart.OneTwentyEighth, out long nextNoteTime))
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

                        if (nextNoteTime != note.EndTime) note.Length = nextNoteTime - note.Time;

                        note.ShortenNote(shortenAmt);
                    }
                });
            }         
        }
    }
}
