using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Note : Sign, IDuration, INoteOffsets, IComparable<Note>
	{
		#region props & types
		private static Brush currentBrush = Brushes.DarkGray; // LightSeaGreen;
		private static Brush highlightBrush = Brushes.DarkGray;
		private static Brush textOverWhiteHeadBrush = Brushes.Black;

		public Letter letter;
		public Octave octave;
		public Accidental accidental;
		public Duration duration { get; set; } = new Duration();
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

		private TextBlock head;
		public double headOffset { get; private set; }
		public double offset { private get; set; }
		public bool isHeadReversed;
		public bool IsHeadBlack { get { return duration.name != Duration.Name.Whole && duration.name != Duration.Name.Half; } }
		public int accidentalColumn = 0;
		public enum StemDirection { Up, Down }
		public StemDirection stemDirection = StemDirection.Up;
		public int finger = 0;
		public int fingerColumn;

		public enum Letter
		{
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
		#endregion

		public Note(Letter letter, Accidental.Type accidentalType, Octave octave, Duration duration = null, Staff.Type? preferredStaffType = null)
		{
			this.letter = letter;
			this.accidental = new Accidental(accidentalType);
			this.octave = octave;
			this.duration = duration ?? new Duration();
			this.staffPosition = ToStaffPosition(preferredStaffType);
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			// Left i right będą potrzebne do rysowania znaków ottavy
			this.step = step;
			right = left + offset;

			// Na której pięciolinii ma być umieszczona nuta?
			Staff staff
				= staffType == Staff.Type.Treble
				? trebleStaff
				: bassStaff;

			// Po której stronie nuty ma być numer palca.
			bool fingerAfterNote =
				isHeadReversed && stemDirection == StemDirection.Up
				|| !isHeadReversed && stemDirection == StemDirection.Down;

			// Czy w stepie są jakieś numery palców?
			bool fingersExist = step.SelectAllNotes().Exists(note => note.finger > 0);

			// Dodajemy znaki chromatyczne.
			AddAccidentalToScore(score, trebleStaff, bassStaff, step, left);
			// Dodajemy numer palca.
			if (fingersExist)
				AddFingerToScore(score, staff, left, fingerAfterNote);
			// Dodajemy głowkę nuty.
			AddHeadToScore(score, trebleStaff, bassStaff, staff, left);
			// Dodajemy numer palca.
			if (fingersExist && fingerAfterNote)
				AddFingerToScore(score, staff, left);

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
				return right + Staff.spaceBetweenSigns * score.Magnification;
			}
		}

		private void AddLegerLinesToScore(Score score, Staff trebleStaff, Staff bassStaff, Staff staff, double left)
		{
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

		private void AddFingerToScore(Score score, Staff staff, double left, bool onlyPlaceholder = false)
		{
			// Rysujemy numer palca.
			double fingerTop
					= staff.top * score.Magnification
					+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification
					- 6 * score.Magnification;
			double fingerLeft = right;
			double fingerScale = 0.9;
			TextBlock fingerTextBlock = new TextBlock
			{
				FontFamily = new FontFamily("Consolas"),
				FontSize = 12 * score.Magnification * fingerScale,
				//Text = (finger ?? 0).ToString(),
				Text = finger.ToString(),
				//Foreground = onlyPlaceholder || finger == null ? Brushes.Transparent : Brushes.Black,
				Foreground = onlyPlaceholder || finger == 0 ? Brushes.Transparent : Brushes.Black,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(fingerLeft, fingerTop, 0, 0)
			};
			base.AddElementToScore(score, fingerTextBlock, 2);
			FormattedText fingerFormattedText = new FormattedText(
				fingerTextBlock.Text,
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface("Consolas"),
				fingerTextBlock.FontSize,
				Brushes.Black);
			Rect fingerBounds = new Rect(
				fingerLeft,
				fingerTop + fingerFormattedText.Height + fingerFormattedText.OverhangAfter - fingerFormattedText.Extent,
				fingerFormattedText.Width,
				fingerFormattedText.Extent);
			base.ExtendBounds(fingerBounds);
			right = fingerLeft + fingerFormattedText.Width;
		}

		private void AddAccidentalToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			if (accidental.type == Accidental.Type.None)
				return;
			accidental.staffPosition = staffPosition;
			accidental.staffType = staffType;
			accidental.isKeySignatureHint = score.scale.AccidentalForLetter(letter) != Accidental.Type.None;
			right = accidental.AddToScore(score, trebleStaff, bassStaff, step, right);
			base.ExtendBounds(accidental.bounds);
			if (accidentalColumn > 0)
			{
				double sharpWidth = base.GlyphFormatedText(score, Accidental.sharpGlyphCode).Width;
				right += accidentalColumn * sharpWidth;
			}
		}

		private void AddHeadToScore(Score score, Staff trebleStaff, Staff bassStaff, Staff staff, double left)
		{
			// Rysujemy główkę nuty.
			string glyphCode = IsHeadBlack ? "\x0056" : "\x0055";
			double glyphTop
					= staff.top * score.Magnification
					 + (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			glyphTop -= 57.5 * score.Magnification;
			headOffset = right - left;
			double glyphLeft = right;
			double headWidth = base.GlyphFormatedText(score, glyphCode).Width;
			// Wyznaczamy położenie i offset głowki nuty.
			/*
			 *       |
			 *     # |o  (2)
			 *     #o|   (1)
			 *    
			 *   # |o    (3)
			 *   #o|     (4)
			 *     |
			 */
			if (stemDirection == StemDirection.Up)
			{
				if (!isHeadReversed)
				{
					// (1) Nuta z laseczka ku górze nieodwrócona - rysujemy przed laseczą.
					// #o|
				}
				else
				{
					// (2) Nuta z laseczką ku górze odwrocona - rysujemy ja za laseczka.
					// # |o
					glyphLeft += headWidth;
				}
			}
			else if (stemDirection == StemDirection.Down)
			{
				if (!isHeadReversed)
				{
					// (3) Nuta z laseczką ku kołowi nieodwrócona - rysujemy za laseczką.
					// # o|
					headOffset += headWidth;
					glyphLeft += headWidth;
				}
				else
				{
					// (4) Nuta z laseczką ku dołowi odwrócona - rysujemy ją przed laseczką.
					// #o|
					headOffset += headWidth;
				}
			}
			right = base.AddGlyphToScore(score, glyphLeft, glyphTop, glyphCode, 1);
			head = base.elements.FindLast(e => true) as TextBlock;
			// Rysujemy linie dodane górne i dolne - jeśli nuta nie jest częścią akordu.
			if (showLegerLines)
				AddLegerLinesToScore(score, trebleStaff, bassStaff, staff, glyphLeft);

			// Rysujemy pomocniczą nazwę nuty - literę.
			double letterTop
				= staff.top * score.Magnification
				+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			double letterLeft = glyphLeft;
			double letterScale = 1;
			string noteString = ToString();
			if (noteString.Length == 3)
			{
				letterTop -= 3.5 * score.Magnification;
				letterLeft += 2 * score.Magnification;
				letterScale = 0.7;
			}
			else if (noteString.Length == 2)
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
				Text = ToString("{letter}"),
				Foreground = IsHeadBlack? Brushes.White : textOverWhiteHeadBrush,
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
			switch (accidental.type)
			{
				case Accidental.Type.Sharp:
					accidentalGlyphCode = "\x002e";
					break;
				case Accidental.Type.Flat:
					accidentalGlyphCode = "\x003a";
					break;
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
					Text = accidentalGlyphCode,
					Foreground = IsHeadBlack? Brushes.White : textOverWhiteHeadBrush,
					Padding = new Thickness(0, 0, 0, 0),
					Margin = new Thickness(accidentalLeft, accidentalTop, 0, 0)
				};
				//if (!IsHeadBlack)
				//	letterTextBlock.Effect = shadowOverWhiteHead;
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
				Foreground = IsHeadBlack? Brushes.White : textOverWhiteHeadBrush,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(octaveLeft, octaveTop, 0, 0)
			};
			base.AddElementToScore(score, octaveTextBlock, 2);
		}

		public override void RemoveFromScore(Score score)
		{
			accidental?.RemoveFromScore(score);
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
			string accidentalSing;
			switch (accidental.type)
			{
				case Accidental.Type.None:
				default:
					accidentalSing = "";
					break;
				case Accidental.Type.Flat:
					accidentalSing = "b";
					break;
				case Accidental.Type.Sharp:
					accidentalSing = "#";
					break;
				case Accidental.Type.Natural:
					accidentalSing = "N";
					break;
			}

			string durationString = duration.ToString();

			format = format.Replace("{letter}", letterUpper ? letter.ToString().ToUpper() : letter.ToString().ToLower());
			format = format.Replace("{accidental}", accidentalSing);
			format = format.Replace("{octave}", octaveIndex);
			format = format.Replace("{duration}", durationString);

			return format;
			//return string.Format(
			//	"{0}{1}{2}",
			//	letterUpper ? letter.ToString().ToUpper() : letter.ToString().ToLower(),
			//	accidental,
			//	octaveIndex);
		}

		public override string ToString()
		{
			return ToString("{letter}{accidental}{octave}{duration}");
		}

		public override bool Equals(object obj)
		{
			Note other = obj as Note;

			if (obj == null)
				return false;

			if (other == null)
				return false;

			if (this.letter == other.letter &&
				this.accidental.type == other.accidental.type &&
				this.octave == other.octave)
				return true;

			// TODO porównywanie z krzyżykami i bemolami !

			return false;
		}

		private Note UnificateAccidentals()
		{
			// Nute z krzyżykiem przerabiamy na nute z bemolem.
			if (accidental.type == Accidental.Type.Flat)
			{
				Letter uLetter = letter == Letter.C ? Letter.H : letter - 1;
				if (octave == Octave.SubContra)
					throw new ArgumentOutOfRangeException(letter.ToString(), "Nuta poza dolnym zakresem!");
				Octave uOctave = octave - 1;

				return new Note(uLetter, Accidental.Type.Sharp, uOctave);
			}
			else
				return new Note(letter, accidental.type, octave, null, staffType);
		}

		public bool EqualsEffective(Note other)
		{
			return UnificateAccidentals().EqualsChromatic(other.UnificateAccidentals());
		}

		public bool EqualsChromatic(Note other)
		{
			return CompareTo(other) == 0;
		}

		public override int GetHashCode()
		{
			return (int)octave * 100 + (int)letter * 10 + (int)accidental.type * 1;
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
