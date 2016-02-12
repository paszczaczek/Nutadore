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
        static private readonly double spaceBetweenScaleSigns = 1;
        static public readonly double spaceBetweenSigns = 15;

		private Score score;
        private Type type;
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

            // Dodaję klucz.
            double clefRight = ShowClef();

            // Dodaję znaki przykluczowe.
            double signRight = ShowScale(clefRight);

            // Zwracam miejsce w którym mozna umieszczać następny znak.
            return signRight + spaceBetweenSigns;
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
                    X2 = score.canvas.ActualWidth - marginLeft,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                score.canvas.Children.Add(shapeLine);
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

        private double ShowClef()
		{
            Clef clef = new Clef((Clef.Type)type);
            double clefTop = StaffPositionToY(StaffPosition.ByLine(2));
            double clefRight = clef.Show(score, left, clefTop);
            return clefRight;
        }

        private double ShowScale(double clefRight)
		{
            double signLeft = clefRight + 10 * score.Magnification;
            foreach (Accidental accidental in score.scale.Accidentals())
            {
                double signTop = StaffPositionToY(accidental.staffPosition);
                signLeft
                    = accidental.Show(score, signLeft, signTop)
                    + spaceBetweenScaleSigns * score.Magnification;
            }

            return signLeft;
        }

        public double StaffPositionToY(StaffPosition staffPosition)
		{
            return
                // tu będzie piąta linia
                top * score.Magnification
                + (4 - staffPosition.LineNumber) * spaceBetweenLines * score.Magnification;
        }
    }
}