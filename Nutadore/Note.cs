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
		private static Brush currentBrush = Brushes.DarkGray; // LightSeaGreen;
		private static Brush highlightBrush = Brushes.DarkGray;

		public Letter letter;
		public Octave octave;
		public Accidental.Type accidentalType;
		public Perform.HowTo performHowTo;
		public StaffPosition staffPosition = StaffPosition.ByLine(1);
		public Staff.Type staffType;

		private Step step;

		private bool? _guessed;
		public bool? Guessed
		{
			get { return _guessed; }
			set { _guessed = value; SetColor(); }
		}
		private bool highlighted;
		private Rectangle highlightRect;
		private bool HighlightingIsActive
		{
			get
			{
				return
					System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) ||
					System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
			}
		}

		public bool showLegerLines = true;
		public double right { get; private set; } 

		public static readonly Note lowest = new Note(Letter.A, Accidental.Type.None, Octave.SubContra);
		public static readonly Note highest = new Note(Letter.C, Accidental.Type.None, Octave.FiveLined);

		private Accidental accidental;
		private TextBlock head;
		public double headShift;

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

		public Note(Letter letter, Accidental.Type accidentalType, Octave octave, Staff.Type? preferredStaffType = null)
		{
			this.letter = letter;
			this.accidentalType = accidentalType;
			this.octave = octave;
			this.staffPosition = ToStaffPosition(preferredStaffType);
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			// Left i right będą potrzebne do rysowania znaków ottavy
			this.step = step;
			right = left;

			// Na której pięciolinii ma być umieszczona nuta?
			Staff staff
				= staffType == Staff.Type.Treble
				? trebleStaff
				: bassStaff;

			// Rysujemy znak chromatyczny.
			if (accidentalType != Accidental.Type.None)
			{
				accidental = new Accidental(Accidental.Type.Sharp, staffPosition, staffType);
				accidental.isKeySignatureHint = score.scale.AccidentalForLetter(letter) != Accidental.Type.None;
				base.ExtendBounds(accidental.bounds);
				right = accidental.AddToScore(score, trebleStaff, bassStaff, step, left);
				headShift = right - left;
			}

			// Rysujemy główkę nuty.
			//double noteLeft = right;
			string glyphCode = "\x0056";
			double glyphTop
					= staff.top * score.Magnification
					 + (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			glyphTop -= 57.5 * score.Magnification;
			right = base.AddGlyphToScore(score, left + headShift, glyphTop, glyphCode, 1);
			head = base.elements.FindLast(e => true) as TextBlock;
			// Rysujemy linie dodane górne i dolne - jeśli nuta nie jest częścią akordu.
			if (showLegerLines)
				AddLegerLinesToScore(score, trebleStaff, bassStaff, left + headShift);

			// Rysujemy pomocniczą nazwę nuty - literę.
			double letterTop
					= staff.top * score.Magnification
					+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			double letterLeft = left + headShift;
			double letterScale = 1;
			string noteString = ToString();
			if (noteString.Length == 3) {
				letterTop -= 3.5 * score.Magnification;
				letterLeft += 2 * score.Magnification;
				letterScale = 0.7;
			} else if (noteString.Length == 2)
			{
				letterTop -= 4.5 * score.Magnification;
				letterLeft += 3 * score.Magnification;
				letterScale = 0.8;
			}
			else
			{
				letterTop -= 6 * score.Magnification;
				letterLeft += 4 * score.Magnification;
				letterScale = 0.9;
			}
			TextBlock letterTextBlock = new TextBlock
			{
				FontFamily = new FontFamily("Consolas"),
				FontSize = 12 * score.Magnification * letterScale,
				//Content = this.letter.ToString(),
				Text = ToString("{letter}"),
				Foreground = Brushes.White,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(letterLeft, letterTop, 0, 0)
			};
			base.AddElementToScore(score, letterTextBlock, 2);
			FormattedText letterFormattedText = new FormattedText(
				letterTextBlock.Text,
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface("Consolas"),
				letterTextBlock.FontSize,
				Brushes.Black);

			// Rysujemy pomocniczą nazwę nuty - krzyżyk lub bemol.
			string accidentalGlyphCode = string.Empty;
			double accidentalLeft = 0;
			FormattedText accidentalFormattedText = null;
			switch (accidentalType)
			{
				case Accidental.Type.Sharp:
					accidentalGlyphCode = "\x002e";
					break;
				case Accidental.Type.Flat:
					throw new NotImplementedException();
			}
			if (accidentalGlyphCode != string.Empty)
			{
				double accidentalTop
						= staff.top * score.Magnification
						+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
				accidentalTop -= 9 * score.Magnification;
				accidentalLeft = letterLeft + letterFormattedText.Width;
				TextBlock accidentalTextBlock = new TextBlock
				{
					FontFamily = new FontFamily("feta26"),
					FontSize = 12 * score.Magnification * 0.6,
					Text = "\x002e",
					Foreground = Brushes.White,
					Padding = new Thickness(0, 0, 0, 0),
					Margin = new Thickness(accidentalLeft, accidentalTop, 0, 0)
				};
				base.AddElementToScore(score, accidentalTextBlock, 2);
				accidentalFormattedText = new FormattedText(
					accidentalTextBlock.Text,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface("feta26"),
					letterTextBlock.FontSize,
					Brushes.Black);
			}

			// Rysujemy pomocniczą nazwę nuty - numer oktawy.
			double octaveTop
						= staff.top * score.Magnification
						+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			octaveTop -= 5.5 * score.Magnification;
			double octaveLeft
				= accidentalGlyphCode == string.Empty 
				? letterLeft + letterFormattedText.Width
				: accidentalLeft + accidentalFormattedText.Width;
			TextBlock octaveTextBlock = new TextBlock
			{
				FontFamily = new FontFamily("Consola"),
				FontSize = 12 * score.Magnification * 0.5,
				Text = ToString("{octave}"),
				Foreground = Brushes.White,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(octaveLeft, octaveTop, 0, 0)
			};
			base.AddElementToScore(score, octaveTextBlock, 2);

			// Dodajemy prostokąt reagujący na mysz.
			double top = base.bounds.Top;
			double bottom = base.bounds.Bottom;
			highlightRect = new Rectangle
			{
				Width = right - left,
				Height = bottom - top,
				Margin = new Thickness(left, top, 0, 0),
				Fill = Brushes.Transparent,
				Stroke = Brushes.Transparent,
				Tag = score // potrzebne w event handlerze
			};
			base.AddElementToScore(score, highlightRect, 101);
			highlightRect.MouseEnter += MouseEnter;
			highlightRect.MouseLeave += MouseLeave;
			highlightRect.MouseDown += MouseDown;
			highlightRect.MouseUp += MouseUp;

			// Czy znak zmieścił sie na pięcolinii?
			if (right >= score.ActualWidth - Staff.marginLeft)
			{
				// Nie zmieścił się - narysujemy ją na następnej pieciolinii.
				RemoveFromScore(score);
				return -1;
			}
			else
			{
				// Znak zmieścił sie na pięciolinii.
				//right += Staff.spaceBetweenSigns * score.Magnification;
				return right + Staff.spaceBetweenSigns * score.Magnification;
			}
		}
	
		public override void RemoveFromScore(Score score)
		{
			accidental?.RemoveFromScore(score);
			base.RemoveFromScore(score);
		}

		private void AddLegerLinesToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
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
					base.AddElementToScore(score, legerLine);
				}
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
					base.AddElementToScore(score, legerLine);
				}
			}
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

		public string ToString(string format)
		{
			string octaveIndex = "";
			bool letterUpper = false;
			switch (octave)
			{
				case Octave.SubContra:
					octaveIndex = "2";
					letterUpper = true;
					break;
				case Octave.Contra:
					octaveIndex = "1";
					letterUpper = true;
					break;
				case Octave.Great:
					octaveIndex = "";
					letterUpper = true;
					break;
				case Octave.Small:
					octaveIndex = "";
					letterUpper = false;
					break;
				case Octave.OneLined:
					octaveIndex = "1";
					letterUpper = false;
					break;
				case Octave.TwoLined:
					octaveIndex = "2";
					letterUpper = false;
					break;
				case Octave.ThreeLined:
					octaveIndex = "3";
					letterUpper = false;
					break;
				case Octave.FourLined:
					octaveIndex = "4";
					letterUpper = false;
					break;
				case Octave.FiveLined:
					octaveIndex = "5";
					letterUpper = false;
					break;
			}
			string accidental;
			switch (accidentalType)
			{
				case Accidental.Type.None:
				default:
					accidental = "";
					break;
				case Accidental.Type.Flat:
					accidental = "b";
					break;
				case Accidental.Type.Sharp:
					accidental = "#";
					break;
				case Accidental.Type.Natural:
					accidental = "N";
					break;
			}

			format = format.Replace("{letter}", letterUpper ? letter.ToString().ToUpper() : letter.ToString().ToLower());
			format = format.Replace("{accidental}", accidental);
			format = format.Replace("{octave}", octaveIndex);

			return format;
			//return string.Format(
			//	"{0}{1}{2}",
			//	letterUpper ? letter.ToString().ToUpper() : letter.ToString().ToLower(),
			//	accidental,
			//	octaveIndex);
		}

		public override string ToString()
		{
			return ToString("{letter}{accidental}{octave}");
		}

		public override bool Equals(object obj)
		{
			Note other = obj as Note;

			if (obj == null)
				return false;

			if (other == null)
				return false;

			if (this.letter == other.letter &&
				this.accidentalType == other.accidentalType &&
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

		private void MouseEnter(object sender, MouseEventArgs e)
		{
			if (HighlightingIsActive)
			{
				// Pokoloruj nutę.
				highlighted = true;
				SetColor();
				// Pokoloruj krok.
				step.Highlight(true);
				// Wygeneruj zdarzenie o najechaniu na nutę.
				Score score = (sender as Rectangle).Tag as Score;
				score.FireEvent(this, ScoreEventArgs.EventType.HighlightedOn);
			}
			else
			{
				// Przekaż zdarzenie do kroku.
				step.MouseEnter(sender, e);
			}
		}

		private void MouseLeave(object sender, MouseEventArgs e)
		{
			// Pokoloruj nutę.
			highlighted = false;
			SetColor();
			if (HighlightingIsActive)
			{
				// Pokoloruj krok.
				step.Highlight(false);
				// Wygeneruj zdarzenie o zjechaniu z nuty.
				Score score = (sender as Rectangle).Tag as Score;
				score.FireEvent(this, ScoreEventArgs.EventType.HighlightedOff);
			}
			else
			{
				// Przekaż zdarzenie do kroku.
				step.MouseLeave(sender, e);
			}
		}

		private void MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (HighlightingIsActive)
			{
				// Ustaw krok na bieżący.
				Score score = (sender as Rectangle).Tag as Score;
				score.CurrentStep = step;
				// Wygeneruj zdarzenie o naciśnięciu nuty.
				MouseEnter(sender, e);
				score.FireEvent(this, ScoreEventArgs.EventType.Selected);
			}
			else
			{
				// Przekaż zdarzenie do kroku.
				step.MouseDown(sender, e);
			}
		}

		private void MouseUp(object sender, MouseButtonEventArgs e)
		{
			Score score = (sender as Rectangle).Tag as Score;
			if (HighlightingIsActive)
			{
				// Wygeneruj zdarzenie o puszczeniu nuty.
				//score.FireEvent(this, ScoreEventArgs.EventType.MouseUp);
			}
			else
			{
				// Przekaż zdarzenie do kroku.
				//step.MouseUp(sender, e);
			}
		}

		private void SetColor()
		{
			if (step.IsCurrent && highlighted)
			{
				head.Foreground = currentBrush;
			}
			else if (step.IsCurrent && !highlighted)
			{
				head.Foreground = Brushes.Black;

			}
			else if (!step.IsCurrent && highlighted)
			{
				head.Foreground = highlightBrush;
			}
			else if (!step.IsCurrent && !highlighted)
			{
				head.Foreground = Brushes.Black;
			}

			if (Guessed == true)
			{
				head.Foreground = Brushes.Green;
			}
			else if (Guessed == false)
			{
				head.Foreground = Brushes.Red;
			}
		}

		static public string OctaveToString(Octave octave)
		{
			switch (octave)
			{
				case Note.Octave.SubContra:
					return "SUB CONTRA 2";
				case Note.Octave.Contra:
					return "CONTRA 1";
				case Note.Octave.Great:
					return "GREAT";
				case Note.Octave.Small:
					return "small";
				case Note.Octave.OneLined:
					return "1";
				case Note.Octave.TwoLined:
					return "2";
				case Note.Octave.ThreeLined:
					return "3";
				case Note.Octave.FourLined:
					return "4";
				case Note.Octave.FiveLined:
					return "5";
				default:
					return "";
			}
		}
	}
}
