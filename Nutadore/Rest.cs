using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Rest : Sign, IDuration
	{
		public Duration duration { get; set; } = new Duration();

		public Rest(Duration duration)
		{
			this.duration = duration;
		}


		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			double right = left;

			// TODO
			// Na której pięciolinii ma być umieszczona pauza?
			Staff.Type staffType = Staff.Type.Treble;
			Staff staff
				= staffType == Staff.Type.Treble
				? trebleStaff
				: bassStaff;
			StaffPosition staffPosition = StaffPosition.ByLegerBelow(4);

			// Rysujemy znakk pauzy.
			string glyphCode = "\x0027";
			double glyphTop
					= staff.top * score.Magnification
					 + (4 - staffPosition.Number) * Staff.spaceBetweenLines * score.Magnification;
			glyphTop -= 57.5 * score.Magnification;
			double glyphLeft = right;
			right = base.AddGlyphToScore(score, glyphLeft, glyphTop, glyphCode, 1);

			// Czy znak zmieścił sie na pięcolinii?
			if (right >= score.ActualWidth - Staff.marginLeft)
			{
				// Nie zmieścił się - narysujemy ją na następnej pieciolinii.
				RemoveFromScore(score);
				return -1;
			}
			else
			{
				// Znak zmieścił sie na pięciolinii.
				return right + Staff.spaceBetweenSigns * score.Magnification;
			}
		}

		public override void RemoveFromScore(Score score)
		{
		}

		public override string ToString()
		{
			return "r" + duration.ToString();
		}
	}	
}
