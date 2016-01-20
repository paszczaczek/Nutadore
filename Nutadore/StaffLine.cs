using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
    public class StaffLine : IComparable
    {
        static public StaffLine Base(int lineBaseNo)
        {
            if (lineBaseNo < 1 || lineBaseNo > 5)
                throw new ArgumentOutOfRangeException("lineBaseNo", lineBaseNo, "Numer linii pieciolinii musi był liczbą 1..5");
            return new StaffLine(Name.Base1 + lineBaseNo - 1);
        }

        static public StaffLine LagerAbove(int lineAboveNo)
        {
            if (lineAboveNo < 1 || lineAboveNo > 5)
                throw new ArgumentOutOfRangeException("lineAboveNo", lineAboveNo, "Numer linii dodanej górnej pieciolinii musi być liczba 1..5");
            return new StaffLine(Name.Base1 + 5 + lineAboveNo);
        }

        static public StaffLine LagerBelow(int lineBelowNo)
        {
            if (lineBelowNo < 1 || lineBelowNo > 5)
                throw new ArgumentOutOfRangeException("lineBelowNo", lineBelowNo, "Numer linii dodanej dolnej pieciolinii musi być liczba 1..6");
            return new StaffLine(Name.Base1 - lineBelowNo);
        }

        public enum Name
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

        public Name name;
        private StaffLine(Name name)
        {
            this.name = name;
        }

        public int Number
        {
            get { return (int)name; }
        }

        public int CompareTo(object obj)
        {
            StaffLine line = obj as StaffLine;
            if (name == line.name)
                return 0;
            if (name < line.name)
                return -1;
            return 1;
        }

        static public bool operator <=(StaffLine line1, StaffLine line2)
        {
            return line1.CompareTo(line2) <= 0;
        }

        static public bool operator >=(StaffLine line1, StaffLine line2)
        {
            return line1.CompareTo(line2) >= 0;
        }

        static public StaffLine operator ++(StaffLine line)
        {
            line.name++;
            return line;
        }
    }
}
