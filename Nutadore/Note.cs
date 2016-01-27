using System;

namespace Nutadore
{
    public class Note : Sign
    {
        public Letter letter;
        public Octave octave;
        public ShowAt showAt;

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

        override public double Show(Score score, double left, double top)
        {           
            base.code = "\x0055";
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
        public StaffPosition CalculateStaffPosition(Staff.Type? preferredStaffType)
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
                        lineNumber = 1.5f;
                    }
                    else
                    {
                        base.staffType = Staff.Type.Treble;
                        lineNumber = -4.5f;
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
                        lineNumber = -1.0f;
                    }
                    else
                    {
                        base.staffType = Staff.Type.Bass;
                        lineNumber = 5.0f;
                    }
                    break;
                case Octave.TwoLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 2.5f;
                    break;
                case Octave.ThreeLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 6.0f;
                    break;
                case Octave.FourLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 9.5f;
                    break;
                case Octave.FiveLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNumber = 13.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("octave", octave, "Nieobsłużona oktawa.");
            }

            // czy nutę należy rysować w ottavie?
            if (octave == Octave.FourLined && letter >= Letter.D ||
                octave > Octave.FourLined)
            {
                // tak, na pięcionii wiolinowej rysujemy o oktawę niżej
                showAt = ShowAt.OctaveBelow;
                lineNumber -= 3.5f;
            }
            if (octave == Octave.Contra && letter <= Letter.E ||
                octave < Octave.Contra)
            {
                // tak, na pięciolinii basowej rysujemy o oktawę wyżej
                showAt = ShowAt.OctaveAbove;
                lineNumber += 3.5f;
            }

            // dodajemy przesunięcie względem dzięku C
            lineNumber += (double)letter / 2;

            return StaffPosition.ByLineNumber(lineNumber);
        }

        public enum ShowAt
        {
            Place,
            OctaveBelow,
            OctaveAbove
        }
    }
}
