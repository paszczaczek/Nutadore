using System.Windows.Controls;

namespace Nutadore
{
    public class Scale
    {
        private Based based;
        private Type type;

        public Scale(Based based, Type type)
        {
            this.based = based;
            this.type = type;
        }

        public enum Type { Major, Minor }
        public enum Based { A, B, C, D, E, F, G }

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
