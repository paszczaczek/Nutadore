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
        private Staff trebleStaff;
        private Staff bassStaff;
        private double cursor;
		public Sign firstSign;
		public Sign lastSign;
		private List<Perform> performs = new List<Perform>();

		public StaffGrand(Score score, double top)
		{
			this.score = score;
			this.top = top;
		}

		public double Show(Sign fromSign)
		{
			// Rysuję klamrę.
			double braceRight = ShowBrace();

			// Rysuję pięciolinie wiolinową i basową.
			double bottom = ShowStaffs(braceRight);

			// Rysuję nienarysowane jeszcze na piecilinii nuty i inne znaki.
			ShowSigns(fromSign);

			// Rysuję znaki zmiany wysokości wykonania (ottava)
			ShowPerform();

			// Zwracam dolną krawędź StaffGrand.
			return bottom;
		}

		private double ShowBrace()
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
            double braceRight = braceWidth - braceOffestX;
			return braceRight;
        }

		private double ShowStaffs(double left)
		{
			// Rysuję pięciolinię wiolinową.
			double trebleStaffTop = top + spaceAboveTrebleStaff;
			trebleStaff = new Staff(score, Staff.Type.Treble, left, trebleStaffTop);
			double trebleStaffRight = trebleStaff.Show();

			// Rysuję pięciolinię basową.
			double bassStaffTop
				= trebleStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBetweenTrebleAndBassStaff;
			bassStaff = new Staff(score, Staff.Type.Bass, left, bassStaffTop);
			double bassStaffRight = bassStaff.Show();

			// Wyznaczam połoznenie kursora na StaffGrand.
			cursor
				= trebleStaffRight > bassStaffRight
				? trebleStaffRight
				: bassStaffRight;

			// Wyznaczam dolną krawędź StaffGrand i zwracam ją.
			double bottom
				= bassStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBelowBassStaff;

			return bottom;
		}

		private void ShowSigns(Sign fromSign)
		{
			firstSign = fromSign;
			for (int idx = score.signs.IndexOf(fromSign); idx < score.signs.Count; idx++)
			{
				Sign sign = score.signs[idx];
				cursor = sign.Show(score, trebleStaff, bassStaff, cursor);

				// Czy znak zmieścil się na pieciolinii?
				if (cursor == -1)
				{
					// Nie - umieścimy go na kolejnym SraffGrand.
					// Wycofujemy też wszyskie znaki do początku taktu.
					lastSign = HideToBeginOfMeasure(sign);
					break;
				}
				else
				{
					// Tak - zmieścił się.
					lastSign = sign;
				}
			}
		}

		private void ShowPerform()
		{
			Perform.HowTo performHowTo = Perform.HowTo.AtPlace;
			double performLeft = 0;
			double performRight = 0;

			int idxFirst = score.signs.IndexOf(firstSign);
			int idxLast = score.signs.IndexOf(lastSign);
			for (int idx = idxFirst; idx <= idxLast; idx++)
			{
				Sign sign = score.signs[idx];
				if (!(sign is Note))
					continue;

				Note note = sign as Note;

				bool performChanged = note.performHowTo != performHowTo;
				bool performNew = performChanged && note.performHowTo != Perform.HowTo.AtPlace;
				bool performEnd = performChanged && performHowTo != Perform.HowTo.AtPlace;
				if (performEnd)
				{
					// Tu wystąpił koniec znaku zmiany wysokości. Rysuję go.
					// TODO: konstruktor+show() ?
					Perform perform = new Perform(performHowTo, performLeft, performRight);
					performs.Add(perform);
					perform.Show(score, trebleStaff, bassStaff);
					performHowTo = Perform.HowTo.AtPlace;
				}
				if (performNew)
				{
					// Tu wystąpił początek znaku zmiany wysokości. Zapamiętuję jego położnie i wysokość.
					performHowTo = note.performHowTo;
					performLeft = note.left;
				}
				performRight = note.right;
			}


			if (performHowTo != Perform.HowTo.AtPlace)
			{
				// Tu wystąpił koniec znaku zmiany wysokości. Rysuję go.
				// TODO: konstruktor+show() ?
				Perform perform = new Perform(performHowTo, performLeft, performRight);
				performs.Add(perform);
				perform.Show(score, trebleStaff, bassStaff);
				performHowTo = Perform.HowTo.AtPlace;
			}
		}

		public void Hide()
		{
			foreach(var perform in performs)
				perform.Hide(score);

			performs.Clear();
		}

		private Sign HideToBeginOfMeasure(Sign fromSign)
		{
			for (int idx = score.signs.IndexOf(fromSign) - 1; idx >= 0 ; idx--)
			{
				Sign sign = score.signs[idx];
				if (sign is Bar)
					return sign;
				else
					sign.Hide(score);
			}

			return null;
		}
	}
}