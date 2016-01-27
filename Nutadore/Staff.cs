using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Staff
    {
        private readonly static double distanceBetweenLines = 10;
        private readonly static double distanceBetweenScaleSigns = 1;
        private readonly static double distanceBetweenSigns = 10;
        private readonly static double marginLeft = 10;

        private Type type;
        private double left;
        private double top;

        public enum Type
        {
            Treble = 0,
            Bass
        }

        public Staff(Type type)
        {
            this.type = type;            
        }

        public double Show(Score score, double staffLeft, double staffTop)
        {
            left = staffLeft;
            top = staffTop;

            // Dodaję pięciolinię.
            for (var staffLine = StaffPosition.CreateByLine(1); staffLine <= StaffPosition.CreateByLine(5); staffLine++)
            {
                double y = StaffPositionToY(score, staffLine, type);
                Line shapeLine = new Line
                {
                    X1 = staffLeft,
                    X2 = score.canvas.ActualWidth - marginLeft,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                score.canvas.Children.Add(shapeLine);
            }

            // Dodaję klucz.
            Clef clef = new Clef((Clef.Type)type);
            double clefTop = StaffPositionToY(score, StaffPosition.CreateByLine(2), type);
            double clefRight = clef.Show(score, staffLeft, clefTop);

            // Dodaję znaki przykluczowe.
            double signLeft = clefRight + 10 * score.Magnification;
            foreach (Sign sign in score.scale.Signs())
            {
                double signTop = StaffPositionToY(score, sign.staffPosition, type);
                signLeft
                    = sign.Show(score, signLeft, signTop)
                    + distanceBetweenScaleSigns * score.Magnification;
            }

            // Zwracam miejsce w którym mozna umieszczać następny znak
            return signLeft + distanceBetweenSigns;
        }

        public double ShowSign(Score score, Sign sign, double left)
        {
            // TODO: tak naprawdę sign będzie kolekcją zawierająca akordy wiolinowe i basowe
            // TODO: wyznaczyć na podstwie wysokości nuty
            //StaffPosition staffPosition = StaffPosition.Above.Line(1);
            //double signTop = StaffPositionToY(score, staffPosition, type);
            double signTop = StaffPositionToY(score, sign.staffPosition, type);
            left = sign.Show(score, left, signTop);

            // Czy nuta zmieściła sie na pięcolinii?
            if (left >= score.canvas.ActualWidth - marginLeft)
            {
                // Nie zmieściła się - narysujemy ją na następnej pieciolinii.
                sign.Clear();
                return -1;
            }
            else
            {
                // Zmieściła się.
                left += distanceBetweenSigns;
                return left;
            }
        }

        public double StaffPositionToY(Score score, StaffPosition staffLine, Type staffType)
        {
            return
                // tu będzie piąta linia
                top * score.Magnification
                // pięciolinia wiolinowa lub basowa
                //+ (int)staffType * (5 + 1) * distanceBetweenLines * score.Magnification
                // numer linii
                + (4 - staffLine.LineNumber) * distanceBetweenLines * score.Magnification;
        }
    }
}