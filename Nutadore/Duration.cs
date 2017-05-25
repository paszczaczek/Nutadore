using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
	public class Duration
	{
		public enum Name
		{
			ThirtySecond,
			Sixteenth,
			Eighth,
			Quarter,
			Half,
			Whole
		}

		public Name name = Name.Quarter;
		public bool dotted = false;

		public Duration() { }

		public Duration(Name name, bool dotted = false)
		{
			this.name = name;
			this.dotted = dotted;
		}

		public int Fraction()
		{
			int fraction;
			switch (name)
			{
				case Name.Whole:
					fraction = 1;
					break;
				case Name.Half:
					fraction = 2;
					break;
				case Name.Quarter:
					fraction = 4;
					break;
				case Name.Eighth:
					fraction = 8;
					break;
				case Name.Sixteenth:
					fraction = 16;
					break;
				case Name.ThirtySecond:
					fraction = 32;
					break;
				default:
					throw new NotImplementedException();
			}
			if (dotted)
				fraction = (int)(fraction * 1.5);

			return fraction;
		}

		public int ContainsHowMuch(Duration tick)
		{
			//return tick.Fraction / Fraction;
			return (int)(Count() / tick.Count());
		}

		public double Count()
		{
			double val;
			switch (name)
			{
				case Name.Whole: val = 4; break;
				case Name.Half: val = 2; break;
				case Name.Quarter: val = 1; break;
				case Name.Eighth: val = 1.0 / 2; break;
				case Name.Sixteenth: val = 1.0 / 4; break;
				case Name.ThirtySecond: val = 1.0 / 8; break;
				default:
					throw new Exception();
			}
			if (dotted)
				val = val * 1.5;

			return val;
		}

		public override string ToString()
		{
			return string.Format("*{0}{1}",
				Fraction(),
				dotted ? "." : "");
		}
	}
}
