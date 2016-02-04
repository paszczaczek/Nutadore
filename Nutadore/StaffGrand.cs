using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    public class StaffGrand
    {
        static private readonly double spaceAboveTrebleStaff = Staff.spaceBetweenLines * 8;
        static private readonly double spaceBetweenTrebleAndBassStaff = Staff.spaceBetweenLines * 7;
        static private readonly double spaceBelowBassStaff = Staff.spaceBetweenLines * /*7*/5;

        private double top;
        private Staff trebleStaff;
        private Staff bassStaff;
        private double cursor;

        // Rysuje pięciolinie i klamrę je spinającą.
        public bool Show(Score score, double top, out double bottom)
        {
            this.top = top;

            // Rysuję klamrę.
            cursor = ShowBrace(score);

            // Rysuję pięciolinię wiolinową.
            trebleStaff = new Staff(Staff.Type.Treble);
            double trebleStaffTop = top + spaceAboveTrebleStaff;
            double trebleStaffCursor = trebleStaff.Show(score, cursor, trebleStaffTop);

            // Rysuję pięciolinię basową.
            bassStaff = new Staff(Staff.Type.Bass);
            double bassStaffTop 
                = trebleStaffTop
                + Staff.spaceBetweenLines * 4 
                + spaceBetweenTrebleAndBassStaff;
            double bassStaffCursor = bassStaff.Show(score, cursor, bassStaffTop);

            // Wyznaczam dolną krawędź StaffGrand.
            bottom
                = bassStaffTop
                + Staff.spaceBetweenLines * 4
                + spaceBelowBassStaff;

            // Wyznaczam połoznenie kursora na StaffGrand.
            cursor 
                = trebleStaffCursor > bassStaffCursor
                ? trebleStaffCursor
                : bassStaffCursor;

            // Rysuję nienarysowane jeszcze na piecilinii nuty i inne znaki.
            List<Sign> signsNotShown = score.signs.FindAll(sign => !sign.IsShown);
            foreach (Sign sign in signsNotShown)
            {
                if (sign is Chord)
                {
                    // Dla akrodu rysuję poczczególne nuty na właściwej pięciolinii
                    Chord chord = sign as Chord;
                    cursor = chord.Show(score, trebleStaff, bassStaff, cursor);
                }
                else if (sign is Bar)
                {
                    // Linie taktów muszą być na obu pięcioliniach.
                    trebleStaff.ShowSign(score, sign, cursor);
                    cursor = bassStaff.ShowSign(score, sign, cursor);
                }
                else if (sign is Note)
                {
                    Note note = sign as Note;
                    // Pozostałe znaki na jednej pięciolinii.
                    Staff staff
                        = note.staffType == Staff.Type.Treble
                        ? trebleStaff
                        : bassStaff;
                    cursor = staff.ShowSign(score, sign, cursor);
                }

                // Czy znak zmieścil się na pieciolinii?
                if (cursor == -1)
                {                    
                    // nie - trzeba go będzie umieścić na kolejnym SraffGrand
                    return false;
                }
            }

            // Narysuj te elementy które nie można było dokończyć jak ottvy
            trebleStaff.ShowPerformSign(score);
            bassStaff.ShowPerformSign(score);

            // Wszystkie znaki zmieściły sie na tym StaffGrand
            return true;
        }

        // Rysuje klamrę spinającą pięciolinię wiolinową i basową.
        private double ShowBrace(Score score)
        {
            // wyznaczam wymiary klamry
            string familyName = "MS Mincho";
            //double fontSize = 116 * score.Magnification;
            double fontSize = 176 * score.Magnification;
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
                    (top + spaceAboveTrebleStaff) * score.Magnification - braceOffsetY, 
                    0, 
                    0);
            score.canvas.Children.Add(brace);

            // zwracam miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
            return braceWidth - braceOffestX;
        }
    }
}