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
            Console.WriteLine($"Processing {path}...", ConsoleColor.Green);

            // cache chart file
            ChartFile chart = ChartFile.Read(path);
            int ticksPer192ndNote = 0; // TEMP VAL/VAR //

            List<Section> sections = chart.sections;

            for (int i = 0; i < sections.Count(); i++)
            {
                ProcessSection(sections[i], ticksPer192ndNote);
            }

            chart.Write(path);
        }


        private static void ProcessSection(Section section, int ticksPer192ndNote)
        {
            try
            {
                Console.WriteLine($"Processing Track: {section.sectionName}");
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
                if (section.sectionName.ToLower() != "") // TEMP VAL //
                {
                    section.ProcessNotes(note => note.RoundNoteToNearest(ticksPer192ndNote));

                    // cache note positions
                    List<long> notePositions = section.notes.GetNotePositions();

                    // shorten note  by 1/32 note
                    section.ProcessNotes(note =>
                    {
                        if (note.Length > 1
                        && notePositions.ContainsElementWithinRange(note.EndTime, 3)) // TEMP VAL //
                        {
                            note.ShortenNote(3); // TEMP VAL //
                        }
                    });
                }
            }
        }





        // TODO - get rid of this eventually
        static void RewriteChartFile(string path)
        {
            Console.WriteLine("Found .chart file", ConsoleColor.Red);
            string fullText = File.ReadAllText(path);

            ChartData cd = new ChartData(path);

            if (cd != null)
            {
                List<string> originalNoteLines = new List<string>();
                foreach (Note note in cd.notesInChartCache)
                {
                    originalNoteLines.Add(note.ToLine());
                }

                Note[] newNotes = cd.notesInChartCache; //create copy of notes in chart
                List<long> newNotePositions = newNotes.GetNotePositions(); //cache the positions of these notes in a list

                for (int i = 0; i < newNotes.Length; i++) //for each note in the newNotes array...
                {
                    if (newNotes[i].IsSustained && //if the note is sustained...
                        newNotePositions.Contains(newNotes[i].position + newNotes[i].sustain))
                    //...and another note exists at the position where its sustain ends...
                    {
                        newNotes[i].sustain -= Note.thirtySecond; //...shorten its sustain by 1/32 beat...

                        //...and replace ORIGINAL note string in the .chart file with the NEW note, converted to a string
                        fullText = fullText.Replace(originalNoteLines[i], newNotes[i].ToLine());
                    }
                }

                //overwrite the file with the new text
                File.WriteAllText(path, fullText);
            }
        }
    }
}
