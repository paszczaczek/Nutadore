using System.Windows.Controls;

namespace Nutadore
{
    public class Scale
    {
        private Based _based;
        private Type _type;

        public Scale(Based based, Type type)
        {
            this._based = based;
            this._type = type;
        }

        public enum Type { Major, Minor }
        public enum Based { A, B, C, D, E, F, G }

        public Sign[] Signs()
        {
            return new Sign[] {
                new Sharp(Position.Line(5)),
                new Sharp(Position.Above.Line(3)),
                new Sharp(Position.Above.Line(5)),
                new Sharp(Position.Line(4)),
                new Sharp(Position.Above.Line(2)),
                new Sharp(Position.Above.Line(4)),
                new Sharp(Position.Line(3))
            };
        }
    }
}
