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
			switch (value)
			{
				case Value.Whole: return 4;
				case Value.Half: return 2;
				case Value.Quarter: return 1;
				case Value.Eighth: return 1.0 / 2;
				case Value.Sixteenth: return 1.0 / 4;
				case Value.ThirtySecond: return 1.0 / 8;
				default:
					throw new Exception();
			}
		}
	}
}
