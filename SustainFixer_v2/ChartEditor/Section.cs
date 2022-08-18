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
        public List<Note> notes;

        public Section(string trackName, List<Note> notes)
        {
            this.sectionName = trackName;
            this.notes = notes;
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
