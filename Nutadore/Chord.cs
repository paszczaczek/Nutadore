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

        public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
        {
            double chordRight = left;
            foreach (var note in notes)
            {
                Staff staff
                    = note.staffType == Staff.Type.Treble
                    ? trebleStaff
                    : bassStaff;
                // double noteRight = staff.ShowSign(note, left); // TODO: sprawdzić czy to działa!
				double noteRight = note.Show(score, trebleStaff, bassStaff, left);
                if (noteRight > chordRight || noteRight == -1)
                    chordRight = noteRight;
            }
            return chordRight;
        }

        public override void Hide(Score score)
        {
            notes.ForEach(note => note.Hide(score));
        }

        public override bool IsShown
        {
            get
            {
                return notes.Find(note => note.IsShown) != null;
            }
        }
    }
}
