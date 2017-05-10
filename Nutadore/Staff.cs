using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Staff
	{
		static public readonly double marginLeft = 10;
		static public readonly double spaceBetweenLines = 10;
		static public readonly double spaceBetweenScaleSigns = 1;
		static public readonly double spaceBetweenSigns = 0;//5;//15;

		static private readonly Brush helperLineBrush = Brushes.Red;

		private Score score;
		public Type type;
		private double left;
		public double top;

		public enum Type
		{
			Treble = 0,
			Bass
		}

		public Staff(Score score, Type type, double left, double top)
		{
			this.score = score;
			this.type = type;
			this.left = left;
			this.top = top;
		}

		public double Show()
		{
			// Dodaję pięciolinię.
			ShowLines();

			// Dodaję pomocnicze kolorowe linie.
			ShowHelperLines();

			return left;
		}

		private void ShowLines()
		{
			for (var staffPosition = StaffPosition.ByLine(1);
				staffPosition <= StaffPosition.ByLine(5);
				staffPosition.AddLine(1))
			{
				double y = StaffPositionToY(staffPosition);
				Line shapeLine = new Line
				{
					X1 = left,
					X2 = score.ActualWidth - marginLeft,
					Y1 = y,
					Y2 = y,
					Stroke = Brushes.Black,
					StrokeThickness = 1
				};
				score.Children.Add(shapeLine);
			}
		}

		private void ShowHelperLines()
		{
			for (var octave = Note.Octave.Great; octave <= Note.Octave.ThreeLined; octave++)
			{
				var note = new Note(Note.Letter.C, Accidental.Type.None, octave);
				var staffPosition = note.ToStaffPosition(type, false);
				if (note.staffType != type)
					continue;
				double y = StaffPositionToY(staffPosition);
				// Rysuję linie pomocniczą.
				Line helperLine = new Line
				{
					X1 = left,
					X2 = score.ActualWidth - marginLeft,
					Y1 = y,
					Y2 = y,
					Stroke = helperLineBrush,
					StrokeThickness = 0.4
				};
				score.Children.Add(helperLine);
				// Rusuje nazwę oktawy.
				const string familyName = "Consolas";
				double fontSize = 9 * score.Magnification;
				FormattedText formattedText = new FormattedText(
					//octave.ToString(),
					Note.OctaveToString(octave),
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(familyName),
					fontSize,
					Brushes.Black);
				TextBlock octaveNameTextBlock = new TextBlock
				{
					FontFamily = new FontFamily(familyName),
					FontSize = fontSize,
					//Text = octave.ToString(),
					Text = Note.OctaveToString(octave),
					Foreground = helperLineBrush,
					Padding = new Thickness(0, 0, 0, 0),
					Margin = new Thickness(helperLine.X1, helperLine.Y1 - formattedText.Height/* + formattedText.OverhangAfter - formattedText.Extent*/, 0, 0)
				};
				score.Children.Add(octaveNameTextBlock);
				Canvas.SetZIndex(octaveNameTextBlock, 1);
			}
		}

		public double StaffPositionToY(StaffPosition staffPosition)
		{
			return
				// tu będzie piąta linia
				top * score.Magnification
				+ (4 - staffPosition.Number) * spaceBetweenLines * score.Magnification;
		}
	}
}