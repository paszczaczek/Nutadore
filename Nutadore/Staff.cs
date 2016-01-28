using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Staff
    {
        static private readonly double marginLeft = 10;
        static public readonly double spaceBetweenLines = 10;
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
            double top = StaffPositionToY(score, sign.staffPosition, type);
            double right = sign.Show(score, left, top);

            // Czy nuta zmieściła sie na pięcolinii?
            if (right >= score.canvas.ActualWidth - marginLeft)
            {
                // Nie zmieściła się - narysujemy ją na następnej pieciolinii.
                sign.Clear();
                return -1;
            }

            // Nuty mogą wymagać linii dodanych i znaków ottava
            if (sign is Note)
            {
                // Czy trzeba dorysować linie dodane?
                double legerLeft = left - (right - left) * 0.2;
                double legerRight = right + (right - left) * 0.2;
                if (sign.staffPosition <= StaffPosition.ByLegerBelow(1))
                {
                    // Tak, trzeba dorysować linie dodane dolne.
                    for (var staffPosition = StaffPosition.ByLegerBelow(1);
                         staffPosition >= sign.staffPosition; 
                         staffPosition.SubstractLine(1))
                    {
                        double y = StaffPositionToY(score, staffPosition, type);
                        Line lagerLine = new Line
                        {
                            X1 = legerLeft,
                            X2 = legerRight,
                            Y1 = y,
                            Y2 = y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5
                        };
                        score.canvas.Children.Add(lagerLine);
                    }
                }
                else if (sign.staffPosition >= StaffPosition.ByLegerAbove(1))
                {
                    // Tak, trzeba dorysować linie dodane górne.
                    for (var staffPosition = StaffPosition.ByLegerAbove(1);
                         staffPosition <= sign.staffPosition;
                         staffPosition.AddLine(1))
                    {
                        double y = StaffPositionToY(score, staffPosition, type);
                        Line lagerLine = new Line
                        {
                            X1 = legerLeft,
                            X2 = legerRight,
                            Y1 = y,
                            Y2 = y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5
                        };
                        score.canvas.Children.Add(lagerLine);
                    }
                }
                
                // TODO wyłapywac początek i koniec ottavy!
                // Czy trzeba dorysować znaki ottava?
                Note note = sign as Note;
                if (note.perform != Note.Perform.AtPlace)
                {
                    double y;
                    switch (note.perform)
                    {
                        case Note.Perform.TwoOctaveHigher:
                            // Tak, trzeba dorysować znak 15ma
                            y = StaffPositionToY(score, StaffPosition.ByLegerAbove(3), type);
                            break;
                        case Note.Perform.OneOctaveHigher:
                            // Tak, trzeba dorysować znak 8va
                            y = StaffPositionToY(score, StaffPosition.ByLegerAbove(6), type);
                            break;
                        case Note.Perform.OneOctaveLower:
                        default:
                            // Tak, trzeba dorysować znak 8va
                            y = StaffPositionToY(score, StaffPosition.ByLegerBelow(4), type);
                            break;
                    }
                    Line ottavaLine = new Line
                    {
                        X1 = left,
                        X2 = right + distanceBetweenSigns,
                        Y1 = y,
                        Y2 = y,
                        Stroke = Brushes.Red,
                        StrokeThickness = 0.5
                    };
                    score.canvas.Children.Add(ottavaLine);
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
                + (4 - staffPosition.LineNumber) * spaceBetweenLines * score.Magnification;
        }
    }
}