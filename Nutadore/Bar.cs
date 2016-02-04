using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Bar: Sign
    {
        public Bar()
        {
            //base.staffPosition = StaffPosition.ByLine(5);
        }

        override public double Show(Score score, double left, double top)
        {
            //base.score = score;
            base.uiElement = new Line
            {
                X1 = left,
                X2 = left,
                Y1 = top,
                Y2 = top + Staff.spaceBetweenLines * 4 * score.Magnification,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            score.canvas.Children.Add(base.uiElement);

            return left + 1;
        }
    }
}
