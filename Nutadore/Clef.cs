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
        public enum Type
        {
            Treble,
            Bass,
            TrebleSmall,
            BassSmall
        }

        private Type type;

        public Clef(Type type)
        {
            this.type = type;
        }

        override public double Show(Score score, double left, double top)
        {
            string glyphCode = null;
            double glyphTop = top;
            switch (type)
            {
                case Type.Treble:
                    glyphCode = "\x00c9";
                    glyphTop -= 57 * score.Magnification;
                    break;
                case Type.Bass:
                    glyphCode = "\x00c7";
                    glyphTop -= 77.5 * score.Magnification;
                    break;
                case Type.TrebleSmall:
                    // TODO
                    break;
                case Type.BassSmall:
                    // TODO
                    break;
                default:
                    break;
            }
            double right = base.ShowFetaGlyph(score, left, glyphTop, glyphCode);

            return right;
        }
    }
}
