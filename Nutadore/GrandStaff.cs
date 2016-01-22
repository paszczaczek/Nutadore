using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    public partial class GrandStaff
    {
        private readonly static double marginTop = 20;

        private Score score;
        private double top;
        private Staff trebleStaff;
        private Staff bassStaff;
        private double cursor;

        // Konstruktor.
        public GrandStaff(Score score, double top)
        {
            this.score = score;
            this.top = top;
        }

        // Rysuje pięciolinie i klamrę je spinającą.
        public void Show()
        {
            // Rysuję klamrę.
            double staffLeft = PaintBrace();

            // Rysuję klucz wiolinowy.
            trebleStaff = new Staff(score, Staff.Type.Treble);
            double trebleStaffCursor = trebleStaff.Show(staffLeft, marginTop);

            // Rysuje klucz basowy.
            bassStaff = new Staff(score, Staff.Type.Bass);
            double bassStaffCursor = bassStaff.Show(staffLeft, marginTop);

            // Wyznaczam połoznenie kursora na GrandStaff
            cursor 
                = trebleStaffCursor > bassStaffCursor
                ? trebleStaffCursor
                : bassStaffCursor;
        }

        // Rysuje klamrę spinającą pięciolinię wiolinową i basową.
        private double PaintBrace()
        {
            // wyznaczam wymiary klamry
            string familyName = "MS Mincho";
            double fontSize = 116 * score.Magnification;
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
                    marginTop * score.Magnification - braceOffsetY, 
                    0, 
                    0);
            score.canvas.Children.Add(brace);

            // zwracam miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
            return braceWidth - braceOffestX;
        }

        public bool Add(Sign sign)
        {
            // TODO: która pięcilinia
            cursor = trebleStaff.Add(sign, cursor);
            return true;
        }
    }
}