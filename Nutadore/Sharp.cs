using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nutadore
{
    public class Sharp : Sign
    {
        public Sharp(Position staffLine)
        {
            base.staffLine = staffLine;
        }

        override public double Paint(Canvas canvas, double left, double top, double maginification)
        {
            const string sharpCode = "\x002e";

            top -= 57 * maginification;

            double right = base.Paint(canvas, sharpCode, left, top, maginification);

            return right;
        }

    }
}
