using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    internal class Staff
    {
        private readonly static double _marginTop = 20;
        private readonly static double _distanceBetweenLines = 10;
        private readonly static FontFamily fontFamily = new FontFamily("feta26");

        private Canvas _canvas;
        private double _y;
        private double _magnification;
        private double _curPos;

        // konstruktor
        public Staff(Canvas canvas, double y, double magnification)
        {
            _canvas = canvas;
            _y = y;
            _magnification = magnification;
            _curPos = 0;
        }

        public void Paint()
        {
            double staffsLeft = _PaintBracket();
            _PaintStaffs(staffsLeft);
            _PaintClefs(staffsLeft);
        }

        // rysuje klamrę spinającą pięciolinię wiolinową i basową.
        private double _PaintBracket()
        {
            // wyznaczam wymiary klamry
            string familyName = "MS Mincho";
            double fontSize = 116 * _magnification;
            FormattedText formattedText = new FormattedText(
                "{", 
                CultureInfo.GetCultureInfo("en-us"), 
                FlowDirection.LeftToRight, 
                new Typeface(familyName), 
                fontSize, 
                Brushes.Black);
            double bracketWidth = formattedText.Width;
            double bracketHeight = formattedText.Height;
            double bracketOffestX = bracketWidth * 0.3;
            double bracketOffsetY = bracketHeight * 0.07;

            // rysuję klamrę
            Label bracket = new Label();
            bracket.FontFamily = new FontFamily(familyName);
            bracket.FontSize = fontSize;
            bracket.Content = "{";
            bracket.Padding = new Thickness(0, 0, 0, 0);
            bracket.Margin = new Thickness(
                    -bracketOffestX, 
                    _marginTop * _magnification - bracketOffsetY, 
                    0, 
                    0);
            this._canvas.Children.Add(bracket);

            // zwracam miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
            return bracketWidth - bracketOffestX;
        }

        // rysuje pięciolinie wiolinową i basową
        private void _PaintStaffs(double staffsLeft)
        {
            for (int staffNo = 0; staffNo < 2; staffNo++)
            {
                for (int lineNo = 0; lineNo < 5; lineNo++)
                {
                    Line line = new Line();
                    line.X1 = staffsLeft;
                    line.X2 = _canvas.ActualWidth - 10;
                    line.Y1 = (_y + Staff._marginTop + (lineNo + staffNo * 6) * _distanceBetweenLines) * _magnification;
                    line.Y2 = line.Y1;
                    line.Stroke = Brushes.Black;
                    line.StrokeThickness = 0.5;
                    this._canvas.Children.Add(line);
                }
            }
        }

        // rysuje klucz wiolinowy i basowy
        private void _PaintClefs(double sraffsLeft)
        {

        }

        public bool PaintSing(Sign s)
        {
            return true;
        }
    }
}