using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Staff
    {
        static public readonly double marginLeft = 10;
        static public readonly double spaceBetweenLines = 10;
        static public readonly double spaceBetweenScaleSigns = 1;
        static public readonly double spaceBetweenSigns = 15;

        private Score score;
        public Type type;
        private double left;
        public double top;

        public enum Type
        {
            Treble = 0,
            Bass
        }

        public Staff(Score score, Type type, double left, double top)
        {
            this.score = score;
            this.type = type;
            this.left = left;
            this.top = top;
        }

        public double Show()
        {
            // Dodaję pięciolinię.
            ShowLines();

            // Dodaję pomocnicze kolorowe linie.
            ShowHelperLines();

            return left;
        }

        private void ShowLines()
        {
            for (var staffPosition = StaffPosition.ByLine(1);
                staffPosition <= StaffPosition.ByLine(5);
                staffPosition.AddLine(1))
            {
                double y = StaffPositionToY(staffPosition);
                Line shapeLine = new Line
                {
                    X1 = left,
                    X2 = score.ActualWidth - marginLeft,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                score.Children.Add(shapeLine);
            }
        }

        private void ShowHelperLines()
        {
            for (var octave = Note.Octave.Great; octave <= Note.Octave.ThreeLined; octave++)
            {
                foreach (var letter in new[] { Note.Letter.C/*, Note.Letter.F*/ })
                {
                    var note = new Note(letter, octave);
                    var staffPosition = note.ToStaffPosition(type, false);
                    if (note.staffType != type)
                        continue;
                    double y = StaffPositionToY(staffPosition);
                    Line shapeLine = new Line
                    {
                        X1 = left,
                        X2 = score.ActualWidth - marginLeft,
                        Y1 = y,
                        Y2 = y,
                        Stroke = Brushes.Red, // note.Brush,
                        StrokeThickness = 0.4
                    };
                    if (letter == Note.Letter.F)
                        shapeLine.StrokeDashArray = new DoubleCollection { 6, 6 };
                    score.Children.Add(shapeLine);
                }
            }
        }

        public double StaffPositionToY(StaffPosition staffPosition)
        {
            return
                // tu będzie piąta linia
                top * score.Magnification
                + (4 - staffPosition.Number) * spaceBetweenLines * score.Magnification;
        }
    }
}