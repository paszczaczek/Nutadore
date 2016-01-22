using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nutadore
{
    class Note : Sign
    {
        public Note(Score score)
        {
            base.score = score;
        }

        override public double Show(double left, double top)
        {
            string noteCode = "\x0055";
            top -= 57.5 * base.score.Magnification;
            double right = base.Show(noteCode, left, top);

            return right;
        }

    }
}
