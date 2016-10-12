using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Note : Sign, IComparable<Note>
	{
		public Letter letter;
		public Octave octave;
		public Accidental accidental;
		public Perform.HowTo performHowTo;
		public StaffPosition staffPosition = StaffPosition.ByLine(1);
		public Staff.Type staffType;

		/// <summary>
		/// Pozwala zablokowac rysowanie linii dodanych. Wykorzysytwane w rysowaniu akordów.
		/// </summary>
		public bool showLegerLines = true;

		public bool isPartOfChord = false;

		public double left;
		public double right; 

		public static readonly Note lowest = new Note(Letter.A, Accidental.None, Octave.SubContra);
		public static readonly Note highest = new Note(Letter.C, Accidental.None, Octave.FiveLined);

		private TextBlock head;

		public enum Letter {
			C,
			D,
			E,
			F,
			G,
			A,
			H
		}

		public enum Octave
		{
			SubContra,
			Contra,
			Great,
			Small,
			OneLined,
			TwoLined,
			ThreeLined,
			FourLined,
			FiveLined
		}

		public enum Accidental
		{
			None,
			Flat,
			Sharp,
			Natural
		}

		public Note(Letter letter, Accidental accidental, Octave octave, Staff.Type? preferredStaffType = null)
		{
			this.letter = letter;
			this.accidental = accidental;
			this.octave = octave;
			this.staffPosition = ToStaffPosition(preferredStaffType);
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			// Left i right będą potrzebne do rysowania znaków ottavy
			this.left = left;

			// Na której pięciolinii ma być umieszczona nuta?
			Staff staff
				= staffType == Staff.Type.Treble
				? trebleStaff
				: bassStaff;

			// Rysujemy główkę nuty.
			string glyphCode = "\x0056";
			double glyphTop
					= staff.top * score.Magnification
					 + (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			glyphTop -= 57.5 * score.Magnification;
			right = base.AddFetaGlyph(score, left, glyphTop, glyphCode, 1);
			head = base.elements.FindLast(e => true) as TextBlock;
			// Rysujemy linie dodane górne i dolne - jeśli nuta nie jest częścią akordu.
			if (showLegerLines)
				ShowLegerLines(score, trebleStaff, bassStaff, left);

			// Rysujemy pomocniczą nazwę nuty.
			double letterTop
					= staff.top * score.Magnification
					+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			letterTop -= 7 * score.Magnification;
			double letterLeft = left + 3 * score.Magnification;
			TextBlock letterTextBlock = new TextBlock
			{
				FontFamily = new FontFamily("Consolas"),
				FontSize = 12 * score.Magnification,
				//Content = this.letter.ToString(),
				Text = this.letter.ToString(),
				Foreground = Brushes.White,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(letterLeft, letterTop, 0, 0)
			};
			base.AddElement(score, letterTextBlock, 2);

			// Dodajemy prostokąt reagujący na mysz.
			if (!isPartOfChord)
				base.AddHighlightRectangle(score, trebleStaff, bassStaff, 100);

			// Czy znak zmieścił sie na pięcolinii?
			if (right >= score.ActualWidth - Staff.marginLeft)
			{
				// Nie zmieścił się - narysujemy ją na następnej pieciolinii.
				base.RemoveFromScore(score);

				return -1;
			}
			else
			{
				// Znak zmieścił sie na pięciolinii.
				//right += Staff.spaceBetweenSigns * score.Magnification;
				return right + Staff.spaceBetweenSigns * score.Magnification;
			}
		}

		public void ShowLegerLines(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			// Na której pięciolinii ma być umieszczona nuta?
			Staff staff
				= staffType == Staff.Type.Treble
				? trebleStaff
				: bassStaff;

			// Czy trzeba dorysować linie dodane?
			double legerLeft = left - (right - left) * 0.2;
			double legerRight = right + (right - left) * 0.2;
			if (this.staffPosition <= StaffPosition.ByLegerBelow(1))
			{
				// Tak, trzeba dorysować linie dodane dolne.
				for (var staffPosition = StaffPosition.ByLegerBelow(1);
					 staffPosition >= this.staffPosition;
					 staffPosition.SubstractLine(1))
				{
					double y = staff.StaffPositionToY(staffPosition);
					Line legerLine = new Line
					{
						X1 = legerLeft,
						X2 = legerRight,
						Y1 = y,
						Y2 = y,
						Stroke = Brushes.Black,
						StrokeThickness = 0.5
					};
					base.AddElement(score, legerLine);
				}
				right = legerRight;
			}
			else if (this.staffPosition >= StaffPosition.ByLegerAbove(1))
			{
				// Tak, trzeba dorysować linie dodane górne.
				for (var staffPosition = StaffPosition.ByLegerAbove(1);
					 staffPosition <= this.staffPosition;
					 staffPosition.AddLine(1))
				{
					double y = staff.StaffPositionToY(staffPosition);
					Line legerLine = new Line
					{
						X1 = legerLeft,
						X2 = legerRight,
						Y1 = y,
						Y2 = y,
						Stroke = Brushes.Black,
						StrokeThickness = 0.5
					};
					base.AddElement(score, legerLine);
				}
				right = legerRight;
			}
		}

		override public void RemoveFromScore(Score score)
		{
			base.RemoveFromScore(score);
		}

		public StaffPosition ToStaffPosition(Staff.Type? preferredStaffType, bool withPerform = true)
		{
			// na której linii leży dźwięk C z oktawy w której jest nuta
			double lineNumber = 0;
			switch (octave)
			{
				case Octave.SubContra:
					// Wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej.
					staffType = Staff.Type.Bass;
					lineNumber = -9.0;
					break;
				case Octave.Contra:
					// wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej
					staffType = Staff.Type.Bass;
					lineNumber = -5.5;
					break;
				case Octave.Great:
					// wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej
					staffType = Staff.Type.Bass;
					lineNumber = -2.0;
					break;
				case Octave.Small:
					// domyślnie wszystkie nuty z tej oktawy leżą na pięciolinii basowej
					// nuty od F włącznie mogą leżeć na pięciolini wiolinowej
					if (preferredStaffType == null ||
						preferredStaffType == Staff.Type.Bass || 
						preferredStaffType == Staff.Type.Treble && letter < Letter.F)
					{
						staffType = Staff.Type.Bass;
						lineNumber = 1.5;
					}
					else
					{
						staffType = Staff.Type.Treble;
						lineNumber = -4.5;
					}
					break;
				case Octave.OneLined:
					// domyślnie wszystkie nuty z tej oktawy leżą na pięciolinii wiolinowej
					// nuty do G włącznie mogą leżeć na pięciolini basowej
					if (preferredStaffType == null ||
						preferredStaffType == Staff.Type.Treble || 
						preferredStaffType == Staff.Type.Bass && letter > Letter.G)
					{
						staffType = Staff.Type.Treble;
						lineNumber = -1.0;
					}
					else
					{
						staffType = Staff.Type.Bass;
						lineNumber = 5.0;
					}
					break;
				case Octave.TwoLined:
					// wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
					staffType = Staff.Type.Treble;
					lineNumber = 2.5;
					break;
				case Octave.ThreeLined:
					// wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
					staffType = Staff.Type.Treble;
					lineNumber = 6.0;
					break;
				case Octave.FourLined:
					// wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
					staffType = Staff.Type.Treble;
					lineNumber = 9.5;
					break;
				case Octave.FiveLined:
					// wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
					staffType = Staff.Type.Treble;
					lineNumber = 13.0;
					break;
				default:
					throw new ArgumentOutOfRangeException("octave", octave, "Nieobsłużona oktawa.");
			}

			// Może nutę należy rysować w ottavie?
			performHowTo = Perform.HowTo.AtPlace;
			if (withPerform)
			{
				if (octave == Octave.FiveLined)
				{
					// Tak, na pięcionii wiolinowej rysujemy o dwie oktawy niżej.
					performHowTo = Perform.HowTo.TwoOctaveHigher;
					lineNumber -= 3.5 * 2;
				}
				// else if (octave == Octave.FourLined && letter >= Letter.D ||
				else if (octave == Octave.FourLined && letter >= Letter.C ||
					octave > Octave.FourLined)
				{
					// Tak, na pięcionii wiolinowej rysujemy o oktawę niżej.
					performHowTo = Perform.HowTo.OneOctaveHigher;
					lineNumber -= 3.5;
				}
				else if (octave == Octave.Contra && letter <= Letter.E ||
					octave < Octave.Contra)
				{
					// Tak, na pięciolinii basowej rysujemy o oktawę wyżej
					performHowTo = Perform.HowTo.OneOctaveLower;
					lineNumber += 3.5;
				}
			}

			// dodajemy przesunięcie względem dzięku C
			lineNumber += (double)letter / 2;

			return StaffPosition.ByNumber(lineNumber);
		}

		public Note Copy()
		{
			return new Note(letter, accidental, octave, staffType);
		}

		/// <summary>
		/// Zmienia wysokość nuty o zadaną liczbę półlinii.
		/// </summary>
		/// <param name="halfSpaceCount"></param>
		/// <returns></returns>
		public Note Transpose(int halfSpaceCount)
		{
			int newLetter = (int)letter + halfSpaceCount;
			if (newLetter >= 0)
			{
				octave += newLetter / 7;
				letter = (Letter)(newLetter % 7);
			}
			else
			{
				octave += newLetter / 7 - 1;
				letter = (Letter)newLetter + 7;
				while (letter < 0)
					letter += 7;
			}

			staffPosition = ToStaffPosition(staffType);

			return this;
		}

		public bool InChord(Sign sign)
		{
			if (sign is Chord)
			{
				Chord chord = sign as Chord;
				return chord.notes.Exists(note => note.Equals(this));
			}
			else if (sign is Note)
			{
				Note note = sign as Note;
				return note.Equals(this);
			}
			else
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			Note other = obj as Note;

			if (obj == null)
				return false;

			if (other == null)
				return false;

			if (this.letter == other.letter &&
				this.accidental == other.accidental &&
				this.octave == other.octave)
				return true;

			// TODO porównywanie z krzyżykami i bemolami !

			return false;
		}

		public override int GetHashCode()
		{
			return (int)octave * 10 + (int)letter;
		}

		public int CompareTo(Note other)
		{
			if (other == null)
				return 1;

			int hashCode = GetHashCode();
			int otherHashCode = other.GetHashCode();

			if (hashCode == otherHashCode)
				return 0;
			else if (hashCode > otherHashCode)
				return 1;
			else
				return -1;
		}

		public void MarkAsHit()
		{
			head.Foreground = Brushes.Green;
		}

		public void MarkAsMissed()
		{
			head.Foreground = Brushes.PaleVioletRed;
		}
	}
}
