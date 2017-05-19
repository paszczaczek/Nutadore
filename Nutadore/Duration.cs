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
			Whole,
			Half,
			Quarter,
			Eighth,
			Sixteenth,
			ThirtySecond
		}
		public Name name = Name.Quarter;
		public bool dotted = false;

		public double ToDouble()
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
	}
}
