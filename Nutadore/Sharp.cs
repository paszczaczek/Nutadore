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
        public Sharp(Score score, StaffPosition statfPosition)
        {
            base.score = score;
            base.staffPosition = statfPosition;
        }

        override public double Show(double left, double top)
        {
            const string sharpCode = "\x002e";
            top -= 57 * base.score.Magnification;
            double right = base.Show(sharpCode, left, top);

            return right;
        }
    }
}
