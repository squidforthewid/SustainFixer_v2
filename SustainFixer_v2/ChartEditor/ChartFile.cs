using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    /// <summary>
    /// Contains data from a .chart file.
    /// </summary>
    internal class ChartFile
    {
        public int resolution = 480;
        public List<Section> sections = new();
        public List<Tempo> tempoMap = new();
        public float? consistentBPM = 120f;

        string fullText = string.Empty;

        #region ReadOnly Properties

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

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ChartFile() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullText"></param>
        /// <param name="resolution"></param>
        /// <param name="sections"></param>
        /// <param name="tempoMap"></param>
        public ChartFile(string fullText, int resolution, List<Section> sections, List<Tempo> tempoMap)
        {
            this.fullText = fullText;
            this.resolution = resolution;
            this.sections = sections.ConvertAll(section => new Section(section.sectionName, section.notes));
            this.tempoMap = tempoMap;
            consistentBPM = tempoMap.Max(x => x.BeatsPerMinute) - tempoMap.Min(x => x.BeatsPerMinute) < 20 ?
                tempoMap.Average(x => x.BeatsPerMinute) : null;
        }

        #endregion

        #region Public functions

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

            StreamReader sr = File.OpenText(path);

            // read the chart file line by line
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine().Trim() ?? string.Empty;

                if (line.Length == 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']') // section header line
                {
                    sectionName = line.Substring(1, line.Length - 2);
                }
                else if (line == "{") // start of section
                {
                    readingSection = true;
                }
                else if (line == "}") // end of section
                {
                    readingSection = false;
           
                    // add gathered data to list of sections
                    sections.Add(new Section(sectionName, notes));

                    // clear vars for next section
                    sectionName = string.Empty;
                    notes.Clear();
                }
                else
                {
                    if (readingSection)
                    {
                        // convert and store line data accordingly
                        if (line.IsNoteEvent()) 
                            notes.Add(line.ToNote());
                        else if (line.IsTempoEvent())
                            tempoMap.Add(line.ToTempo());
                        else if (line.ToLower().Contains("resolution")) // get chart resolution
                        {
                            string resultString = Regex.Match(line, @"\d+").Value;
                            resolution = int.Parse(resultString);
                        }
                    }
                }
            }

            sr.Close();

            return new ChartFile(fullText, resolution, sections, tempoMap);
        }

        /// <summary>
        /// Writes the chart file to location specified by path.
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            // cache a dictionary of sections (which should be processed at this point) and their names
            Dictionary<string, Section> sectionMap = SectionMap();

            string sectionName = string.Empty;
            StringBuilder sb = new StringBuilder(string.Empty); // StringBuilder for building sections into strings
            bool readingSection = false;

            StreamReader reader = File.OpenText(path);

            // read chart line by line 
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim() ?? string.Empty;

                if (line.Length == 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']') // section header line
                {
                    sectionName = line.Substring(1, line.Length - 2);
                }
                else if (line == "{") // section start
                {
                    readingSection = true;
                }
                else if (line == "}") // section end
                {
                    readingSection = false;

                    // check dictionary for section name and make sure current section is not empty
                    if (sb.Length != 0 && sectionName != string.Empty && sectionMap.ContainsKey(sectionName))
                    {
                        // replace section we just went through with one of the processed sections with a matching name
                        string oldStr = sb.ToString().Remove(sb.ToString().Length - 1, 1);
                        string newStr = new Section(sectionMap[sectionName].sectionName, sectionMap[sectionName].notes).SectionToString();
                        newStr = Regex.Replace(newStr, @"[\r\n]*^\s*$[\r\n]*", "", RegexOptions.Multiline);

                        fullText = fullText.Replace(oldStr, newStr);
                    }

                    // clear vars for next section
                    sectionName = string.Empty;
                    sb = sb.Clear();
                }
                else
                {
                    if (readingSection) // note line
                    {
                        // leading double-whitespace so that when fullText.Replace is called at the end of a section (if line == "}")
                        // the old string matches the original fullText - since the sb trims each line to see what non-whitespace
                        // character it starts with. Not the cleanest solution, but it works for now.

                        //TODO: make this not shit 
                        sb = sb.AppendLine($"  {line}");
                    }
                }
            }

            reader.Close();

            // overwrite text
            File.WriteAllText(path, fullText);
        }

        #endregion

        #region Private functions

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

        #endregion
    }
}
