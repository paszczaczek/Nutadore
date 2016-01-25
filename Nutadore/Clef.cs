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

        private  Type type;

        public Clef(Type type)
        {
            this.type = type;
        }

        override public double Show(Score score, double left, double top)
        {
            base.code = null;
            switch (type)
            {
                case Type.Treble:
                    base.code = "\x00c9";
                    top -= 57 * score.Magnification;
                    break;
                case Type.Bass:
                    base.code = "\x00c7";
                    top -= 77.5 * score.Magnification;
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
            double right = base.Show(score, left, top);

            return right;
        }
    }
}
