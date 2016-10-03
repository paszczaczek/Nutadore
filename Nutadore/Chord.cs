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

		public double left;

		public double right;

		public Perform.HowTo performHowToStaffTreble;
		public Perform.HowTo performHowToStaffBass;

		public void Add(Note note)
		{
			// Nuta bedzie częścią akordu i będzie rysowana troszką inaczej niż nuta zwykła.
			focusable = true;
			notes.Add(note);
		}

		public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			CalculateAndCorrectPerformHowTo();

			double chordLeft = left;
			double chordRight = left;
			double cursor = left;

			// Wyszukaj najwyższą nutę na pięciolinii wiolinowej.
			var trebleNotes = notes.FindAll(note => note.staffType == Staff.Type.Treble);
			trebleNotes.Sort();
			Note trebleHighestNote = trebleNotes.LastOrDefault();

			// Wyszukaj najniższa nutę na pięciolinii basowej.
			var bassNotes = notes.FindAll(note => note.staffType == Staff.Type.Bass);
			bassNotes.Sort();
			Note bassLowestNote = bassNotes.FirstOrDefault();

			// Narysuj wszystkie nuty akordu.
			foreach (var note in notes)
			{
				// Linie dodane rysuj tylko dla najwyzszej i najniższej nuty.
				note.showLegerLines = note == trebleHighestNote || note == bassLowestNote;
				Staff staff
					= note.staffType == Staff.Type.Treble
					? trebleStaff
					: bassStaff;
				double noteCursor = note.Show(score, trebleStaff, bassStaff, left);
				if (noteCursor == -1)
					return -1;
				if (noteCursor > cursor)
					cursor = noteCursor;
				if (note.right > chordRight || note.right == -1)
					chordRight = note.right;
				// Rozszerz obszar akrodru o obszar nuty.
				base.ExtendBounds(score, note.Bounds, 101);
			}

			this.left = chordLeft;
			this.right = chordRight;

			return cursor;
		}

		public override void Hide(Score score)
		{
			base.Hide(score);
			notes.ForEach(note => note.Hide(score));
		}

		public override bool IsShown
		{
			get
			{
				return notes.Find(note => note.IsShown) != null;
			}
		}

		/// <summary>
		/// W akordzie nuty mogą być w różnych ottavach, a akord jako całość musi być w jednej.
		/// Ta funcja koryguje ottavy poszczególnych nut żeby akord był w jednej ottavie.
		/// </summary>
		public void CalculateAndCorrectPerformHowTo()
		{
			// Wyszukaj wszystkie nuty akordu leżące na pięcilinii wilonowej.
			var trebleNotes = notes.FindAll(note => note.staffType == Staff.Type.Treble);

			// Czy sa jakieś nuty wymagające zmiany wysokości wykonania na pieciolinii wiolinowej?
			if (trebleNotes.Any(note => note.performHowTo == Perform.HowTo.TwoOctaveHigher))
			{
				// Sa nuty wymagające zmiany wysokości wykonania o dwie oktawy wyżej.
				// Wyszukaj wszystkie nuty wymagające wykonania o jedną oktawę wyżej
				// i zmień je na wymagające wykonania o dwie oktawy wyżej.
				trebleNotes
					.FindAll(note => note.performHowTo == Perform.HowTo.OneOctaveHigher)
					.ForEach(note =>
					{
						note.performHowTo = Perform.HowTo.TwoOctaveHigher;
						note.staffPosition.Number -= 3.5;
					});

				// Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
				// wymagające wykonania o dwie oktawy wyżej.
				trebleNotes
					.FindAll(note => note.performHowTo == Perform.HowTo.AtPlace)
					.ForEach(note =>
					{
						note.performHowTo = Perform.HowTo.TwoOctaveHigher;
						note.staffPosition.Number -= 3.5 * 2;
					});

				// Teraz wszystkie nuty akordu leżące na pięciolinii wiolinowej
				// będą wykonywane o dwie oktawy wyzej.
				performHowToStaffTreble = Perform.HowTo.TwoOctaveHigher;
			}
			else if (trebleNotes.Any(note => note.performHowTo == Perform.HowTo.OneOctaveHigher))
			{
				// Są nuty wymagające zmiany wysokści wykonania o oktawę wyżej.
				// Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
				// wymagające wykonania o jedną oktawę wyżej.
				trebleNotes
					.FindAll(note => note.performHowTo == Perform.HowTo.AtPlace)
					.ForEach(note =>
					{
						note.performHowTo = Perform.HowTo.OneOctaveHigher;
						note.staffPosition.Number -= 3.5; // 0.0; // To ma tak byc!
					});

				// Teraz wszyskie nuty akordu leżące na pęciolinii wiolinowej będą
				// wykonywane o oktawę wyżej.
				performHowToStaffTreble = Perform.HowTo.OneOctaveHigher;
			}

			// Wyszukaj wszystkie nuty akordu leżące na pięcilinii basowej.
			var bassNotes = notes.FindAll(note => note.staffType == Staff.Type.Bass);

			// Czy sa jakieś nuty wymagające zmiany wysokości wykonania?
			if (bassNotes.Any(note => note.performHowTo == Perform.HowTo.OneOctaveLower))
			{
				// Jest przynajmniej jedna nuta wymagająca wykonania o oktawę niżej.
				// Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
				// wymagające wykonania o jedną oktawę niżej.
				bassNotes
					.FindAll(note => note.performHowTo == Perform.HowTo.AtPlace)
					.ForEach(note =>
					{
						note.performHowTo = Perform.HowTo.OneOctaveLower;
						note.staffPosition.Number += 3.5;
					});

				// Teraz wszystkie nuty leżące na pięciolinii basowej
				// będą wykonywane o oktawę nizej.
				performHowToStaffBass = Perform.HowTo.OneOctaveLower;
			}
		}

		public override void Bounds_MouseLeave(object sender, MouseEventArgs e)
		{
			//if ((System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.LeftShift) & KeyStates.Down) == 0)
			foreach (var note in notes)
				note.Bounds_MouseLeave(sender, e);
		}

		public override void Bounds_MouseEnter(object sender, MouseEventArgs e)
		{
			foreach (var note in notes)
				note.Bounds_MouseEnter(sender, e);
		}
	}
}
