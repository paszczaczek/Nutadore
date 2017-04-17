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
		private Step firstStep;
		public Step lastStep { get; private set; }
		private List<Perform> performs = new List<Perform>();

		public StaffGrand(Score score, double top)
		{
			this.score = score;
			this.top = top;
		}

		public double AddSteps(Step fromStep, out Step nextStep)
		{
			// Dodaję klamrę.
			double braceRight = AddBraceToScore();

			// Dodaję pięciolinie wiolinową i basową, klucze i znaki przykluczowe.
			double bottom = AddStaffsToScore(braceRight);

			// Dodaję kroki - tyle ile sie zmiesci.
			nextStep = AddStepsToScore(fromStep);

			// Dodaję znaki zmiany wysokości wykonania (ottava)
			AddPerformToScore(Staff.Type.Bass);
			AddPerformToScore(Staff.Type.Treble);

			// Zwracam pierwszą nutę do narysowania na następnym StaffGrand
			return bottom;
		}

		private double AddBraceToScore()
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

		private double AddStaffsToScore(double left)
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
			cursor = clef.AddToScore(score, trebleStaff, bassStaff, null, left);

			// Rysuję znaki przykluczowe wynikające z tonacji.
			cursor = score.scale.AddToScore(score, trebleStaff, bassStaff, null, cursor);

			// Wyznaczam dolną krawędź StaffGrand i zwracam ją.
			double bottom
				= bassStaffTop
				+ Staff.spaceBetweenLines * 4
				+ spaceBelowBassStaff;

			return bottom;
		}

		private Step AddStepsToScore(Step fromStep)
		{
			// Może nie być żadnych znaków do rysowania.
			if (fromStep == null)
				return null;

			firstStep = fromStep;
			for (int idx = score.steps.IndexOf(fromStep); idx < score.steps.Count; idx++)
			{
				Step step = score.steps[idx];
				cursor = step.AddToScore(score, trebleStaff, bassStaff, cursor);

				// Czy znak zmieścil się na pieciolinii?
				if (cursor == -1)
				{
					// Nie - umieścimy go na kolejnym SraffGrand.
					// Wycofujemy też wszyskie znaki do początku taktu.
					int idxBar = RemoveToBeginOfMeasure(step);
					if (idxBar == -1)
					{
						// Nie znaleziono znaku taktu. Nie robimy wycofywania.
						return step;
					}
					lastStep = score.steps[idxBar];
					Step nextStep = score.steps[idxBar + 1];
					return nextStep;
				}
				else
				{
					// Tak - zmieścił się.
					lastStep = step;
				}
			}

			// Wszystkie zmieściły się.
			return null;
		}

		private void AddPerformToScore(Staff.Type staffType)
		{
			if (score.steps.Count == 0)
				return;

			Perform.HowTo performHowTo = Perform.HowTo.AtPlace;
			double performLeft = 0;
			double performRight = 0;

			int idxFirst = score.steps.FindIndex(sign => sign == firstStep);
			int idxLast = score.steps.FindIndex(sign => sign == lastStep);
			for (int idx = idxFirst; idx <= idxLast; idx++)
			{
				Step step = score.steps[idx];

				Perform.HowTo stepPerformHowTo;
				double stepLeft = step.bounds.Left;
				double stepRight = step.bounds.Right;
				stepPerformHowTo = staffType
					== Staff.Type.Treble
					? step.performHowToStaffTreble
					: step.performHowToStaffBass;

				bool performChanged = stepPerformHowTo != performHowTo;
				bool performNew = performChanged && stepPerformHowTo != Perform.HowTo.AtPlace;
				bool performEnd = performChanged && performHowTo != Perform.HowTo.AtPlace;
				if (performEnd)
				{
					// Tu wystąpił koniec znaku zmiany wysokości. Rysuję go.
					// TODO: konstruktor+show() ?
					Perform perform = new Perform(performHowTo, performLeft, performRight);
					performs.Add(perform);
					perform.AddToScore(score, trebleStaff, bassStaff, null, 0);
					performHowTo = Perform.HowTo.AtPlace;
				}
				if (performNew)
				{
					// Tu wystąpił początek znaku zmiany wysokości. Zapamiętuję jego położnie i wysokość.
					performHowTo = stepPerformHowTo;
					performLeft = stepLeft;
				}
				performRight = stepRight;
			}


			if (performHowTo != Perform.HowTo.AtPlace)
			{
				// Tu wystąpił koniec znaku zmiany wysokości. Rysuję go.
				// TODO: konstruktor+show() ?
				Perform perform = new Perform(performHowTo, performLeft, performRight);
				performs.Add(perform);
				perform.AddToScore(score, trebleStaff, bassStaff, null, 0);
				performHowTo = Perform.HowTo.AtPlace;
			}
		}

		public void RemoveFromScore()
		{
			foreach(var perform in performs)
				perform.RemoveFromScore(score);

			performs.Clear();
		}

		private int RemoveToBeginOfMeasure(Step fromStep)
		{
			List<Step> stepsForRemove = new List<Step>();

			for (int idx = score.steps.IndexOf(fromStep) - 1; idx >= 0 ; idx--)
			{
				Step step = score.steps[idx];
				if (step.IsBar)
				{
					// Jest znak taktu. Ukrywamy znaki i zwracamy jego index.
					stepsForRemove.ForEach(s => s.RemoveFromScore(score));
					return idx;
				}
				else
				{
					stepsForRemove.Add(step);
				}
			}

			// Nie znaleziono znaku taktu.
			return -1;
		}
	}
}