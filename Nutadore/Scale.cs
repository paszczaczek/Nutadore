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
