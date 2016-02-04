using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nutadore
{
    public class Accidental : Sign
    {
        public StaffPosition staffPosition;

        public Accidental(StaffPosition statfPosition)
        {
            this.staffPosition = statfPosition;
        }

        override public double Show(Score score, double left, double top)
        {
            string glyphCode = "\x002e";
            double glyphTop = top;
            glyphTop -= 57 * score.Magnification;
            double right = base.ShowFetaGlyph(score, left, glyphTop, glyphCode);

            return right;
        }
    }
}
