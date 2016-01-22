using System.Windows.Controls;

namespace Nutadore
{
    public class Scale
    {
        private Score score;
        private Based based;
        private Type type;

        public Scale(Score score, Based based, Type type)
        {
            this.score = score;
            this.based = based;
            this.type = type;
        }

        public enum Type { Major, Minor }
        public enum Based { A, B, C, D, E, F, G }

        public Sign[] Signs()
        {
            return new Sign[] {
                new Sharp(score, StaffPosition.Line(5)),
                new Sharp(score, StaffPosition.Above.Line(3)),
                new Sharp(score, StaffPosition.Above.Line(5)),
                new Sharp(score, StaffPosition.Line(4)),
                new Sharp(score, StaffPosition.Above.Line(2)),
                new Sharp(score, StaffPosition.Above.Line(4)),
                new Sharp(score, StaffPosition.Line(3))
            };
        }
    }
}
