using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    internal class ChartFile
    {
        public int resolution = 480;
        public List<Section> sections = new();
        public List<Tempo> tempoMap = new();
        public float? consistentBPM = 120f;

        public long Whole => resolution * 4;
        public long Half => resolution * 2;
        public long Quarter => resolution;
        public long Eighth => resolution / 2;
        public long Twelfth => resolution / 3;
        public long Sixteenth => resolution / 4;
        public long TwentyFourth => resolution / 6;
        public long ThirtySecond => resolution / 8;
        public long FourtyEighth => resolution / 12;
        public long SixtyFourth => resolution / 16;
        public long NinetySixth => resolution / 24;
        public long OneTwentyEighth => resolution / 32;
        public long OneNinetySecond => resolution / 48;

        string fullText = string.Empty;

        public ChartFile() { }

        public ChartFile(string fullText, int resolution, List<Section> sections, List<Tempo> tempoMap)
        {
            this.fullText = fullText;
            this.resolution = resolution;
            this.sections = sections.ConvertAll(section => new Section(section.sectionName, section.notes));
            this.tempoMap = tempoMap;
            consistentBPM = tempoMap.Max(x => x.BeatsPerMinute) - tempoMap.Min(x => x.BeatsPerMinute) < 20 ?
                tempoMap.Average(x => x.BeatsPerMinute) : null;
        }

        /// <summary>
        /// Reads a chart file at the given file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ChartFile Read(string path)
        {
            string fullText = File.ReadAllText(path);
            int resolution = 480;
            List<Section> sections = new List<Section>();
            List<Note> notes = new();
            List<Tempo> tempoMap = new();

            string sectionName = string.Empty;

            bool readingSection = false;

            StreamReader reader = File.OpenText(path);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim() ?? string.Empty; // TODO: check if this trim is needed
                if (line.Length <= 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    sectionName = line.Substring(1, line.Length - 2);
                }
                else if (line == "{")
                {
                    readingSection = true;
                }
                else if (line == "}")
                {
                    readingSection = false;
           
                    sections.Add(new Section(sectionName, notes));

                    sectionName = string.Empty;
                    notes.Clear();
                }
                else
                {
                    if (readingSection)
                    {
                        if (line.IsNoteEvent()) 
                            notes.Add(line.ToNote());
                        else if (line.IsTempoEvent()) tempoMap.Add(line.ToTempo());
                        else if (line.ToLower().Contains("resolution"))
                        {
                            string resultString = Regex.Match(line, @"\d+").Value;
                            resolution = int.Parse(resultString);
                        }
                    }
                    else if (notes.Count > 0 && sectionName != string.Empty)
                    {
                        sections.Add(new Section(sectionName, notes));

                        sectionName = string.Empty;
                        notes.Clear();
                    }
                }
            }

            reader.Close();
;
            return new ChartFile(fullText, resolution, sections, tempoMap);
        }

        /// <summary>
        /// Writes the chart file to location specified by path.
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            Dictionary<string, Section> sectionMap = SectionMap();

            string sectionName = string.Empty;
            StringBuilder sb = new StringBuilder(string.Empty);
            bool readingSection = false;

            StreamReader reader = File.OpenText(path);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine() ?? string.Empty; // TODO: check if this trim is needed
                if (line.Length <= 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    sectionName = line.Substring(1, line.Length - 2);
                }
                else if (line == "{")
                {
                    readingSection = true;
                }
                else if (line == "}")
                {
                    readingSection = false;

                    if (sectionMap.ContainsKey(sectionName))
                    {
                        string oldStr = sb.ToString().Remove(sb.ToString().Length - 1, 1);
                        string newStr = new Section(sectionMap[sectionName].sectionName, sectionMap[sectionName].notes).SectionToString();
                        newStr = Regex.Replace(newStr, @"[\r\n]*^\s*$[\r\n]*", "", RegexOptions.Multiline);
                        
                        fullText = fullText.Replace(oldStr, newStr);
                    }

                    sectionName = string.Empty;
                    sb = sb.Clear();
                }
                else
                {
                    if (readingSection)
                    {
                        sb = sb.AppendLine(line);
                    }
                    else if (sb.Length > 0 && sectionName != string.Empty)
                    {
                        if (sectionMap.ContainsKey(sectionName))
                        {
                            string oldStr = sb.ToString().Remove(sb.ToString().Length - 1, 1);
                            string newStr = new Section(sectionMap[sectionName].sectionName, sectionMap[sectionName].notes).SectionToString();
                            newStr = Regex.Replace(newStr, @"[\r\n]*^\s*$[\r\n]*", "", RegexOptions.Multiline);
                            fullText = fullText.Replace(oldStr, newStr);
                        }

                        sectionName = string.Empty;
                        sb = sb.Clear();
                    }
                }
            }

            reader.Close();

            File.WriteAllText(path, fullText);
        }

        /// <summary>
        /// Returns a dictionary of section names and section objects.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Section> SectionMap()
        {
            Dictionary<string, Section> sectionMap = new Dictionary<string, Section>();

            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].sectionName.ToLower() != "song" &&
                    sections[i].sectionName.ToLower() != "synctrack" &&
                    sections[i].sectionName.ToLower() != "events")
                        sectionMap.Add(sections[i].sectionName, new Section(sections[i].sectionName, sections[i].notes));
            }

            return sectionMap;
        }
    }
}
