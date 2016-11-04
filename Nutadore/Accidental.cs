using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Nutadore
{
	public class Accidental : Sign
	{
		public enum Type
		{
			None,
			Flat,
			Sharp,
			Natural
		}

		public StaffPosition staffPosition;

		public Accidental(StaffPosition statfPosition)
		{
			this.staffPosition = statfPosition;
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			double right = left;

			string glyphCode = "\x002e";
			foreach (Staff staff in new[] { trebleStaff, bassStaff })
			{
				double glyphTop = staff.StaffPositionToY(staffPosition);

				glyphTop -= 57 * score.Magnification;
				right = base.AddGlyph(score, left, glyphTop, glyphCode);
			}

			return right;
		}
	}
}
