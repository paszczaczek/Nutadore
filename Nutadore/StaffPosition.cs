using System;

namespace Nutadore
{
    public class StaffPosition : IComparable
    {
        #region Publiczne konstruktory statyczne.
        static public StaffPosition ByLine(int baseLineNo, bool above = false)
        {
            CheckLineNo(baseLineNo, 1, 5, "linii");
            return new StaffPosition(LineName.Base1 + baseLineNo - 1, above);
        }

        static public StaffPosition ByLagerAbove(int lagerAboveNo, bool above = false)
        {
            CheckLineNo(lagerAboveNo, 1, 5, "linii dodanej górnej");
            return new StaffPosition(LineName.Base1 + 4 + lagerAboveNo, above);
        }

        static public StaffPosition ByLagerBelow(int lagerBelowNo, bool above = false)
        {
            CheckLineNo(lagerBelowNo, 1, 6, "linii dodanej dolnej");
            return new StaffPosition(LineName.Base1 - lagerBelowNo, above);
        }

        static private void CheckLineNo(int lineNo, int lineNoMin, int lineNoMax, string lineName)
        {
            if (lineNo < lineNoMin || lineNo > lineNoMax)
                throw new ArgumentOutOfRangeException(
                    "line", 
                    lineNo, string.Format("Numer {0} pieciolinii musi być liczba {1}..{2}", lineName, lineNoMin, lineNoMax));
        }

        static public StaffPosition ByLineNumber(double lineNumber)
        {
            double lineNumberMin = (int)LineName.LagerBelow6;
            double lineNumberMax = (int)LineName.LagerAbove5 + 0.5;
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

            LineName lineName;
            bool lineAbove;
            if (remainder == 0)
            {
                lineName = (LineName)(int)lineNumber;
                lineAbove = false;
            }
            else if (remainder > 0)
            {
                lineName = (LineName)(int)lineNumber;
                lineAbove = true;
            }
            else // remainder < 0
            {
                lineName = (LineName)(int)(lineNumber - 1);
                lineAbove = true;
            }

            return new StaffPosition(lineName, lineAbove);
        }
        #endregion

        public enum LineName
        {
            LagerBelow6 = -6,
            LagerBelow5 = -5,
            LagerBelow4 = -4,
            LagerBelow3 = -3,
            LagerBelow2 = -2,
            LagerBelow1 = -1,
            Base1 = 0,
            Base2 = 1,
            Base3 = 2,
            Base4 = 3,
            Base5 = 4,
            LagerAbove1 = 5,
            LagerAbove2 = 6,
            LagerAbove3 = 7,
            LagerAbove4 = 8,
            LagerAbove5 = 9
        }

        private LineName lineName;
        private bool lineAbove;
        public double LineNumber
        {
            get { return (double)lineName + (lineAbove ? 0.5 : 0.0); }
        }

        // Konstruktor prywatny
        private StaffPosition(LineName lineName, bool lineAbove)
        {
            this.lineName = lineName;
            this.lineAbove = lineAbove;
        }
        
        #region Funkcje do obsługi petli for()
        public int CompareTo(object obj)
        {
            StaffPosition staffPosition = obj as StaffPosition;
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
            lineName += count;
            return this;
        }

        public StaffPosition SubstractLine(int count)
        {
            lineName -= count;
            return this;
        }
        #endregion
    }
}
