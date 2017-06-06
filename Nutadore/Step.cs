using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Step
	{
		#region props & types
		public List<Sign> voices = new List<Sign>();

		private List<Note> notGuessedNotes = new List<Note>();
		private Score score;
		private Staff trebleStaff;
		private Staff bassStaff;
		private double left;

		private static Brush currentBrush = Brushes.LightSeaGreen;
		private static Brush highlightBrush = Brushes.Gray;

		private bool isCurrent;
		public bool IsCurrent
		{
			get { return isCurrent; }
			set { isCurrent = value; SetColor(); }
		}

		public Perform.HowTo performHowToStaffTreble;
		public Perform.HowTo performHowToStaffBass;

		private bool isHighlighted;
		private Rectangle highlightRect;
		public Rect bounds { get; private set; } = Rect.Empty;
		#endregion

		public Step AddVoice(Sign voice)
		{
			voices.Add(voice);
			return this;
		}

		public bool IsBar
		{
			get
			{
				Sign firstVoice = voices.FirstOrDefault();
				return firstVoice != null && firstVoice is Bar;
			}
		}

		public double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			// Zapamietujemy, to bo będziemy chcieli dodawać nietrafione nuty.
			this.score = score;
			this.trebleStaff = trebleStaff;
			this.bassStaff = bassStaff;
			this.left = left;

			// Wyliczamy i korygujemy ottave górną i dolną dla całego kroku.
			CalculateAndCorrectPerformHowTo();

			double cursor = left;
			double right = left;

			// Przenieś główki nut drugą strone stem, jesli zachodzą na siebie.
			EliminateHeadsOverlapping();

			// Przesuń znaki chromatyczne, jeśli zachodza na siebie.
			EliminateAccidentalOverlapping();

			// Jeśli nuta w jednym głosie ma przypadkowy znak chromatyczny, nuty w pozostałych 
			// głosach trzeba przesunąć w prawo, by ich główki były w jednej linii.
			double noteHeadOffsetMax = .0;
			for (int phase = 0; phase <= 1; phase++)
			{
				// Dodajemy do score poszczególne głosy.
				foreach (Sign voice in voices)
				{
					INoteOffsets offs = voice as INoteOffsets;
					if (offs != null)
						offs.offset = phase == 0 ? .0 : noteHeadOffsetMax - offs.headOffset;
					double voiceCursor = voice.AddToScore(score, trebleStaff, bassStaff, this, left);
					if (offs != null)
						noteHeadOffsetMax = Math.Max(offs.headOffset, noteHeadOffsetMax);
					if (voiceCursor == -1)
					{
						// Jeden z głosów nie zmieścił się - wycofujemy pozostałe.
						RemoveFromScore(score);
						return -1;
					}
					if (voiceCursor > cursor)
						cursor = voiceCursor;
					if (voice.bounds.Right > right || voice.bounds.Right == -1)
						right = voice.bounds.Right;
				}
				if (phase == 0)
				{
					// Czy jakaś nuta w jakimś głosie miała przypadkowy znak chromatyczny?
					if (noteHeadOffsetMax > 0)
					{
						// Tak - wycofujemy wszystkie głosy i narysujemy je jeszcze raz z przesunięciem.
						foreach (Sign voice in voices)
							voice.RemoveFromScore(score);
						cursor = left;
						right = left;
					}
					else
					{
						// Nie.
						break;
					}
				}
			}

			// Dodajemy do score błędnie wciśnięte nuty.
			foreach (Note notGuessedNote in notGuessedNotes)
			{
				double noteCursor = notGuessedNote.AddToScore(score, trebleStaff, bassStaff, this, left);
				if (noteCursor == -1)
					return -1;
				if (noteCursor > cursor)
					cursor = noteCursor;
				if (notGuessedNote.bounds.Right > right || notGuessedNote.bounds.Right == -1)
					right = notGuessedNote.bounds.Right;
			}

			// Dodajemy prostokąt do reagujący na mysz.
			double top = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(6));
			double bottom = bassStaff.StaffPositionToY(StaffPosition.ByLegerBelow(4));
			highlightRect = new Rectangle
			{
				Width = right - left,
				Height = bottom - top,
				Margin = new Thickness(left, top, 0, 0),
				Fill = Brushes.Transparent,
				Stroke = Brushes.Transparent,
				Tag = score // potrzebne w event handlerze
			};
			highlightRect.MouseEnter += MouseEnter;
			highlightRect.MouseLeave += MouseLeave;
			highlightRect.MouseDown += MouseDown;
			score.Children.Add(highlightRect);
			Canvas.SetZIndex(highlightRect, 100);

			bounds = new Rect(left, top, right - left, bottom - top);

			return cursor;
		}

		private void EliminateHeadsOverlapping()
		{
			// Główki nut (w ramach akordu) zachodzące na siebie przenosim na drugą strone laseczki.
			foreach (Chord chord in SelectAll<Chord>())
			{
				// Podziel nuty leżące na liniach i pomiędzy liniami.
				List<Note> notesOnLine = chord.notes
					.Where(note =>
						!note.staffPosition.LineAbove
						/*&& note.accidental.type == Accidental.Type.None*/)
					.ToList();
				List<Note> notesAboveLine = chord.notes
					.Where(note =>
						note.staffPosition.LineAbove /*||
						note.accidental.type != Accidental.Type.None*/)
					.ToList();

				// Tych których jest mniej prznosimy na drugą stronę laseczki.
				List<Note> notesReversed;
				List<Note> notesNotReversed;
				if (notesOnLine.Count < notesAboveLine.Count)
				{
					notesReversed = notesOnLine;
					notesNotReversed = notesAboveLine;
				}
				else
				{
					notesReversed = notesAboveLine;
					notesNotReversed = notesOnLine;
				}
				notesReversed.ForEach(note => note.isHeadReversed = true);

				// Przenosimy z powrotem te, dla których znajdzie się miejsce po właściwej stronie laseczki.
				foreach (Note noteReversed in notesReversed)
				{
					bool canUndone = !notesNotReversed
						.Where(noteNotReversed =>
							noteNotReversed.staffType == noteReversed.staffType &&
							Math.Abs(noteNotReversed.staffPosition.Number - noteReversed.staffPosition.Number) < 1.0)
						.Any();
					if (canUndone)
						noteReversed.isHeadReversed = false;
				}

				// Dla, np c i c#, c# powinno byc przeniesione na druga strone laseczki.
				foreach (Note noteNotReversed in notesNotReversed)
				{
					notesNotReversed
						.Where(note => 
							note.staffPosition.LineName == noteNotReversed.staffPosition.LineName &&
							note.accidental.type != noteNotReversed.accidental.type)
						.Where(note => note.accidental.type != Accidental.Type.None)
						.ToList()
						.ForEach(note => note.isHeadReversed = true);
				}
			}

			// TODO: Zachodzić na siebi mogą równiez pojenyńcze nuty. Ich nie przenosi się na drugą
			// stronę kreseczki, tylko trzeba przesunąć. Tego na razie nie implementuje.
		}

		private void EliminateAccidentalOverlapping()
		{
			// Znaki chromatyczne zachodzące na siebie przesuwamy w lewą stronę.
			// Wybieramy nuty ze stepu posiadające znaki chromatyczne.
			var notes = SelectAllNotes()
				.Where(note => note.accidental.type != Accidental.Type.None)
				.OrderBy(note => note)
				.AsEnumerable();
			foreach (Note note in notes)
			{
				// Ile kolumn w lewo trzeba przesunąć znak chromatyczny, 
				// żeby nie kolidował z sąsiednimi znakami.
				note.accidentalColumn = 0;
				FindAccidentalOverlaping(note, 0, ref notes);
			}

			// Teraz żadne znaki chromnatyczne nie kolidują ze sobą, ale
			// algorytm jest trochę nadgorliwy i okazuje się że nie które
			// znaki mozna przenieść do kolumn w prawo. Zaczynając od znaków
			// najbardziej przesuniętych w lewo sprawdzamy, czy  nie można
			// ich umieścić bardziej na prawo.
			var notesOrdByAccidentalColumn = notes
				.OrderBy(note => note)
				.OrderByDescending(note => note.accidentalColumn);
			foreach (var note in notesOrdByAccidentalColumn)
			{
				note.accidentalColumn = 0;
				FindAccidentalOverlaping(note, 0, ref notes);
			}
		}

		private void FindAccidentalOverlaping(Note note, int col, ref IEnumerable<Note> notes)
		{
			// Sprawdzamy czy znak chromatyczny nie koliduje z innymi znakami w kolumnie col.
			var noteOverlapped = notes.Where(n =>
					n.accidentalColumn == col
					&& note.staffType == n.staffType
					&& Math.Abs(note.staffPosition.Number - n.staffPosition.Number) <= 3.0
					&& !note.Equals(n));
			if (noteOverlapped.Count() == 0)
				return;
			// Koliduje - przesuwamy znak chromatyczny do kolumny na lewo
			note.accidentalColumn++;
			// i sprawdzamy w niej nie ma kolizji. I tak do skutku, aż znajdziemy dla niego miejsce.
			FindAccidentalOverlaping(note, col + 1, ref notes);
		}

		public void RemoveFromScore(Score score)
		{
			foreach (Sign sign in voices)
				sign.RemoveFromScore(score);

			foreach (Note notGuessedNote in notGuessedNotes)
				notGuessedNote.RemoveFromScore(score);

			score.Children.Remove(highlightRect);
			highlightRect = null;
		}

		private void SetColor()
		{
			if (isCurrent && isHighlighted)
			{
				highlightRect.Fill = currentBrush;
				highlightRect.Stroke = currentBrush;
				highlightRect.Opacity = 0.3;
			}
			else if (isCurrent && !isHighlighted)
			{
				highlightRect.Fill = currentBrush;
				highlightRect.Stroke = currentBrush;
				highlightRect.Opacity = 0.2;

			}
			else if (!isCurrent && isHighlighted)
			{
				highlightRect.Fill = highlightBrush;
				highlightRect.Stroke = highlightBrush;
				highlightRect.Opacity = 0.1;
			}
			else if (!isCurrent && !isHighlighted)
			{
				highlightRect.Fill = Brushes.Transparent;
				highlightRect.Stroke = Brushes.Transparent;
			}
		}

		private void CalculateAndCorrectPerformHowTo()
		{
			// Wyszukujemy wszystkie nuty w kroku.
			List<Note> stepNotes = SelectAllNotes();

			// Dodajemy również nuty błędnie wciśniętych klawiszy.
			stepNotes.AddRange(notGuessedNotes);

			// Wyszukaj wszystkie nuty akordu leżące na pięcilinii wilonowej.
			List<Note> trebleNotes = stepNotes.FindAll(note => note.staffType == Staff.Type.Treble);

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
			List<Note> bassNotes = stepNotes.FindAll(note => note.staffType == Staff.Type.Bass);

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

		public List<T> SelectAll<T>() where T : class
		{
			return voices
				.Where(voice => voice is T)
				.Select(voice => voice as T)
				.ToList<T>();
		}

		public List<Note> SelectAllNotes()
		{
			return voices
				.Where(voice => voice is Chord)
				.SelectMany(voice => (voice as Chord).notes)
				.Union(
					voices
						.Where(voice => voice is Note)
						.Select(voice => voice as Note)
				).ToList();
			/*
			List<Note> notes = new List<Note>();
			foreach (Sign voice in voices)
			{
				if (voice is Chord)
				{
					Chord chord = voice as Chord;
					notes.AddRange(chord.notes);
				}
				else if (voice is Note)
				{
					Note note = voice as Note;
					notes.Add(note);
				}
			}

			return notes;
			*/
		}

		public void Highlight(bool highlight)
		{
			isHighlighted = highlight;
			SetColor();
		}

		public void MouseEnter(object sender, MouseEventArgs e)
		{
			isHighlighted = true;
			SetColor();

			Score score = (sender as Rectangle).Tag as Score;
			score.FireEvent(SelectAllNotes(), ScoreEventArgs.EventType.HighlightedOn);
		}

		public void MouseLeave(object sender, MouseEventArgs e)
		{
			isHighlighted = false;
			SetColor();

			Score score = (sender as Rectangle).Tag as Score;
			score.FireEvent(SelectAllNotes(), ScoreEventArgs.EventType.HighlightedOff);
		}

		public void MouseDown(object sender, MouseButtonEventArgs e)
		{
			Score score = (sender as Rectangle).Tag as Score;
			score.CurrentStep = this;

			score.FireEvent(SelectAllNotes(), ScoreEventArgs.EventType.Selected);
		}

		public void KeyDown(Note noteDown)
		{
			// Czy trafiono wciśnięto właściwy klawisz?
			Note note = SelectAllNotes().Find(n => n.Equals(noteDown));
			if (note != null)
			{
				// Tak, zaznaczmy nutę na zielono.
				note.Guessed = true;
			}
			else
			{
				// Nie, dodajemy czerwoną nutę.
				note = new Note(noteDown.letter, noteDown.accidental.type, noteDown.octave);
				note.AddToScore(score, trebleStaff, bassStaff, this, left);
				note.Guessed = false;
				notGuessedNotes.Add(note);
				// TODO: dodana czerwona nuta może wymagać zmiany Perform.HowTo
			}
		}

		public void KeyUp(Note noteDown)
		{
			// Czy trafiono we właściwy klawisz?
			Note note = SelectAllNotes().Find(n => n.Equals(noteDown));
			if (note != null)
			{
				// Tak, zmieniamy zielony kolor na czarny.
				note.Guessed = null;
			}
			else
			{
				// Nie, usuwamy czerwoną nutę.
				note = notGuessedNotes.Find(n => n.Equals(noteDown));
				if (note == null)
					return;
				note.RemoveFromScore(score);
				notGuessedNotes.Remove(note);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			voices.ForEach(voice => sb.Append(voice.ToString()));
			return sb.ToString();
		}
	}
}
