using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    public class Section
    {
        public string sectionName;
        public List<Note> notes = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="notes"></param>
        public Section(string sectionName, List<Note> notes)
        {
            this.sectionName = sectionName;
            this.notes = notes.ConvertAll(note => new Note(note.Time, note.Fret, note.Length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Section Clone()
        {
            Section section = new Section(sectionName, notes);

            return new Section(sectionName, notes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public int ProcessNotes(Action<Note> action)
        {
            int notesProcessed = 0;

            foreach (var note in notes)
            {
                action(note);
                notesProcessed++;
            }

            return notesProcessed;
        }
    }
}
