using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Chord : Sign
	{
		public List<Note> notes = new List<Note>();

		//public double left;
		//public double right;

		public void Add(Note note)
		{
			note.isPartOfChord = true;
			notes.Add(note);
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			double chordLeft = left;
			double chordRight = left;
			double cursor = left;

			// Wyszukaj najwyższą nutę na pięciolinii wiolinowej.
			var trebleNotes = notes.FindAll(note => note.staffType == Staff.Type.Treble);
			trebleNotes.Sort();
			Note trebleHighestNote = trebleNotes.LastOrDefault();
			Note trebleLowestNote = trebleNotes.FirstOrDefault();

			// Wyszukaj najniższa nutę na pięciolinii basowej.
			var bassNotes = notes.FindAll(note => note.staffType == Staff.Type.Bass);
			bassNotes.Sort();
			Note bassHighestNote = bassNotes.LastOrDefault();
			Note bassLowestNote = bassNotes.FirstOrDefault();

			// Narysuj wszystkie nuty akordu.
			foreach (var note in notes)
			{
				// Linie dodane rysuj tylko dla najwyzszej i najniższej nuty.
				note.showLegerLines = 
					note == trebleHighestNote || 
					note == trebleLowestNote ||
					note == bassHighestNote ||
					note == bassLowestNote;
				Staff staff
					= note.staffType == Staff.Type.Treble
					? trebleStaff
					: bassStaff;
				double noteCursor = note.AddToScore(score, trebleStaff, bassStaff, left);
				if (noteCursor == -1)
					return -1;
				if (noteCursor > cursor)
					cursor = noteCursor;
				if (note.right > chordRight || note.right == -1)
					chordRight = note.right;
				// Rozszerz obszar akrodru o obszar nuty.
				base.ExtendBounds(note.bounds);
			}

			// Dodaj prostokąt do focusowania akordu.
			base.AddHighlightRectangle(score, trebleStaff, bassStaff, 101);

			//this.left = chordLeft;
			//this.right = chordRight;

			return cursor;
		}

		public override void RemoveFromScore(Score score)
		{
			base.RemoveFromScore(score);
			foreach (Note note in notes)
			{
				note.RemoveFromScore(score);
			}
		}

		//public override bool IsShown
		//{
		//	get
		//	{
		//		return notes.Find(note => note.IsShown) != null;
		//	}
		//}

		public override void KeyDown(Key key)
		{
			// dopiero zaczęte
			foreach (var note in notes.FindAll(n => n.Equals(key.note)))
				note.MarkAsHit();
		}

		public override void KeyUp(Key key)
		{
			// dopiero zaczęte
			foreach (var note in notes.FindAll(n => n.Equals(key.note)))
				note.MarkAsHit();
		}

	}
}
