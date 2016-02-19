using System;

namespace Nutadore
{
    public class StaffPosition : IComparable
    {
        #region Publiczne konstruktory statyczne.
        static public StaffPosition ByLine(int lineNo, bool above = false)
        {
            CheckLineNo(lineNo, 1, 5, "linii");
            return new StaffPosition(Line.Base1 + lineNo - 1, above);
        }

        static public StaffPosition ByLegerAbove(int lineNo, bool above = false)
        {
            CheckLineNo(lineNo, 1, 8, "linii dodanej górnej");
            return new StaffPosition(Line.Base1 + 4 + lineNo, above);
        }

        static public StaffPosition ByLegerBelow(int lineNo, bool above = false)
        {
            CheckLineNo(lineNo, 1, 6, "linii dodanej dolnej");
            return new StaffPosition(Line.Base1 - lineNo, above);
        }

        static private void CheckLineNo(int lineNo, int lineNoMin, int lineNoMax, string lineName)
        {
            if (lineNo < lineNoMin || lineNo > lineNoMax)
                throw new ArgumentOutOfRangeException(
                    "line", 
                    lineNo, string.Format(
						"Numer {0} pieciolinii musi być liczba {1}..{2}", 
						lineName, lineNoMin, lineNoMax));
        }

#if false
		static public StaffPosition ByLineNumber(double lineNumber)
        {
            double lineNumberMin = (int)Line.LegerBelow6;
            double lineNumberMax = (int)Line.LegerAbove5 + 0.5;
            double remainder = lineNumber % 1;
            if (lineNumber < lineNumberMin ||
                lineNumber > lineNumberMax ||
                remainder != 0 && Math.Abs(remainder) != 0.5)
            {
                string message = string.Format(
                    "Numeracja wewnętrzna linii pięciolinii musi być liczą {0}..{1} podzielną przez 1 lub 0.5",
                    lineNumberMin, lineNumberMax);
                throw new ArgumentOutOfRangeException("number", lineNumber, message);
            }

            Line lineName;
            bool lineAbove;
            if (remainder == 0)
            {
                lineName = (Line)(int)lineNumber;
                lineAbove = false;
            }
            else if (remainder > 0)
            {
                lineName = (Line)(int)lineNumber;
                lineAbove = true;
            }
            else // remainder < 0
            {
                lineName = (Line)(int)(lineNumber - 1);
                lineAbove = true;
            }

            return new StaffPosition(lineName, lineAbove);
        }
#endif

		static public StaffPosition ByNumber(double number)
		{
			return new StaffPosition(number);
		}
        #endregion

        public enum Line
        {
            LegerBelow6 = -6,
            LegerBelow5 = -5,
            LegerBelow4 = -4,
            LegerBelow3 = -3,
            LegerBelow2 = -2,
            LegerBelow1 = -1,
            Base1 = 0,
            Base2 = 1,
            Base3 = 2,
            Base4 = 3,
            Base5 = 4,
            LegerAbove1 = 5,
            LegerAbove2 = 6,
            LegerAbove3 = 7,
            LegerAbove4 = 8,
            LegerAbove5 = 9
        }

        //private Line lineName;
        //private bool lineAbove;
        //public double LineNumber
        //{
        //    get
        //    {
        //        return (double)lineName + (lineAbove ? 0.5 : 0.0);
        //    }
        //    set
        //    {
        //        if (value > 0)
        //        {
        //            lineName = (Line)(int)value;
        //            lineAbove = (value % 1) == 0.5;
        //        }
        //        else
        //        {
        //            lineName = (Line)(int)(value - 0.5);
        //            lineAbove = (value % 1) == -0.5;
        //        }
        //    }
        //}

		double lineNumberMin = (int)Line.LegerBelow6;
		double lineNumberMax = (int)Line.LegerAbove5 + 0.5 + 3.0/* to 3.0 jest dla napisów 8va */;

		private double number;
		public double Number
		{
			set
			{
				double remainder = value % 1;
				if (value < lineNumberMin ||
					value > lineNumberMax ||
					remainder != 0 && Math.Abs(remainder) != 0.5)
				{
					string message = string.Format(
						"Numeracja wewnętrzna linii na pięciolinii musi być liczą {0}..{1} podzielną przez 1 lub 0.5",
						lineNumberMin, lineNumberMax);
					throw new ArgumentOutOfRangeException("number", value, message);
				}

				number = value;
			}
			get
			{
				return number;
			}
		}

		public Line LineName
		{
			get
			{
				double remainder = number % 1;
				if (remainder == 0)
				{
					return (Line)(int)number;
				}
				else if (remainder > 0)
				{
					return (Line)(int)number;
				}
				else // remainder < 0
				{
					return (Line)(int)(number - 1);
				}
			}
		}

		public bool LineAbove
		{
			get
			{
				double remainder = number % 1;
				if (remainder == 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}


		// Konstruktor prywatny
		private StaffPosition(Line lineName, bool lineAbove)
        {
            // this.lineName = lineName;
            // this.lineAbove = lineAbove;
			Number = (double)lineName + (lineAbove ? 0.5 : 0.0);
		}

		private StaffPosition(double number)
		{
			Number = number;
		}

		#region Funkcje do obsługi petli for()
		public int CompareTo(object obj)
        {
            StaffPosition staffPosition = obj as StaffPosition;

			if (Number == staffPosition.Number)
				return 0;
			else if (Number > staffPosition.Number)
				return 1;
			else
				return -1;
			/*
            if (this.lineName == staffPosition.lineName)
            {
                if (this.lineAbove == staffPosition.lineAbove)
                    return 0;
                if (this.lineAbove && !staffPosition.lineAbove)
                    return 1;
                return -1;
            }
            if (this.lineName > staffPosition.lineName)
                return 1;
            return -1;
			*/
        }

        static public bool operator <=(StaffPosition staffPosition1, StaffPosition staffPosition2)
        {
            return staffPosition1.CompareTo(staffPosition2) <= 0;
        }

        static public bool operator >=(StaffPosition staffPosition1, StaffPosition staffPosition2)
        {
            return staffPosition1.CompareTo(staffPosition2) >= 0;
        }

        public StaffPosition AddLine(int count)
        {
			//lineName += count;
			Number += count;
            return this;
        }

        public StaffPosition SubstractLine(int count)
        {
			//lineName -= count;
			Number -= count;
            return this;
        }
        #endregion
    }
}
