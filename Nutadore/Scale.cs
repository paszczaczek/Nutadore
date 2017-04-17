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
		public enum Type { Major, Minor }

		private Note.Letter based;
		private Type type;
		Accidental[] accidentals;

		public Scale(Note.Letter based, Type type)
		{
			this.based = based;
			this.type = type;
			accidentals = CreateAccidentals();
		}

		public override double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left)
		{
			double signLeft = left;
			foreach (Accidental accidental in accidentals)
			{
				signLeft
					= accidental.AddToScore(score, trebleStaff, bassStaff, step, signLeft)
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

		public Accidental.Type AccidentalForLetter(Note.Letter letter)
		{
			foreach (Accidental accidental in accidentals)
			{
				StaffPosition staffPosition = StaffPosition.ByLine(1);
				switch (letter)
				{
					case Note.Letter.C:
						staffPosition = StaffPosition.ByLine(3, true);
						break;
					case Note.Letter.D:
						staffPosition = StaffPosition.ByLine(4);
						break;
					case Note.Letter.E:
						staffPosition = StaffPosition.ByLine(4, true);
						break;
					case Note.Letter.F:
						staffPosition = StaffPosition.ByLine(5);
						break;
					case Note.Letter.G:
						staffPosition = StaffPosition.ByLine(5, true);
						break;
					case Note.Letter.A:
						staffPosition = StaffPosition.ByLine(2, true);
						break;
					case Note.Letter.H:
						staffPosition = StaffPosition.ByLine(3);
						break;
				}
				if (accidental.staffPosition.CompareTo(staffPosition) == 0)
					return accidental.type;
			}

			return Accidental.Type.None;
		}

		//public override void HightlightRectangle_MouseLeave(object sender, MouseEventArgs e)
		//{
		//	foreach (Accidental accidental in accidentals)
		//		accidental.HightlightRectangle_MouseLeave(sender, e);
		//}

		//public override void HighlightRectangle_MouseEnter(object sender, MouseEventArgs e)
		//{
		//	foreach (Accidental accidental in accidentals)
		//		accidental.HighlightRectangle_MouseEnter(sender, e);
		//}

		private Accidental[] CreateAccidentals()
		{
			switch (type)
			{
				case Type.Major:
					switch (based)
					{
						case Note.Letter.C:
							return new Accidental[] { };
						case Note.Letter.D:
							return new Accidental[]
							{
								new Accidental(Accidental.Type.Sharp, StaffPosition.ByLine(5)),
								new Accidental(Accidental.Type.Sharp, StaffPosition.ByLine(3, true))
							};
						case Note.Letter.E:
							throw new System.NotImplementedException();
						case Note.Letter.F:
							throw new System.NotImplementedException();
						case Note.Letter.G:
							throw new System.NotImplementedException();
						case Note.Letter.A:
							throw new System.NotImplementedException();
						case Note.Letter.H:
							throw new System.NotImplementedException();
					}
					break;
				case Type.Minor:
					throw new System.NotImplementedException();
			}
			throw new System.NotImplementedException();
			//return new Accidental[] {
			//	new Accidental(StaffPosition.ByLine(5)),
			//	new Accidental(StaffPosition.ByLine(3, true)),
			//	new Accidental(StaffPosition.ByLine(5, true)),
			//	new Accidental(StaffPosition.ByLine(4)),
			//	new Accidental(StaffPosition.ByLine(2, true)),
			//	new Accidental(StaffPosition.ByLine(4, true)),
			//	new Accidental(StaffPosition.ByLine(3))
			//};
		}
	}
}
