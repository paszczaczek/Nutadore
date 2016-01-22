using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
    public class StaffPosition : IComparable
    {
        #region Publiczne konstruktory statyczne.
        static public StaffPosition Line(int line)
        {
            if (line < 1 || line > 5)
                throw new ArgumentOutOfRangeException("line", line, "Numer linii pieciolinii musi był liczbą 1..5");
            return new StaffPosition(Name.Base1 + line - 1, false);
        }

        static public StaffPosition LagerAbove(int line)
        {
            if (line < 1 || line > 5)
                throw new ArgumentOutOfRangeException("line", line, "Numer linii dodanej górnej pieciolinii musi być liczba 1..5");
            return new StaffPosition(Name.Base1 + 5 + line, false);
        }

        static public StaffPosition LagerBelow(int line)
        {
            if (line < 1 || line > 6)
                throw new ArgumentOutOfRangeException("line", line, "Numer linii dodanej dolnej pieciolinii musi być liczba 1..6");
            return new StaffPosition(Name.Base1 - line, false);
        }

        public class Above
        {
            static public StaffPosition Line(int line)
            {
                if (line < 1 || line > 5)
                    throw new ArgumentOutOfRangeException("line", line, "Numer linii pieciolinii musi był liczbą 1..5");
                return new StaffPosition(Name.Base1 + line - 1, true);
            }

            static public StaffPosition LagerAbove(int line)
            {
                if (line < 1 || line > 5)
                    throw new ArgumentOutOfRangeException("line", line, "Numer linii dodanej górnej pieciolinii musi być liczba 1..5");
                return new StaffPosition(Name.Base1 + 5 + line, true);
            }

            static public StaffPosition LagerBelow(int line)
            {
                if (line < 1 || line > 6)
                    throw new ArgumentOutOfRangeException("line", line, "Numer linii dodanej dolnej pieciolinii musi być liczba 1..6");
                return new StaffPosition(Name.Base1 - line, true);
            }
        }
        #endregion

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

        private Name name;
        private bool above;

        // Konstruktor prywatny
        private StaffPosition(Name name, bool above)
        {
            this.name = name;
            this.above = above;
        }

        public double ToDouble()
        {
            return (double)name + (above ? 0.5 : 0.0);
        }

        public int CompareTo(object obj)
        {
            StaffPosition line = obj as StaffPosition;
            if (name == line.name)
                return 0;
            if (name < line.name)
                return -1;
            return 1;
        }

        static public bool operator <=(StaffPosition line1, StaffPosition line2)
        {
            return line1.CompareTo(line2) <= 0;
        }

        static public bool operator >=(StaffPosition line1, StaffPosition line2)
        {
            return line1.CompareTo(line2) >= 0;
        }

        static public StaffPosition operator ++(StaffPosition line)
        {
            line.name++;
            return line;
        }
    }
}
