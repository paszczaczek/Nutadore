using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    public class Staff
    {
        private readonly static double _distanceBetweenLines = 10;

        private Clef _clef;

        static public Staff Treble()
        {
            return new Staff(Type.Treble);
        }

        static public Staff Bass()
        {
            return new Staff(Type.Bass);
        }

        public enum Type
        {
            Treble = 0,
            Bass
        }

        public Type type;
        private Staff(Type type)
        {
            this.type = type;
            _clef = new Clef(type);
        }

        public int Number
        {
            get { return (int)type; }
        }

        public void Paint(Canvas canvas, double left, double top, double magnification)
        {
            // Rysuję pięciolinię.
            for (var staffLine = StaffLine.Base(1); staffLine <= StaffLine.Base(5); staffLine++)
            {
                double y = _LineY(staffLine, Number, top, magnification);
                System.Windows.Shapes.Line shapeLine = new System.Windows.Shapes.Line
                {
                    X1 = left,
                    X2 = canvas.ActualWidth - 10,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                canvas.Children.Add(shapeLine);
            }

            // Rysuję klucz wiolinowy lub basowy.
            double clefTop = _LineY(StaffLine.Base(2), Number, top, magnification);
            _clef.Paint(canvas, left, clefTop, magnification);
        }

        public double _LineY(StaffLine staffLine, int staffNumber, double top, double magnification)
        {
            return
                top * magnification // tu będzie piąta linia
                + staffNumber * 6 * _distanceBetweenLines * magnification // pięciolinia wiolinowa lub basowa
                + (4 - staffLine.Number) * _distanceBetweenLines * magnification; // numer linii
        }
    }
}