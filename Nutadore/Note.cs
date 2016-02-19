using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Note : Sign
    {
        public Letter letter;
        public Octave octave;
		public Perform.HowTo performHowTo;
        public StaffPosition staffPosition = StaffPosition.ByLine(1);
        public Staff.Type staffType;

		public double left;
		public double right; 

		public Note(Letter letter, Octave octave, Staff.Type? preferredStaffType = null)
        {
            this.letter = letter;
            this.octave = octave;
            this.staffPosition = ToStaffPosition(preferredStaffType);
        }

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

		public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			// Left i right będą potrzebne do rysowania znaków ottavy
			this.left = left;

			// Na której pięciolinii ma być umieszczona nuta?
			Staff staff
				= staffType == Staff.Type.Treble
				? trebleStaff
				: bassStaff;

			// Rysujemy znak nuty.
			string glyphCode = "\x0056";
			double glyphTop
					= staff.top * score.Magnification
					 + (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			glyphTop -= 57.5 * score.Magnification;
			right = base.ShowFetaGlyph(score, left, glyphTop, glyphCode);

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

			// Rysujemy pomocniczą nazwę nuty.
			double letterTop
					= staff.top * score.Magnification
					+ (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			letterTop -= 7 * score.Magnification;
			double letterLeft = left + 3 * score.Magnification;
			Label letterLabel = new Label
			{
				FontFamily = new FontFamily("Consolas"),
				FontSize = 12 * score.Magnification,
				Content = this.letter.ToString(),
				Foreground = Brushes.White,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(letterLeft, letterTop, 0, 0)
			};
			base.AddElement(score, letterLabel);

			// Czy znak zmieścił sie na pięcolinii?
			if (right >= score.canvas.ActualWidth - Staff.marginLeft)
			{
				// Nie zmieścił się - narysujemy ją na następnej pieciolinii.
				base.Hide(score);

				// Trzeba jeszcze narysować nienarysowane ottavy
				//if (staff.performHowTo != Perform.HowTo.AtPlace)
				//	staff.ShowPerform_OLD(); // TODO: to chyba trzeba wyrzucić

				return -1;
			}
			else
			{
				// Mogą być potrzebne znaki zmiany wysokości wykonania
				//staff.FindPerform(this, left, right); // TODO: to chyba trzeba wywalić

				// Znak zmieścił sie na pięciolinii.
				//right += Staff.spaceBetweenSigns * score.Magnification;
				return right + Staff.spaceBetweenSigns * score.Magnification;
			}

			//return right;
		}

		override public void Hide(Score score)
		{
			base.Hide(score);
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
    }
}
