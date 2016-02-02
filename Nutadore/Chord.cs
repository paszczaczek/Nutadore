using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
    public class Chord : Sign
    {
        private List<Note> notes = new List<Note>();

        public void Add(Note note)
        {
            notes.Add(note);
        }

        public override double Show(Score score, double left, double top)
        {
            double chordLeft = left;

            foreach (Note note in notes)
            {
                double noteTop
                    = top
                    + (4 - note.staffPosition.LineNumber) * Staff.spaceBetweenLines * score.Magnification;
                double noteLeft = note.Show(score, left, noteTop);
                if (noteLeft > chordLeft)
                    chordLeft = noteLeft;
            }

            return chordLeft;
        }
    }
}
