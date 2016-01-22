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

        public Clef(Score score, Type type)
        {
            base.score = score;
            this.type = type;
        }

        override public double Show(double left, double top)
        {
            string clefCode = null;
            switch (type)
            {
                case Type.Treble:
                    clefCode = "\x00c9";
                    top -= 57 * base.score.Magnification;
                    break;
                case Type.Bass:
                    clefCode = "\x00c7";
                    top -= 77.5 * base.score.Magnification;
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
            double right = base.Show(clefCode, left, top);

            return right;
        }
    }
}
