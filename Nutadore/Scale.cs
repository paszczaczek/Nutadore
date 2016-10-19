using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Scale : Sign
	{
		private Note.Letter based;
		private Type type;
		Accidental[] accidentals;

		public Scale(Note.Letter based, Type type)
		{
			this.based = based;
			this.type = type;
			accidentals = Accidentals();
		}

		public enum Type { Major, Minor }

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			double signLeft = left;
			foreach (Accidental accidental in accidentals)
			{
				signLeft
					= accidental.AddToScore(score, trebleStaff, bassStaff, signLeft)
					+ Staff.spaceBetweenScaleSigns * score.Magnification;
				base.ExtendBounds(accidental.bounds);
			}

			double scaleRight = signLeft + Staff.spaceBetweenSigns;

			return scaleRight;
		}

		public override void RemoveFromScore(Score score)
		{
			base.RemoveFromScore(score);
			foreach (Accidental accidental in accidentals)
				accidental.RemoveFromScore(score);
		}

		public override void HightlightRectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (Accidental accidental in accidentals)
				accidental.HightlightRectangle_MouseLeave(sender, e);
		}

		public override void HighlightRectangle_MouseEnter(object sender, MouseEventArgs e)
		{
			foreach (Accidental accidental in accidentals)
				accidental.HighlightRectangle_MouseEnter(sender, e);
		}

		public Accidental[] Accidentals()
		{
			return new Accidental[] {
				new Accidental(StaffPosition.ByLine(5)),
				new Accidental(StaffPosition.ByLine(3, true)),
				new Accidental(StaffPosition.ByLine(5, true)),
				new Accidental(StaffPosition.ByLine(4)),
				new Accidental(StaffPosition.ByLine(2, true)),
				new Accidental(StaffPosition.ByLine(4, true)),
				new Accidental(StaffPosition.ByLine(3))
			};
		}
	}
}
