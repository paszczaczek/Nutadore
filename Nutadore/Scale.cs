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

		public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			double signLeft = left;
			foreach (Accidental accidental in accidentals)
			{
				signLeft
					= accidental.Show(score, trebleStaff, bassStaff, signLeft)
					+ Staff.spaceBetweenScaleSigns * score.Magnification;
				base.ExtendBounds(score, accidental.bounds);
			}

			double scaleRight = signLeft + Staff.spaceBetweenSigns;

			return scaleRight;
		}

		public override void Hide(Score score)
		{
			base.Hide(score);
			foreach (Accidental accidental in accidentals)
				accidental.Hide(score);
		}

		public override void MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (Accidental accidental in accidentals)
				accidental.MouseLeave(sender, e);
		}

		public override void MouseEnter(object sender, MouseEventArgs e)
		{
			foreach (Accidental accidental in accidentals)
				accidental.MouseEnter(sender, e);
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
