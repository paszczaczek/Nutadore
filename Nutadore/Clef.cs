using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nutadore
{
    public class Clef : Sign
    {
        public Clef()
        {
        }

        public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
        {
            double maxRightClef = left;
            foreach (Staff staff in new[] { trebleStaff, bassStaff })
            {
                string glyphCode = null;
                double glyphTop = staff.StaffPositionToY(StaffPosition.ByLine(2));
                switch (staff.type)
                {
                    case Staff.Type.Treble:
                        glyphCode = "\x00c9";
                        glyphTop -= 57 * score.Magnification;
                        break;
                    case Staff.Type.Bass:
                        glyphCode = "\x00c7";
                        glyphTop -= 77.5 * score.Magnification;
                        break;
                }
                double rightClef = base.ShowFetaGlyph(score, left, glyphTop, glyphCode);
                if (rightClef > maxRightClef)
                    maxRightClef = rightClef;
            }

            double right = maxRightClef + Staff.spaceBetweenSigns * score.Magnification;

            return right;
        }
    }
}
