using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nutadore
{
    class Clef : Sign
    {
        private Staff.Type _type;
        public Clef(Staff.Type type)
        {
            _type = type;
        }

        public void Paint(Canvas canvas, double left, double top, double maginification)
        {
            string clefCode = null;
            switch (_type)
            {
                case Staff.Type.Treble:
                    clefCode = "\x00c9";
                    top -= 57 * maginification;
                    break;
                case Staff.Type.Bass:
                    clefCode = "\x00c7";
                    top -= 77.5 * maginification;
                    break;
                default:
                    break;
            }
            base.Paint(canvas, clefCode, left, top, maginification);
        }
    }
}
