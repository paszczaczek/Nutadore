using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    /// <summary>
    /// Znak tak zwanej ottavy czyli pozioma linia zakończona krótka pionową kreseczką
    /// umieszczana nad lub pod pięciolinią informująca że nutę trzeba zagrać jedną lub więcej
    /// oktaw wyżej lub niżej.
    /// </summary>
	public class Perform : Sign
	{
		private double left;
		private double right;
		private HowTo howTo;

		public enum HowTo
		{
			AtPlace,
			OneOctaveHigher,
			OneOctaveLower,
			TwoOctaveHigher
		}

		public Perform(HowTo howTo, double left, double right)
		{
			this.howTo = howTo;
			this.left = left;
			this.right = right;
		}

		public void Show(Score score, Staff trebleStaff, Staff bassStaff)
		{
			const double height = 7;

			// Czy trzeba coś dorysować?
			double y;
			double vertLineTop, verLineBottom;
			double textTop;
			string text;
			switch (howTo)
			{
				case HowTo.TwoOctaveHigher:
					// Tak, trzeba dorysować znak 15ma
					y = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(3));
					vertLineTop = y;
					verLineBottom = y + height;
					textTop = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(5));
					text = "15ma";
					break;
				case HowTo.OneOctaveHigher:
					// Tak, trzeba dorysować znak 8va.
					y = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(6));
					vertLineTop = y;
					verLineBottom = y + height;
					textTop = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(8));
					text = "8va";
					break;
				case HowTo.OneOctaveLower:
					// Tak, trzeba dorysować znak 8vb.
					y = bassStaff.StaffPositionToY(StaffPosition.ByLegerBelow(4));
					vertLineTop = y - height;
					verLineBottom = y;
					textTop = bassStaff.StaffPositionToY(StaffPosition.ByLegerBelow(4));
					text = "8vb";
					break;
				case HowTo.AtPlace:
				default:
					// Nie trzeba nic rysować.
					return;
			}

			// Rysuję linię poziomą.
			double delta = Staff.spaceBetweenSigns / 4 * score.Magnification;
			Line horizontalLine = new Line
			{
				X1 = left - delta,
				X2 = right + delta,
				Y1 = y,
				Y2 = y,
				Stroke = Brushes.Black,
				StrokeDashArray = new DoubleCollection { 7, 7 },
				StrokeThickness = 0.5
			};
			base.AddElement(score, horizontalLine);

			// Rysuję linię pionową.
			Line verticalLine = new Line
			{
				X1 = right + delta,
				X2 = right + delta,
				Y1 = vertLineTop,
				Y2 = verLineBottom,
				Stroke = Brushes.Black,
				StrokeThickness = 0.5
			};
			base.AddElement(score, verticalLine);

			// Rysuję oznaczenie 8va, 8vb lub 15ma
			Label name = new Label
			{
				//name.FontFamily = new FontFamily(familyName),
				FontSize = 12 * score.Magnification,
				Content = text,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(left - delta, textTop, 0, 0)
			};
			base.AddElement(score, name);
		}
	}
}
