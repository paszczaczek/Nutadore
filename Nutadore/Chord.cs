using System.Collections.Generic;

namespace Nutadore
{
    public class Chord : Sign
    {
        public List<Note> notes = new List<Note>();

        public void Add(Note note)
        {
            notes.Add(note);
        }

        public double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
        {
            double chordRight = left;
            foreach (var note in notes)
            {
                Staff staff
                    = note.staffType == Staff.Type.Treble
                    ? trebleStaff
                    : bassStaff;
                double noteRight = staff.ShowSign(score, note, left);
                if (noteRight > chordRight || noteRight == -1)
                    chordRight = noteRight;
            }
            return chordRight;
        }

        public override void Hide()
        {
            notes.ForEach(note => note.Hide());
        }


        override public bool Shown
        {
            get
            {
                return notes.Find(note => note.Shown) != null;
            }
        }
    }
}
