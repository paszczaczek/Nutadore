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

        public Sign[] Signs()
        {
            return new Sign[] {
                new Sharp(StaffPosition.ByLine(5)),
                new Sharp(StaffPosition.ByLine(3, true)),
                new Sharp(StaffPosition.ByLine(5, true)),
                new Sharp(StaffPosition.ByLine(4)),
                new Sharp(StaffPosition.ByLine(2, true)),
                new Sharp(StaffPosition.ByLine(4, true)),
                new Sharp(StaffPosition.ByLine(3))
            };
        }
    }
}
