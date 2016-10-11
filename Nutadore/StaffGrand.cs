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
		private Clef clef;
		private double cursor;
		public Sign firstSign;
		public Sign lastSign;
		private List<Perform> performs = new List<Perform>();

		public StaffGrand(Score score, double top)
		{
			this.score = score;
			this.top = top;
		}

		public double Show(Sign fromSign, out Sign nextSign)
		{
			// Rysuję klamrę.
			double braceRight = ShowBrace();

			// Rysuję pięciolinie wiolinową i basową, klucze i znaki przykluczowe.
			double bottom = ShowStaffs(braceRight);

			// Rysuję nuty i inne znaki - tyle ile sie zmiesci.
			nextSign = ShowSigns(fromSign);

			// Rysuję znaki zmiany wysokości wykonania (ottava)
			ShowPerform(Staff.Type.Bass);
			ShowPerform(Staff.Type.Treble);

			// Zwracam pierwszą nutę do narysowania na następnym StaffGrand
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
			score.Children.Add(brace);

			// wyznaczem miejsce w którym kończy sie klamra i będa rozpoczynały sie pięciolinie
			double braceRight = braceWidth - braceOffestX;
			return braceRight;
		}

		private double ShowStaffs(double left)
		{
			// Rysuję pięciolinię wiolinową.
			double trebleStaffTop = top + spaceAboveTrebleStaff;
			trebleStaff = new Staff(score, Staff.Type.Treble, left, trebleStaffTop);
			trebleStaff.Show();

			// Rysuję pięciolinię basową.
			double bassStaffTop
				= trebleStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBetweenTrebleAndBassStaff;
			bassStaff = new Staff(score, Staff.Type.Bass, left, bassStaffTop);
			bassStaff.Show();

			// Rysuję klucz wiolinowy i basowy.
			clef = new Clef();
			cursor = clef.Show(score, trebleStaff, bassStaff, left);

			// Rysuję znaki przykluczowe wynikające z tonacji.
			cursor = score.scale.Show(score, trebleStaff, bassStaff, cursor);

			// Wyznaczam dolną krawędź StaffGrand i zwracam ją.
			double bottom
				= bassStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBelowBassStaff;

			return bottom;
		}

		private Sign ShowSigns(Sign fromSign)
		{
			// Może nie być żadnych znaków do rysowania.
			if (fromSign == null)
				return null;

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
					int idxBar = HideToBeginOfMeasure(sign);
					if (idxBar == -1)
					{
						// Nie znaleziono znaku taktu. Nie robimy wycofywania.
						return sign;
					}
					lastSign = score.signs[idxBar];
					Sign nextSign = score.signs[idxBar + 1];
					return nextSign;
				}
				else
				{
					// Tak - zmieścił się.
					lastSign = sign;
				}
			}

			// Wszystkie zmieściły się.
			return null;
		}

		private void ShowPerform(Staff.Type staffType)
		{
			if (score.signs.Count == 0)
				return;

			Perform.HowTo performHowTo = Perform.HowTo.AtPlace;
			double performLeft = 0;
			double performRight = 0;

			int idxFirst = score.signs.FindIndex(sign => sign == firstSign);
			int idxLast = score.signs.FindIndex(sign => sign == lastSign);
			for (int idx = idxFirst; idx <= idxLast; idx++)
			{
				Sign sign = score.signs[idx];

				Perform.HowTo singPerformHowTo;
				double signLeft;
				double signRight;
				if (sign is Chord)
				{
					Chord chord = sign as Chord;
					singPerformHowTo = staffType
						== Staff.Type.Treble
						? chord.performHowToStaffTreble
						: chord.performHowToStaffBass;
					signLeft = chord.left;
					signRight = chord.right;
				}
				else if (sign is Note)
				{
					Note note = sign as Note;
					if (note.staffType != staffType)
						continue;
					singPerformHowTo = note.performHowTo;
					signLeft = note.left;
					signRight = note.right;
				}
				else
				{
					continue;
				}


				bool performChanged = singPerformHowTo != performHowTo;
				bool performNew = performChanged && singPerformHowTo != Perform.HowTo.AtPlace;
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
					performHowTo = singPerformHowTo;
					performLeft = signLeft;
				}
				performRight = signRight;
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

		private int HideToBeginOfMeasure(Sign fromSign)
		{
			List<Sign> signsForHide = new List<Sign>();

			for (int idx = score.signs.IndexOf(fromSign) - 1; idx >= 0 ; idx--)
			{
				Sign sign = score.signs[idx];
				if (sign is Bar)
				{
					// Jest znak taktu. Ukrywamy znaki i zwracamy jego index.
					signsForHide.ForEach(s => s.Hide(score));
					return idx;
				}
				else
				{
					signsForHide.Add(sign);
				}
			}

			// Nie znaleziono znaku taktu.
			return -1;
		}
	}
}