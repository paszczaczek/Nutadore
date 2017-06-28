using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
	public class Clef : Sign
	{
		public Clef()
		{
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			double maxRightClef = left;
			foreach (Staff staff in new[] { trebleStaff, bassStaff })
			{
				string glyphCode = null;
				double glyphTop = 0; //= staff.StaffPositionToY(StaffPosition.ByLine(2));
				switch (staff.type)
				{
					case Staff.Type.Treble:
						glyphCode = "\x00c9";
						glyphTop
							= staff.StaffPositionToY(StaffPosition.ByLine(2))
							- base.GlyphFormatedText(score, glyphCode).Baseline;
						break;
					case Staff.Type.Bass:
						glyphCode = "\x00c7";
						glyphTop
							= staff.StaffPositionToY(StaffPosition.ByLine(4))
							- base.GlyphFormatedText(score, glyphCode).Baseline;
						break;
				}

				double rightClef = base.AddGlyphToScore(score, left, glyphTop, glyphCode);
				//Rect boundaryClef = base.AddFetaGlyph(score, left, glyphTop, glyphCode);
				//double rightClef = boundaryClef.Left + boundaryClef.Width;
				if (rightClef > maxRightClef)
					maxRightClef = rightClef;
			}

			double right = maxRightClef + Staff.spaceBetweenSigns * score.Magnification;

			return right;
		}
	}
}
