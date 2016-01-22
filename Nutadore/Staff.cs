using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Staff
    {
        private readonly static double distanceBetweenLines = 10;
        private readonly static double distanceBetweenSigns = 1;

        private Score score;
        private Type type;
        private double left;
        private double top;

        public enum Type
        {
            Treble = 0,
            Bass
        }

        public Staff(Score score, Type staffType)
        {
            this.score = score;
            type = staffType;            
        }

        public double Show(double staffLeft, double staffTop)
        {
            left = staffLeft;
            top = staffTop;

            // Dodaję pięciolinię.
            for (var staffLine = StaffPosition.Line(1); staffLine <= StaffPosition.Line(5); staffLine++)
            {
                double y = StaffPositionToY(staffLine, type);
                Line shapeLine = new Line
                {
                    X1 = staffLeft,
                    X2 = score.canvas.ActualWidth - 10,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                score.canvas.Children.Add(shapeLine);
            }

            // Dodaję klucz.
            Clef clef = new Clef(score, (Clef.Type)type);
            double clefTop = StaffPositionToY(StaffPosition.Line(2), type);
            double clefRight = clef.Show(staffLeft, clefTop);

            // Dodaję znaki przykluczowe.
            double signLeft = clefRight + 10 * score.Magnification;
            foreach (Sign sign in score.scale.Signs())
            {
                double signTop = StaffPositionToY(sign.staffPosition, type);
                signLeft 
                    = sign.Show(signLeft, signTop)
                    + distanceBetweenSigns * score.Magnification;
            }

            // zwracam miejsce w którym mozna umieszczać następny znak
            return signLeft;
        }

        public double StaffPositionToY(StaffPosition staffLine, Type staffType)
        {
            return
                // tu będzie piąta linia
                top * score.Magnification
                // pięciolinia wiolinowa lub basowa
                + (int)staffType * (5 + 1) * distanceBetweenLines * score.Magnification
                // numer linii
                + (4 - staffLine.ToDouble()) * distanceBetweenLines * score.Magnification;
        }

        public double Add(Sign sign, double left)
        {
            // TODO: wyznaczyć na podstwie wysokości nuty
            StaffPosition staffPosition = StaffPosition.Above.Line(1);
            double signTop = StaffPositionToY(staffPosition, type);
            return
                sign.Show(left, signTop)
                + distanceBetweenSigns ;
        }
    }
}