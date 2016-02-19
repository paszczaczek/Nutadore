using System.Windows.Controls;

namespace Nutadore
{
    public class Scale
    {
        private Note.Letter based;
        private Type type;

        public Scale(Note.Letter based, Type type)
        {
            this.based = based;
            this.type = type;
        }

        public enum Type { Major, Minor }

		public double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			double signLeft = left;
			foreach (Accidental accidental in score.scale.Accidentals())
			{
				signLeft
					= accidental.Show(score, trebleStaff, bassStaff, signLeft)
					+ Staff.spaceBetweenScaleSigns * score.Magnification;
			}

			double scaleRight = signLeft + Staff.spaceBetweenSigns;

			return scaleRight;
		}

		public Accidental[] Accidentals()
        {
            return new Accidental[] {
                new Accidental(StaffPosition.ByLine(5)),
                new Accidental(StaffPosition.ByLine(3, true)),
                new Accidental(StaffPosition.ByLine(5, true)),
                new Accidental(StaffPosition.ByLine(4)),
                new Accidental(StaffPosition.ByLine(2, true)),
                new Accidental(StaffPosition.ByLine(4, true)),
                new Accidental(StaffPosition.ByLine(3))
            };
        }
    }
}
