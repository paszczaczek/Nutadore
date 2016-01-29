using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Staff
    {
        static private readonly double marginLeft = 10;
        static public readonly double spaceBetweenLines = 10;
        static private readonly double spaceBetweenScaleSigns = 1;
        static private readonly double spaceBetweenSigns = 15;

        private Type type;
        private double left;
        private double top;

        Note.Perform perform = Note.Perform.AtPlace;
        double performLeft;
        double performRight;

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
                    + spaceBetweenScaleSigns * score.Magnification;
            }

            // Zwracam miejsce w którym mozna umieszczać następny znak
            return signLeft + spaceBetweenSigns;
        }

        public double ShowSign(Score score, Sign sign, double left)
        {
            // TODO: tak naprawdę sign będzie kolekcją zawierająca akordy wiolinowe i basowe
            // TODO: wyznaczyć na podstwie wysokości nuty
            double top = StaffPositionToY(score, sign.staffPosition, type);
            double right = sign.Show(score, left, top);

            // Czy znak zmieścił sie na pięcolinii?
            if (right >= score.canvas.ActualWidth - marginLeft)
            {
                // Nie zmieścił się - narysujemy ją na następnej pieciolinii.
                sign.Clear();

                // Trzeba jeszcze narysować nienarysowane ottavy
                if (perform != Note.Perform.AtPlace)
                    ShowPerform(score);

                return -1;
            }

            // Nuty mogą wymagać linii dodanych i znaków ottava
            ShowLegerLines(score, sign, left, right);

            // Mogą być potrzebne znaki zmiany wysokości wykonania
            if (sign is Note)
            {
                Note note = sign as Note;
                bool performChanged = note.perform != perform;
                bool performNew = performChanged && note.perform != Note.Perform.AtPlace;
                bool performEnd = performChanged && perform != Note.Perform.AtPlace;
                if (performEnd)
                {
                    // Tu wystąpił koniec znaku zmiany wysokości. Rysuję go.
                    ShowPerform(score);
                    perform = Note.Perform.AtPlace;
                }
                if (performNew)
                {
                    // Tu wystąpił początek znaku zmiany wysokości. Zapamiętuję jego położnie i wysokość.
                    perform = note.perform;
                    performLeft = left;
                }
                performRight = right;
            }

            // Znak zmieścił sie na pięciolinii.
            right += spaceBetweenSigns * score.Magnification;

            return right;
        }

        private void ShowLegerLines(Score score, Sign sign, double left, double right)
        {
            if (!(sign is Note))
                return;
            
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
        }

        public void ShowPerform(Score score)
        {
            const double performHeight = 10;

            // Czy trzeba coś dorysować?
            double y;
            double vertLineTop, verLineBottom;
            double textTop;
            string text;
            switch (perform)
            {
                case Note.Perform.TwoOctaveHigher:
                    // Tak, trzeba dorysować znak 15ma
                    y = StaffPositionToY(score, StaffPosition.ByLegerAbove(3), type);
                    vertLineTop = y;
                    verLineBottom = y + performHeight;
                    textTop = StaffPositionToY(score, StaffPosition.ByLegerAbove(5), type);
                    text = "15ma";
                    break;
                case Note.Perform.OneOctaveHigher:
                    // Tak, trzeba dorysować znak 8va.
                    y = StaffPositionToY(score, StaffPosition.ByLegerAbove(6), type);
                    vertLineTop = y;
                    verLineBottom = y + performHeight;
                    textTop = StaffPositionToY(score, StaffPosition.ByLegerAbove(8), type);
                    text = "8va";
                    break;
                case Note.Perform.OneOctaveLower:
                    // Tak, trzeba dorysować znak 8vb.
                    y = StaffPositionToY(score, StaffPosition.ByLegerBelow(4), type);
                    vertLineTop = y - performHeight;
                    verLineBottom = y;
                    textTop = StaffPositionToY(score, StaffPosition.ByLegerBelow(4), type);
                    text = "8vb";
                    break;
                case Note.Perform.AtPlace:
                default:
                    // Nie trzeba nic rysować.
                    return;
            }

            // Rysuję linię poziomą.
            double delta = spaceBetweenSigns * score.Magnification / 2;
            Line horizontalLine = new Line
            {
                X1 = performLeft - delta,
                X2 = performRight + delta,
                Y1 = y,
                Y2 = y,
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection { 7, 7 },
                StrokeThickness = 0.5
            };
            score.canvas.Children.Add(horizontalLine);

            // Rysuję linię pionową.
            Line verticalLine = new Line
            {
                X1 = performRight + delta,
                X2 = performRight + delta,
                Y1 = vertLineTop,
                Y2 = verLineBottom,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5
            };
            score.canvas.Children.Add(verticalLine);

            // Rysuję oznaczenie 8va, 8vb lub 15ma
            Label name = new Label
            {
                //name.FontFamily = new FontFamily(familyName),
                FontSize = 12 * score.Magnification,
                Content = text,
                Padding = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(performLeft - delta, textTop, 0, 0)
            };
            score.canvas.Children.Add(name);
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