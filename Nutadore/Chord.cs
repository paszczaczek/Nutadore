using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Chord : Sign, IDuration, INoteOffsets
	{
		public List<Note> notes = new List<Note>();

		private Duration _duration = new Duration();
		public Duration duration {
			get { return _duration; }
			set { notes.ForEach(note => note.duration = value); _duration = value; }
		}

		public double headOffset { get; private set; }

		private double _offset;
		public double offset
		{
			private get { return _offset; }
			set { notes.ForEach(note => note.offset = value + headOffset - note.headOffset); _offset = value; }
		}

		public void Add(Note note)
		{
			notes.Add(note);
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
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
			headOffset = 0;
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
				double noteCursor = note.AddToScore(score, trebleStaff, bassStaff, step, left);
				headOffset = Math.Max(headOffset, note.headOffset);
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

		public override string ToString()
		{
			StringBuilder sb =  new StringBuilder();
			sb.Append("<");
			foreach (Note note in notes)
			{
				if (sb.Length > 1)
					sb.Append(" ");
				sb.Append(note);
			}
			sb.Append(">");

			return sb.ToString();
		}
	}
}
