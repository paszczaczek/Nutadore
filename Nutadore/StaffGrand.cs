using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    public class StaffGrand
    {
        private readonly static double marginTop = 20;
        private readonly static double distanceBetweenStaffs = 80;

        private double top;
        private Staff trebleStaff;
        private Staff bassStaff;
        private double cursor;

        // Konstruktor.
        public StaffGrand()
        {
        }

        // Rysuje pięciolinie i klamrę je spinającą.
        public bool Show(Score score, double top)
        {
            this.top = top;

            // Rysuję klamrę.
            double staffLeft = ShowBrace(score);

            // Rysuję klucz wiolinowy.
            trebleStaff = new Staff(Staff.Type.Treble);
            double trebleStaffCursor = trebleStaff.Show(score, staffLeft, top + marginTop);

            // Rysuję klucz basowy.
            bassStaff = new Staff(Staff.Type.Bass);
            double bassStaffCursor = bassStaff.Show(score, staffLeft, top + marginTop + distanceBetweenStaffs);

            // Wyznaczam połoznenie kursora na GrandStaff.
            cursor 
                = trebleStaffCursor > bassStaffCursor
                ? trebleStaffCursor
                : bassStaffCursor;

            // Rusuję nienarysowane jeszcze na piecilinii nuty i inne znaki.
            List<Sign> signsNotShown = score.signs.FindAll(sign => !sign.Shown);
            foreach (Sign sign in signsNotShown)
            {
                Staff staff
                    = sign.staffType == Staff.Type.Treble 
                    ? trebleStaff
                    : bassStaff;
                cursor = staff.ShowSign(score, sign, cursor);

                // Czy wystarczyło tej pięciolinii dla wszystkich nut?
                if (cursor == -1)
                {                    
                    // nie - trzeba dodać kolejną pieciolinie
                    return false;
                }
            }

            return true;
        }

        // Rysuje klamrę spinającą pięciolinię wiolinową i basową.
        private double ShowBrace(Score score)
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
                    (top + marginTop) * score.Magnification - braceOffsetY, 
                    0, 
                    0);
            score.canvas.Children.Add(brace);

            // zwracam miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
            return braceWidth - braceOffestX;
        }
    }
}