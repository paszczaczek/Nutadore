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
        public Perform perform;
        public StaffPosition staffPosition = StaffPosition.ByLine(1);
        public Staff.Type staffType;

		private Label letterLabel;

		public Note(Letter letter, Octave octave, Staff.Type? preferredStaffType = null)
        {
            this.letter = letter;
            this.octave = octave;
            this.staffPosition = CalculateStaffPosition(preferredStaffType);
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

        public enum Perform
        {
            AtPlace,
            OneOctaveHigher,
            OneOctaveLower,
            TwoOctaveHigher
        }

        override public double Show(Score score, double left, double top)
        {           
			// Rysujemy znak nuty.
            string glyphCode = "\x0056";
            double glyphTop
                    = top
                     + (4 - staffPosition.LineNumber) * Staff.spaceBetweenLines * score.Magnification;
            glyphTop -= 57.5 * score.Magnification;
            double right = base.ShowFetaGlyph(score, left, glyphTop, glyphCode);

            double letterTop
                    = top
                    + (4 - staffPosition.LineNumber) * Staff.spaceBetweenLines * score.Magnification;
            letterTop -= 7 * score.Magnification;
            double letterLeft = left + 3 * score.Magnification;
            letterLabel = new Label
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12 * score.Magnification,
                Content = this.letter.ToString(),
                Foreground = Brushes.White,
                Padding = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(letterLeft, letterTop, 0, 0)
            };
            score.canvas.Children.Add(letterLabel);

#if false
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
					double y = StaffPositionToY(score, staffPosition);
					Line lagerLine = new Line
					{
						X1 = legerLeft,
						X2 = legerRight,
						Y1 = y,
						Y2 = y,
						Stroke = Brushes.Black,
						StrokeThickness = 0.5
					};
					score.canvas.Children.Add(lagerLine);
				}
			}
			else if (this.staffPosition >= StaffPosition.ByLegerAbove(1))
			{
				// Tak, trzeba dorysować linie dodane górne.
				for (var staffPosition = StaffPosition.ByLegerAbove(1);
					 staffPosition <= this.staffPosition;
					 staffPosition.AddLine(1))
				{
					double y = StaffPositionToY(score, staffPosition);
					Line lagerLine = new Line
					{
						X1 = legerLeft,
						X2 = legerRight,
						Y1 = y,
						Y2 = y,
						Stroke = Brushes.Black,
						StrokeThickness = 0.5
					};
					score.canvas.Children.Add(lagerLine);
				}
			}
#endif

			return right;
        }

		override public void Hide(Score score)
		{
			base.Hide(score);
			score.canvas.Children.Remove(letterLabel);
		}

		public StaffPosition CalculateStaffPosition(Staff.Type? preferredStaffType, bool withPerform = true)
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
                    perform = Perform.TwoOctaveHigher;
                    lineNumber -= 3.5 * 2;
                }
                // else if (octave == Octave.FourLined && letter >= Letter.D ||
                else if (octave == Octave.FourLined && letter >= Letter.C ||
                    octave > Octave.FourLined)
                {
                    // Tak, na pięcionii wiolinowej rysujemy o oktawę niżej.
                    perform = Perform.OneOctaveHigher;
                    lineNumber -= 3.5;
                }
                else if (octave == Octave.Contra && letter <= Letter.E ||
                    octave < Octave.Contra)
                {
                    // Tak, na pięciolinii basowej rysujemy o oktawę wyżej
                    perform = Perform.OneOctaveLower;
                    lineNumber += 3.5;
                }
            }

            // dodajemy przesunięcie względem dzięku C
            lineNumber += (double)letter / 2;

            return StaffPosition.ByLineNumber(lineNumber);
        }

        public SolidColorBrush Brush
        {
            get
            {
                switch (letter)
                {
                    case Letter.C:
                        return Brushes.Red;
                    case Letter.D:
                        return Brushes.Orange;
                    case Letter.E:
                        return Brushes.Yellow;
                    case Letter.F:
                        return Brushes.Green;
                    case Letter.G:
                        return Brushes.Blue;
                    case Letter.A:
                        return Brushes.Indigo;
                    case Letter.H:
                    default:
                        return Brushes.Violet;
                }
            }
        }
    }
}
