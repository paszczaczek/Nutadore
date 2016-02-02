using System;
using System.Windows.Media;

namespace Nutadore
{
    public class Note : Sign
    {
        public Letter letter;
        public Octave octave;
        public Perform perform;

        public Note(Letter letter, Octave octave, Staff.Type? preferredStaffType = null)
        {
            this.letter = letter;
            this.octave = octave;
            base.staffType = staffType;
            base.staffPosition = CalculateStaffPosition(preferredStaffType);
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
            base.code = "\x0056";
            //base.brush = Brush;
            top -= 57.5 * score.Magnification;
            double right = base.Show(score, left, top);

            return right;
        }

        /// <summary>
        /// Wylicza numer linii na pięciolinii na której leży nuta.
        /// </summary>
        /// <param name="staffType">Na której pięciolinii basowej czy wiolinowej chcemy nutę. Nuty z od f do g1 mogą być umieszczane na obu.</param>
        /// <param name="considerOttava">Dla linii pomocniczych pokazujących położenie nut C w różnych oktawach nie chcemy ottawy.</param>
        /// <returns></returns>
        public StaffPosition CalculateStaffPosition(Staff.Type? preferredStaffType, bool withPerform = true)
        {
            // na której linii leży dźwięk C z oktawy w której jest nuta
            double lineNumber = 0;
            switch (octave)
            {
                case Octave.SubContra:
                    // Wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej.
                    base.staffType = Staff.Type.Bass;
                    lineNumber = -9.0;
                    break;
                case Octave.Contra:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej
                    base.staffType = Staff.Type.Bass;
                    lineNumber = -5.5;
                    break;
                case Octave.Great:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej
                    base.staffType = Staff.Type.Bass;
                    lineNumber = -2.0;
                    break;
                case Octave.Small:
                    // domyślnie wszystkie nuty z tej oktawy leżą na pięciolinii basowej
                    // nuty od F włącznie mogą leżeć na pięciolini wiolinowej
                    if (preferredStaffType == null ||
                        preferredStaffType == Staff.Type.Bass || 
                        preferredStaffType == Staff.Type.Treble && letter < Letter.F)
                    {
                        base.staffType = Staff.Type.Bass;
                        lineNumber = 1.5;
                    }
                    else
                    {
                        base.staffType = Staff.Type.Treble;
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
                        base.staffType = Staff.Type.Treble;
                        lineNumber = -1.0;
                    }
                    else
                    {
                        base.staffType = Staff.Type.Bass;
                        lineNumber = 5.0;
                    }
                    break;
                case Octave.TwoLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 2.5;
                    break;
                case Octave.ThreeLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 6.0;
                    break;
                case Octave.FourLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 9.5;
                    break;
                case Octave.FiveLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
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
