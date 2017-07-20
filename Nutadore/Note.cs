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
		private static double stemHightByLines = 3.5;

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
		public bool showStem = true;
		public Note endNoteStem;
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
			B
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

			if (this < lowest || this > highest)
				throw new ArgumentOutOfRangeException(
					"octave, letter",
					new { octave = octave, letter = letter },
					$"Wysokość nuty poza zakresem: {lowest.ToString()} .. {highest.ToString()}");
		}

		public Note Copy()
		{
			Note note = new Note(letter, accidental.type, octave, duration);
			note.staffPosition = StaffPosition.ByNumber(staffPosition.Number);

			return note;
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

			// Po której stronie nuty ma być opis nuty (nazwa i numer palca).
			bool DescrAfterHead =
				isHeadReversed && stemDirection == StemDirection.Up
				|| !isHeadReversed && stemDirection == StemDirection.Down;

			// Czy w stepie są jakieś numery palców?
			bool fingersExist = step.SelectAllNotes().Exists(note => note.finger > 0);

			// Czy i jakie opisy wyświetlać?
			bool showFingers = fingersExist && score.showFingers;
			bool showNoteNames = score.showNotesName;
			bool showDescr = showFingers || showNoteNames;

			// Dodajemy znaki chromatyczne.
			AddAccidentalToScore(score, trebleStaff, bassStaff, step, left);

			// Dodajemy opis nuty przed główką.
			if (showDescr)
				AddDescrToScore(score, staff, left, DescrAfterHead);

			// Dodajemy głowkę nuty.
			AddHeadToScore(score, trebleStaff, bassStaff, staff, left);

			// Dodajemy laseczkę.
			double stemLeft = AddStemToScore(score, trebleStaff, bassStaff, staff);

			// Dodajemy chorągiewkę.
			AddFlagToScore(score, staff, stemLeft);

			// Dodajemy kropkę do główki nuty.
			AddDotToScore(score, staff, left);

			// Dodajemy opis nuty za główką.
			if (showDescr && DescrAfterHead)
				AddDescrToScore(score, staff, left);

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

		private void AddDescrToScore(Score score, Staff staff, double left, bool onlyPlaceholder = false)
		{
			// Po której stronie nuty ma być opis nuty (nazwa i numer palca).
			bool DescrAfterNote =
				isHeadReversed && stemDirection == StemDirection.Up
				|| !isHeadReversed && stemDirection == StemDirection.Down;

			// Rysujemy numer palca.
			double descrTop
					= score.Magnification * (
					staff.top 
					+ (4 - staffPosition.Number) * Staff.spaceBetweenLines 
					/*- 7*/);
			double descrLeft = right;
			string descr = "";
			if (score.showFingers)
				descr += finger > 0 ? finger.ToString() : "";
			if (score.showNotesName)
				descr += ToString("{letter}{accidental}{octave}");
			TextBlock descrTextBlock = new TextBlock
			{
				FontFamily = new FontFamily("Consolas"),
				FontSize = 12 * score.Magnification,
				Text = descr,
				Foreground = onlyPlaceholder ? Brushes.Transparent : Brushes.Black,
				Padding = new Thickness(0, 0, 0, 0)
				//Background = Brushes.OrangeRed
			};
			base.AddElementToScore(score, descrTextBlock, 2);
			FormattedText descrFormattedText = new FormattedText(
				descrTextBlock.Text,
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface("Consolas"),
				descrTextBlock.FontSize,
				Brushes.Black);
			Rect descrBounds = new Rect(
				descrLeft,
				descrTop + descrFormattedText.Height + descrFormattedText.OverhangAfter - descrFormattedText.Extent,
				descrFormattedText.Width,
				descrFormattedText.Extent);
			base.ExtendBounds(descrBounds);
			descrTextBlock.Margin = new Thickness(descrLeft, descrTop - descrFormattedText.Height / 2, 0, 0);
			right = descrLeft + descrFormattedText.Width;
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
			FormattedText ft = base.GlyphFormatedText(score, glyphCode);
			double glyphTop
					= staff.StaffPositionToY(staffPosition)
					- ft.Baseline;
			headOffset = right - left;
			double glyphLeft = right;
			double headWidth = ft.Width;
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
					//   |
					// #o|
				}
				else
				{
					// (2) Nuta z laseczką ku górze odwrocona - rysujemy ja za laseczka.
					//   |
					// # |o
					glyphLeft += headWidth;
				}
			}
			else if (stemDirection == StemDirection.Down)
			{
				if (!isHeadReversed)
				{
					// (3) Nuta z laseczką ku kołowi nieodwrócona - rysujemy za laseczką.
					// # |o
					//   |
					headOffset += headWidth;
					glyphLeft += headWidth;
				}
				else
				{
					// (4) Nuta z laseczką ku dołowi odwrócona - rysujemy ją przed laseczką.
					// #o|
					//   |
					headOffset += headWidth;
				}
			}
			right = base.AddGlyphToScore(score, glyphLeft, glyphTop, glyphCode, 1);
			head = base.elements.FindLast(e => true) as TextBlock;
			// Rysujemy linie dodane górne i dolne - jeśli nuta nie jest częścią akordu.
			if (showLegerLines)
				AddLegerLinesToScore(score, trebleStaff, bassStaff, staff, glyphLeft);
		}

		private void AddDotToScore(Score score, Staff staff, double left)
		{
			if (!duration.dotted)
				return;

			string glyphCode = "\x0050";
			FormattedText glyphFT = base.GlyphFormatedText(score, glyphCode);
			double glyphTop
					= staff.StaffPositionToY(staffPosition)
					- glyphFT.Baseline;
			// Jeśli nuta jest na linii, to kropka musi być powyżej linni, bo to źle wygląda.
			if (!staffPosition.LineAbove)
					glyphTop -= Staff.spaceBetweenLines * score.Magnification * 0.5;
			double glyphLeft = right;
			// Przesunięcie kropki w poziomie.
			double dotOffsetRatio = 0.3;
			if (stemDirection == StemDirection.Up)
				switch (duration.name)
				{
					case Duration.Name.Sixteenth:
						dotOffsetRatio = 0.0;
						break;
					case Duration.Name.Eighth:
						if (!staffPosition.LineAbove)
							dotOffsetRatio = -0.1;
						else
							dotOffsetRatio = -0.3;
						break;
				}
			glyphLeft += Staff.spaceBetweenLines * score.Magnification * dotOffsetRatio;

			right = base.AddGlyphToScore(score, glyphLeft, glyphTop, glyphCode, 1);
		}

		private double AddStemToScore(Score score, Staff trebleStaff, Staff bassStaff, Staff staff)
		{
			// Rysujemy laseczkę.
			if (duration.name > Duration.Name.Half)
				return right;

			double stemY1;
			if (endNoteStem == null)
			{
				// Nuta nie jest częścia akordu.
				stemY1 = staff.StaffPositionToY(staffPosition);
			}
			else
			{
				// Nute jest częścią akordu, początek laseczki wynika z najwyższej/najniższej nuty.
				if (endNoteStem.staffType == Staff.Type.Treble)
					stemY1 = trebleStaff.StaffPositionToY(endNoteStem.staffPosition);
				else
					stemY1 = bassStaff.StaffPositionToY(endNoteStem.staffPosition);
			}
			double stemY2;
			double stemX;
			// Glowka nuty ma os symetrii niedokladnie w poziomie, dlatego laseczka musi zaczynac sie troche
			// wyzej/nizej bo inaczej widac róg laseczki.
			double stemY1Corr = Staff.spaceBetweenLines * score.Magnification * 0.2;
			double thicknes = 1.0 * score.Magnification;
			if (stemDirection == StemDirection.Up)
			{
				stemY1 -= stemY1Corr;
				stemY2 = staff.StaffPositionToY(StaffPosition.ByNumber(staffPosition.Number + stemHightByLines));
				stemX = right - thicknes / 2;
			}
			else
			{
				stemY1 += stemY1Corr;
				stemY2 = staff.StaffPositionToY(StaffPosition.ByNumber(staffPosition.Number - stemHightByLines));
				stemX = head.Margin.Left + thicknes / 2;
			}
			Line stem = new Line
			{
				X1 = stemX,
				X2 = stemX,
				Y1 = stemY1,
				Y2 = stemY2,
				Stroke = Brushes.Black,
				StrokeThickness = thicknes
			};
			stem.Visibility = showStem ? Visibility.Visible : Visibility.Hidden;
			base.AddElementToScore(score, stem);

			return stemX;
		}

		private void AddFlagToScore(Score score, Staff staff, double stemLeft)
		{
			// Rysujemy chorągiewkę nuty.
			string glyphCode = "";
			switch (duration.name)
			{
				case Duration.Name.ThirtySecond:
					glyphCode = "\x00bb";
					break;
				case Duration.Name.Sixteenth:
					glyphCode = "\x00ba";
					break;
				case Duration.Name.Eighth:
					glyphCode = "\x00b9";
					break;
				default:
					return;
			}
			FormattedText glyphFT = base.GlyphFormatedText(score, glyphCode);
			double glyphLeft = stemLeft;
			double glyphTop
				= staff.StaffPositionToY(staffPosition)
				- glyphFT.Baseline;
			if (stemDirection == StemDirection.Up)
			{
				glyphTop -= stemHightByLines * Staff.spaceBetweenLines * score.Magnification;
				right = base.AddGlyphToScore(score, glyphLeft, glyphTop, glyphCode, 1);
			}
			else
			{
				// tego -1 w zasadzie nie powinno być, ale tak wychodzi i nie wiem dlaczego
				glyphTop += (stemHightByLines - 1) * Staff.spaceBetweenLines * score.Magnification;
				base.AddGlyphToScore(score, glyphLeft, glyphTop, glyphCode, 1);
				TextBlock flag = base.elements.FindLast(e => true) as TextBlock;
				// skalowanie jest wzgledem osi BaseLine
				Transform transform = Transform.Parse("1, 0, 0, -1, 0, 0");
				flag.LayoutTransform = transform;
			}

			base.elements.FindLast(e => true).Visibility = showStem ? Visibility.Visible : Visibility.Hidden;
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
					octaveIndex = "3";
					letterUpper = true;
					break;
				case Octave.Contra:
					octaveIndex = "2";
					letterUpper = true;
					break;
				case Octave.Great:
					octaveIndex = "1";
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
				Letter uLetter = letter == Letter.C ? Letter.B : letter - 1;
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

		public static bool operator <(Note a, Note b)
		{
			if (Object.ReferenceEquals(a, b))
				return true;

			if ((object)a == null || (object)b == null)
				return false;

			return a.CompareTo(b) < 0;
		}

		public static bool operator >(Note a, Note b)
		{
			if (Object.ReferenceEquals(a, b))
				return true;

			if ((object)a == null || (object)b == null)
				return false;

			return a.CompareTo(b) > 0;
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
