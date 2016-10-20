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
		public List<Sign> voices = new List<Sign>();

		private static Brush currentBrush = Brushes.LightSeaGreen;
		private static Brush highlightBrush = Brushes.Gray;

		private bool isCurrent;
		public bool IsCurrent
		{
			get { return isCurrent; }
			set { isCurrent = value;  SetColor(); }
		}

		public Perform.HowTo performHowToStaffTreble;
		public Perform.HowTo performHowToStaffBass;
		
		private bool isHighlighted;
		private Rectangle highlightRect;
		public Rect bounds { get; private set; } = Rect.Empty;

		public void AddVoice(Sign voice)
		{
			voices.Add(voice);
			if (voice is Chord)
			{
				Chord chord = voice as Chord;
				chord.step = this;
			}
			else if (voice is Note)
			{
				Note note = voice as Note;
				note.step = this;
			}
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
			// Wyliczamy i korygujemy ottave górną i dolną dla całego kroku.
			CalculateAndCorrectPerformHowTo();

			double cursor = left;
			double right = left;

			// Dodajemy do score poszczególne głosy.
			foreach (Sign voice in voices)
			{
				double voiceCursor = voice.AddToScore(score, trebleStaff, bassStaff, left);
				if (voiceCursor == -1)
					return -1;
				if (voiceCursor > cursor)
					cursor = voiceCursor;
				if (voice.bounds.Right > right || voice.bounds.Right == -1)
					right = voice.bounds.Right;
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
			highlightRect.MouseUp += MouseUp;
			score.Children.Add(highlightRect);
			Canvas.SetZIndex(highlightRect, 100);

			bounds = new Rect(left, top, right - left, bottom - top);

			return cursor;
		}

		public void RemoveFromScore(Score score)
		{
			foreach (Sign sign in voices)
				sign.RemoveFromScore(score);

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
			List<Note> stepNotes = new List<Note>();
			foreach (Sign voice in voices)
			{
				if (voice is Chord)
				{
					Chord chord = voice as Chord;
					stepNotes.AddRange(chord.notes);
				}
				else if (voice is Note)
				{
					Note note = voice as Note;
					stepNotes.Add(note);
				}
			}

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

		private List<Note> FindAllNotes()
		{
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
			score.FireEvent(FindAllNotes(), ScoreEventArgs.EventType.Enter);
		}

		public void MouseLeave(object sender, MouseEventArgs e)
		{
			isHighlighted = false;
			SetColor();

			Score score = (sender as Rectangle).Tag as Score;
			score.FireEvent(FindAllNotes(), ScoreEventArgs.EventType.Leave);
		}

		public void MouseDown(object sender, MouseButtonEventArgs e)
		{
			Score score = (sender as Rectangle).Tag as Score;
			score.CurrentStep = this;

			score.FireEvent(FindAllNotes(), ScoreEventArgs.EventType.Down);
		}

		public void MouseUp(object sender, MouseButtonEventArgs e)
		{
			Score score = (sender as Rectangle).Tag as Score;
			score.FireEvent(FindAllNotes(), ScoreEventArgs.EventType.Up);
		}

	}
}
