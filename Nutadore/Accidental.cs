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
	public class Accidental : Sign
	{
		readonly SolidColorBrush keySignatureHintColor = Brushes.LightGray;

		public enum Type
		{
			None,
			Flat,
			Sharp,
			Natural
		}

		public Type type;
		public StaffPosition staffPosition;
		public Staff.Type? staffType;
		public bool isKeySignatureHint;

		public Accidental(Type type, StaffPosition statfPosition = null, Staff.Type? staffType = null)
		{
			this.type = type;
			this.staffPosition = statfPosition;
			this.staffType = staffType;
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			double right = left;

			string glyphCode;
			switch (type)
			{
				case Type.None:
					return left;
				case Type.Flat:
					glyphCode = "\x003a";
					break;
				case Type.Sharp:
					glyphCode = "\x002e";
					break;
				case Type.Natural:
					glyphCode = "\x0036";
					break;
				default:
					throw new NotImplementedException();
			};

			Staff[] staffs;
			if (staffType == null)
				staffs = new[] { trebleStaff, bassStaff };
			else if (staffType == Staff.Type.Treble)
				staffs = new[] { trebleStaff };
			else
				staffs = new[] { bassStaff };

			foreach (Staff staff in staffs)
			{
				double glyphTop = staff.StaffPositionToY(staffPosition);

				glyphTop -= 57 * score.Magnification;
				right = base.AddGlyphToScore(score, left, glyphTop, glyphCode);

				if (isKeySignatureHint)
				{
					TextBlock glyph = base.elements.FindLast(e => true) as TextBlock;
					glyph.Foreground = keySignatureHintColor;
				}
			}

			return right;
		}
	}
}
