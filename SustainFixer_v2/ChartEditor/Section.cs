using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SustainFixer.Debug;

namespace SustainFixer.Chart
{
    /// <summary>
    /// A container for a section of Notes from a .chart file, usually distinquished by instrument and difficulty (i.e. ExpertSingle) .
    /// </summary>
    public class Section
    {
        public string sectionName;
        public List<Note> notes = new();

        #region Constructors

        public Section(string sectionName, List<Note> notes)
        {
            this.sectionName = sectionName;
            this.notes = notes.ConvertAll(note => new Note(note.Time, note.Fret, note.Length));
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Section Clone()
        {
            return new Section(sectionName, notes);
        }

        /// <summary>
        /// Performs the specified action on each <see cref="Note"/> in the <see cref="Section"/>.
        /// </summary>
        /// <param name="action">The action to perform on each <see cref="Note"/> contained in the
        /// <paramref name="trackChunk"/>.</param>
        /// <returns>Count of the processed notes.</returns>
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

        #endregion
    }
}
