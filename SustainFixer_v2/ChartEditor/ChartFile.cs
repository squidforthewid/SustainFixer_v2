using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainFixer.Chart
{
    internal class ChartFile
    {
        public int resolution = 0;
        public List<Section> sections = new();

        public string fullText;

        public ChartFile(string fullText, List<Section> sections) // INCOMPLETE
        {
            this.fullText = fullText;
            this.sections = sections;
        }

        /// <summary>
        /// Reads a chart file at the given file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ChartFile Read(string path)
        {
            string fullText = File.ReadAllText(path);
            List<Section> sections = new();

            string sectionName = string.Empty;
            List<Note> notes = new();

            bool readingSection = false;

            StreamReader reader = File.OpenText(path);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim() ?? string.Empty;
                if (line.Length <= 0) 
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    sectionName = line.Substring(1, line.Length - 1);
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
                        if (line.IsNoteEvent()) notes.Add(line.ToNote());
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

            return new ChartFile(fullText, sections);
        }

        /// <summary>
        /// Writes the chart file to location specified by path.
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            Dictionary<string, Section> sectionMap = SectionMap();

            string newText = fullText;
            string sectionName = string.Empty;
            StringBuilder sb = new StringBuilder();

            bool readingSection = false;

            StreamReader reader = File.OpenText(path);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim() ?? string.Empty;
                if (line.Length <= 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    sectionName = line.Substring(1, line.Length - 1);
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
                        newText.Replace(sb.ToString(), sectionMap[sectionName].ToString());
                    }

                    sectionName = string.Empty;
                    sb.Clear();
                }
                else
                {
                    if (readingSection)
                    {
                        sb.AppendLine(line);
                    }
                    else if (sb.Length > 0 && sectionName != string.Empty)
                    {
                        if (sectionMap.ContainsKey(sectionName))
                        {
                            newText.Replace(sb.ToString(), sectionMap[sectionName].ToString());
                        }

                        sectionName = string.Empty;
                        sb.Clear();
                    }
                }
            }

            reader.Close();

            File.WriteAllText(path, newText);
        }

        private Dictionary<string, Section> SectionMap()
        {
            Dictionary<string, Section> sectionMap = new Dictionary<string, Section>();

            foreach (Section section in sections)
            {
                sectionMap.Add(section.sectionName, section);
            }

            return sectionMap;
        }
    }
}
