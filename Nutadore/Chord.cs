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

		public Step step {
			set
			{
				notes.ForEach(note => note.step = value);
			}
		}

		public void Add(Note note)
		{
			//note.isPartOfChord = true;
			notes.Add(note);
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			double chordLeft = left;
			double chordRight = left;
			double cursor = left;

			// Wyszukaj najwyższą i najwyższą nutę na pięciolinii wiolinowej z liniami dodanymi.
			var trebleNotes = notes.FindAll(note => note.staffType == Staff.Type.Treble);
			trebleNotes.Sort();
			Note trebleHighestNote = trebleNotes.LastOrDefault(note => note.staffPosition >= StaffPosition.ByLegerAbove(1));
			Note trebleLowestNote = trebleNotes.FirstOrDefault(note => note.staffPosition <= StaffPosition.ByLegerBelow(1));

			// Wyszukaj najniższa i najwyższą nutę na pięciolinii basowej z liniami dodanymi.
			var bassNotes = notes.FindAll(note => note.staffType == Staff.Type.Bass);
			bassNotes.Sort();
			Note bassHighestNote = bassNotes.LastOrDefault(note => note.staffPosition >= StaffPosition.ByLegerAbove(1));
			Note bassLowestNote = bassNotes.FirstOrDefault(note => note.staffPosition <= StaffPosition.ByLegerBelow(1));

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
	}
}
