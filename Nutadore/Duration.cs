using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
	public class Duration
	{
		public enum Value
		{
			Whole,
			Half,
			Quarter,
			Eighth,
			Sixteenth,
			ThirtySecond
		}
		public Value value = Value.Quarter;
		public bool dotted = false;

		public double ToDouble()
		{
			double val;
			switch (value)
			{
				case Value.Whole: val = 4; break;
				case Value.Half: val = 2; break;
				case Value.Quarter: val = 1; break;
				case Value.Eighth: val = 1.0 / 2; break;
				case Value.Sixteenth: val = 1.0 / 4; break;
				case Value.ThirtySecond: val = 1.0 / 8; break;
				default:
					throw new Exception();
			}
			if (dotted)
				val = val * 1.5;

			return val;
		}
	}
}
