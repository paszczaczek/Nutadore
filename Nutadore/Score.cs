using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Shapes;

namespace Nutadore
{
    class Score
    {
        static public readonly double FetaFontSize = 40;
        public Scale scale;

        private Canvas _canvas;
        private List<Sign> _signs = new List<Sign>();

        public Score(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Add(Sign sign)
        {
            _signs.Add(sign);
        }

        private double _magnification = 1.0;
        public double Magnification {
            get
            {
                return _magnification;
            }
            set
            {
                _magnification = value;
                Paint();
            }
        }

        public void Paint()
        {
            _canvas.Children.Clear();

            StaffPair staffCurrent = null;
            int staffNumber = 0;
            foreach (Sign s in _signs)
            {
                while (staffCurrent == null || !staffCurrent.PaintSing(s))
                {
                    staffCurrent = new StaffPair(_canvas, staffNumber, Magnification); 
                    staffCurrent.Paint();
                    staffNumber++;
                }
            }

            _canvas.UpdateLayout();
        }
    }
}
