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
        static private readonly double spaceAboveTrebleStaff = Staff.spaceBetweenLines * 8;
        static private readonly double spaceBetweenTrebleAndBassStaff = Staff.spaceBetweenLines * 7;
        static private readonly double spaceBelowBassStaff = Staff.spaceBetweenLines * /*7*/5;

		private Score score;
		private double top;
		public double bottom { get; private set; }
		public bool allSignsFitted { get; private set; }
        private Staff trebleStaff;
        private Staff bassStaff;
        private double cursor;

		public StaffGrand(Score score, double top)
		{
			this.score = score;
			this.top = top;

			// Rysuję klamrę.
			ShowBrace();

			// Rysuję pięciolinie wiolinową i basową.
			ShowStaffs();

			// Rysuję nienarysowane jeszcze na piecilinii nuty i inne znaki.
			ShowNotShownSigns();

			// Rysuję te elementy których nie można było dokończyć, np. ottvy
			trebleStaff.ShowPerformSign(score);
			bassStaff.ShowPerformSign(score);
		}

		private void ShowBrace()
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

            // wyznaczem miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
            cursor = braceWidth - braceOffestX;
        }

		private void ShowStaffs()
		{
			// Rysuję pięciolinię wiolinową.
			trebleStaff = new Staff(Staff.Type.Treble);
			double trebleStaffTop = top + spaceAboveTrebleStaff;
			double trebleStaffRight = trebleStaff.Show(score, cursor, trebleStaffTop);

			// Rysuję pięciolinię basową.
			bassStaff = new Staff(Staff.Type.Bass);
			double bassStaffTop
				= trebleStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBetweenTrebleAndBassStaff;
			double bassStaffRight = bassStaff.Show(score, cursor, bassStaffTop);

			// Wyznaczam dolną krawędź StaffGrand.
			bottom
				= bassStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBelowBassStaff;

			// Wyznaczam połoznenie kursora na StaffGrand.
			cursor
				= trebleStaffRight > bassStaffRight
				? trebleStaffRight
				: bassStaffRight;
		}

		private void ShowNotShownSigns()
		{
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
					// Nie - umieścimy go na kolejnym SraffGrand.
					// Wycofujemy też wszyskie znaki do początku taktu.
					HideToBeginOfMeasure(sign);
					allSignsFitted = false;

					return;
				}
			}

			// Wszystkie znaki zmieściły się.
			allSignsFitted = true;
		}

		private void HideToBeginOfMeasure(Sign fromSign)
		{
			for (int idx = score.signs.IndexOf(fromSign); idx >= 0 ; idx--)
			{
				Sign sign = score.signs[idx];
				if (!(sign is Bar))
					sign.Hide(score);
				else
					break;
			}
		}
	}
}