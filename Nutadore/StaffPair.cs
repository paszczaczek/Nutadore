using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    public partial class StaffPair
    {
        private readonly static double _marginTop = 20;

        private Canvas _canvas;
        private double _top;
        private double _magnification;

        Staff _trebleStaff = Staff.Treble();
        Staff _bassStaff = Staff.Bass();
        Scale _scale;

        // Konstruktor.
        public StaffPair(Canvas canvas, Scale scale, double top, double magnification)
        {
            _canvas = canvas;
            _top = top;
            _magnification = magnification;
            _scale = scale;
        }

        // Rysuje pięciolinie i klamrę je spinającą.
        public void Paint()
        {
            double staffLeft = _PaintBrace();

            double scaleLeft = _trebleStaff.Paint(_canvas, _scale, staffLeft, _marginTop, _magnification);
            _bassStaff.Paint(_canvas, _scale, staffLeft, _marginTop, _magnification);
        }

        // Rysuje klamrę spinającą pięciolinię wiolinową i basową.
        private double _PaintBrace()
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
            double braceWidth = formattedText.Width;
            double braceHeight = formattedText.Height;
            double braceOffestX = braceWidth * 0.3;
            double braceOffsetY = braceHeight * 0.07;

            // rysuję klamrę
            Label brace = new Label();
            brace.FontFamily = new FontFamily(familyName);
            brace.FontSize = fontSize;
            brace.Content = "{";
            brace.Padding = new Thickness(0, 0, 0, 0);
            brace.Margin = new Thickness(
                    -braceOffestX, 
                    _marginTop * _magnification - braceOffsetY, 
                    0, 
                    0);
            _canvas.Children.Add(brace);

            // zwracam miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
            return braceWidth - braceOffestX;
        }

        public bool PaintSing(Sign s)
        {
            return true;
        }
    }
}