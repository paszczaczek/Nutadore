using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Bar: Sign
    {
        public Bar()
        {
        }

        public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
        {
            foreach (Staff staff in new[] { trebleStaff, bassStaff })
            {
                double top = staff.top * score.Magnification;
                UIElement barLine = new Line
                {
                    X1 = left,
                    X2 = left,
                    Y1 = top,
                    Y2 = top + Staff.spaceBetweenLines * 4 * score.Magnification,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                score.canvas.Children.Add(barLine);
                base.uiElements.Add(barLine);
            }

            double right = left + 1;

            // Czy znak zmieścił sie na pięcolinii?
            if (right >= score.canvas.ActualWidth - Staff.marginLeft)
            {
                // Nie zmieścił się - narysujemy ją na następnej pieciolinii.
                Hide(score);

                return -1;
            }

            right = left + 1 + Staff.spaceBetweenSigns * score.Magnification;

            return right;
        }
    }
}
