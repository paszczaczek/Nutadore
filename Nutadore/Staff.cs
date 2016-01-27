using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Staff
    {
        static private readonly double marginLeft = 10;
        static private readonly double distanceBetweenLines = 10;
        static private readonly double distanceBetweenScaleSigns = 1;
        static private readonly double distanceBetweenSigns = 15;

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
            for (var staffPosition = StaffPosition.ByLine(1); 
                 staffPosition <= StaffPosition.ByLine(5); 
                 staffPosition.AddLine(1))
            {
                double y = StaffPositionToY(score, staffPosition, type);
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
            double clefTop = StaffPositionToY(score, StaffPosition.ByLine(2), type);
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
            double top = StaffPositionToY(score, sign.staffPosition, type);
            double right = sign.Show(score, left, top);
            double width = right - left;

            // Czy nuta zmieściła sie na pięcolinii?
            if (right >= score.canvas.ActualWidth - marginLeft)
            {
                // Nie zmieściła się - narysujemy ją na następnej pieciolinii.
                sign.Clear();
                return -1;
            }

            // jesli to nuta to może trzeba dorysować linie dodane?
            if (sign is Note)
            {
                if (sign.staffPosition <= StaffPosition.ByLagerBelow(1))
                {
                    // trzeba dorysować linie dodane dolne
                    for (var staffPosition = StaffPosition.ByLagerBelow(1);
                         staffPosition >= sign.staffPosition; 
                         staffPosition.SubstractLine(1))
                    {
                        double y = StaffPositionToY(score, staffPosition, type);
                        Line lagerLine = new Line
                        {
                            X1 = left - width * 0.3,
                            X2 = right + width * 0.3,
                            Y1 = y,
                            Y2 = y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5
                        };
                        score.canvas.Children.Add(lagerLine);
                    }
                }
                if (sign.staffPosition >= StaffPosition.ByLagerAbove(1))
                {
                    // trzeba dorysować linie dodane górne
                    for (var staffPosition = StaffPosition.ByLagerAbove(1);
                         staffPosition <= sign.staffPosition;
                         staffPosition.AddLine(1))
                    {
                        double y = StaffPositionToY(score, staffPosition, type);
                        Line lagerLine = new Line
                        {
                            X1 = left - width * 0.3,
                            X2 = right + width * 0.3,
                            Y1 = y,
                            Y2 = y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5
                        };
                        score.canvas.Children.Add(lagerLine);
                    }
                }

            }

            // Zmieściła się.
            right += distanceBetweenSigns;
            return right;
        }

        public double StaffPositionToY(Score score, StaffPosition staffPosition, Type staffType)
        {
            return
                // tu będzie piąta linia
                top * score.Magnification
                + (4 - staffPosition.LineNumber) * distanceBetweenLines * score.Magnification;
        }
    }
}