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
                new Sharp(StaffPosition.Line(5)),
                new Sharp(StaffPosition.Above.Line(3)),
                new Sharp(StaffPosition.Above.Line(5)),
                new Sharp(StaffPosition.Line(4)),
                new Sharp(StaffPosition.Above.Line(2)),
                new Sharp(StaffPosition.Above.Line(4)),
                new Sharp(StaffPosition.Line(3))
            };
        }
    }
}
