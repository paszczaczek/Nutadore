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
        public Sharp(StaffPosition statfPosition)
        {
            base.staffPosition = statfPosition;
        }

        override public double Show(Score score, double left, double top)
        {
            base.code = "\x002e";
            top -= 57 * score.Magnification;
            double right = base.Show(score, left, top);

            return right;
        }
    }
}
