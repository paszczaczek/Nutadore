using System;

namespace Nutadore
{
    public class Note : Sign
    {
        public Letter letter;
        public Octave octave;
        //public Staff.Type staffType;
        //private ShowAt showAt;

        public Note(Letter letter, Octave octave, Staff.Type staffType = Staff.Type.Treble)
        {
            this.letter = letter;
            this.octave = octave;
            base.staffType = staffType;
            base.staffPosition = CalculateStaffPosition();
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
        public StaffPosition CalculateStaffPosition()
        {
            // na której linii leży dźwięk C z oktawy w której jest nuta
            double lineNo = 0;
            switch (octave)
            {
                case Octave.SubContra:
                    // Wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej.
                    base.staffType = Staff.Type.Bass;
                    lineNo = -9.0;
                    break;
                case Octave.Contra:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej
                    base.staffType = Staff.Type.Bass;
                    lineNo = -5.5;
                    break;
                case Octave.Great:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii basowej
                    base.staffType = Staff.Type.Bass;
                    lineNo = -2.0;
                    break;
                case Octave.Small:
                    // wszystkie nuty z tej oktawy mogą leżeć na pięciolinii basowej
                    // nuty od F włącznie mogą leżeć na pięciolini wiolinowej
                    if (base.staffType == Staff.Type.Bass || 
                        base.staffType == Staff.Type.Treble && letter < Letter.F)
                    {
                        base.staffType = Staff.Type.Bass;
                        lineNo = 1.5f;
                    }
                    else
                    {
                        lineNo = -4.5f;
                    }
                    break;
                case Octave.OneLined:
                    // wszystkie nuty z tej oktawy mogą leżeć na pięciolinii wiolinowej
                    // nuty do G włącznie mogą leżeć na pięciolini basowej
                    if (base.staffType == Staff.Type.Treble || 
                        base.staffType == Staff.Type.Bass && letter > Letter.G)
                    {
                        base.staffType = Staff.Type.Treble;
                        lineNo = -1.0f;
                    }
                    else
                    {
                        lineNo = 5.0f;
                    }
                    break;
                case Octave.TwoLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNo = 2.5f;
                    break;
                case Octave.ThreeLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNo = 6.0f;
                    break;
                case Octave.FourLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNo = 9.5f;
                    break;
                case Octave.FiveLined:
                    // wszystkie nuty z tej oktawy mogą leżeć tylko na pięciolinii wiolinowej
                    base.staffType = Staff.Type.Treble;
                    lineNo = 13.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("octave", octave, "Nieobsłużona oktawa.");
            }

            // czy nutę należy rysować w ottavie?
            if (octave == Octave.FourLined && letter >= Letter.D ||
                octave > Octave.FourLined)
            {
                // tak, na pięcionii wiolinowej będzie o oktawę niżej
                //showAt = ShowAt.OctaveBelow;
                lineNo -= 3.5f;
            }
            if (octave == Octave.Contra && letter <= Letter.E ||
                octave < Octave.Contra)
            {
                // tak, na pięciolinii basowej będzie o oktawę wyżej
                //showAt = ShowAt.OctaveAbove;
                lineNo += 3.5f;
            }

            // dodajemy przesunięcie względem dzięku C
            lineNo += (double)letter / 2;

            return StaffPosition.CreateByLineNumber(lineNo);
        }

        public enum ShowAt
        {
            Place,
            OctaveBelow,
            OctaveAbove
        }
    }
}
