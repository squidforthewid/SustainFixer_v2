using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainFixer.Chart
{
    public class Section
    {
        public string sectionName;
        public List<Note> notes = new();

        public Section(string sectionName, List<Note> notes)
        {
            this.sectionName = sectionName;
            this.notes = notes.ConvertAll(note => new Note(note.Time, note.Fret, note.Length));
        }

        public Section Clone()
        {
            Section section = new Section(sectionName, notes);

            return new Section(sectionName, notes);
        }

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
