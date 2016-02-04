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

        // Ottaviation??
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
            ShowLines(score, staffLeft);

            // Dodaję pomocnicze kolorowe linie.
            ShowHelperLines(score, staffLeft);

            // Dodaję klucz.
            double clefRight = ShowClef(score, staffLeft);

            // Dodaję znaki przykluczowe.
            double signRight = ShowScale(score, clefRight);

            // Zwracam miejsce w którym mozna umieszczać następny znak
            return signRight + spaceBetweenSigns;
        }

        private void ShowLines(Score score, double staffLeft)
        {
            for (var staffPosition = StaffPosition.ByLine(1);
                staffPosition <= StaffPosition.ByLine(5);
                staffPosition.AddLine(1))
            {
                double y = StaffPositionToY(score, staffPosition);
                Line shapeLine = new Line
                {
                    X1 = staffLeft,
                    X2 = score.canvas.ActualWidth - marginLeft,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                score.canvas.Children.Add(shapeLine);
            }
        }

        private void ShowHelperLines(Score score, double staffLeft)
        {
            for (var octave = Note.Octave.Great; octave <= Note.Octave.ThreeLined; octave++)
            {
                foreach (var letter in new[] { Note.Letter.C/*, Note.Letter.F*/ })
                {
                    var note = new Note(letter, octave);
                    var staffPosition = note.CalculateStaffPosition(type, false);
                    if (note.staffType != type)
                        continue;
                    double y = StaffPositionToY(score, staffPosition);
                    Line shapeLine = new Line
                    {
                        X1 = staffLeft,
                        X2 = score.canvas.ActualWidth - marginLeft,
                        Y1 = y,
                        Y2 = y,
                        Stroke = Brushes.Red, // note.Brush,
                        StrokeThickness = 0.4
                    };
                    if (letter == Note.Letter.F)
                        shapeLine.StrokeDashArray = new DoubleCollection { 6, 6 };
                    score.canvas.Children.Add(shapeLine);
                }
            }
        }

        private double ShowClef(Score score, double staffLeft)
        {
            Clef clef = new Clef((Clef.Type)type);
            double clefTop = StaffPositionToY(score, StaffPosition.ByLine(2));
            double clefRight = clef.Show(score, staffLeft, clefTop);
            return clefRight;
        }

        private double ShowScale(Score score, double clefRight)
        {
            double signLeft = clefRight + 10 * score.Magnification;
            foreach (Accidental accidental in score.scale.Accidentals())
            {
                double signTop = StaffPositionToY(score, accidental.staffPosition);
                signLeft
                    = accidental.Show(score, signLeft, signTop)
                    + spaceBetweenScaleSigns * score.Magnification;
            }

            return signLeft;
        }

        public double ShowSign(Score score, Sign sign, double left)
        {
            //double top = StaffPositionToY(score, sign.staffPosition);
            double right = sign.Show(score, left, top * score.Magnification);

            // Czy znak zmieścił sie na pięcolinii?
            if (right >= score.canvas.ActualWidth - marginLeft)
            {
                // Nie zmieścił się - narysujemy ją na następnej pieciolinii.
                sign.Hide(score);

                // Trzeba jeszcze narysować nienarysowane ottavy
                if (perform != Note.Perform.AtPlace)
                    ShowPerformSign(score);

                return -1;
            }

            // Jeśli to nuta, to może wymagać dodatowych elementów graficznych
            if (sign is Note)
            {
                Note note = sign as Note;

                // Nuty mogą wymagać linii dodanych i znaków ottava
                ShowLegerLines(score, note, left, right);

                // Mogą być potrzebne znaki zmiany wysokości wykonania
                FindPerformSign(score, note, left, right);
            }

            // Znak zmieścił sie na pięciolinii.
            right += spaceBetweenSigns * score.Magnification;

            return right;
        }

        private void ShowLegerLines(Score score, Note note, double left, double right)
        {
            // Czy trzeba dorysować linie dodane?
            double legerLeft = left - (right - left) * 0.2;
            double legerRight = right + (right - left) * 0.2;
            if (note.staffPosition <= StaffPosition.ByLegerBelow(1))
            {
                // Tak, trzeba dorysować linie dodane dolne.
                for (var staffPosition = StaffPosition.ByLegerBelow(1);
                     staffPosition >= note.staffPosition;
                     staffPosition.SubstractLine(1))
                {
                    double y = StaffPositionToY(score, staffPosition);
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
            else if (note.staffPosition >= StaffPosition.ByLegerAbove(1))
            {
                // Tak, trzeba dorysować linie dodane górne.
                for (var staffPosition = StaffPosition.ByLegerAbove(1);
                     staffPosition <= note.staffPosition;
                     staffPosition.AddLine(1))
                {
                    double y = StaffPositionToY(score, staffPosition);
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

        private void FindPerformSign(Score score, Note note, double left, double right)
        {
            bool performChanged = note.perform != perform;
            bool performNew = performChanged && note.perform != Note.Perform.AtPlace;
            bool performEnd = performChanged && perform != Note.Perform.AtPlace;
            if (performEnd)
            {
                // Tu wystąpił koniec znaku zmiany wysokości. Rysuję go.
                ShowPerformSign(score);
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

        public void ShowPerformSign(Score score)
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
                    y = StaffPositionToY(score, StaffPosition.ByLegerAbove(3));
                    vertLineTop = y;
                    verLineBottom = y + performHeight;
                    textTop = StaffPositionToY(score, StaffPosition.ByLegerAbove(5));
                    text = "15ma";
                    break;
                case Note.Perform.OneOctaveHigher:
                    // Tak, trzeba dorysować znak 8va.
                    y = StaffPositionToY(score, StaffPosition.ByLegerAbove(6));
                    vertLineTop = y;
                    verLineBottom = y + performHeight;
                    textTop = StaffPositionToY(score, StaffPosition.ByLegerAbove(8));
                    text = "8va";
                    break;
                case Note.Perform.OneOctaveLower:
                    // Tak, trzeba dorysować znak 8vb.
                    y = StaffPositionToY(score, StaffPosition.ByLegerBelow(4));
                    vertLineTop = y - performHeight;
                    verLineBottom = y;
                    textTop = StaffPositionToY(score, StaffPosition.ByLegerBelow(4));
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

        public double StaffPositionToY(Score score, StaffPosition staffPosition)
        {
            return
                // tu będzie piąta linia
                top * score.Magnification
                + (4 - staffPosition.LineNumber) * spaceBetweenLines * score.Magnification;
        }
    }
}