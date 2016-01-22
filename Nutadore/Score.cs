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
    public class Score
    {
        public Canvas canvas;
        public Scale scale;

        private List<Sign> signs = new List<Sign>();

        public Score(Canvas canvas, Scale.Based scaleBased, Scale.Type scaleType)
        {
            this.canvas = canvas;
            scale = new Scale(this, scaleBased, scaleType);
        }

        public void Add(Sign sign)
        {
            signs.Add(sign);
        }

        private double magnification = 1.0;
        public double Magnification {
            get{
                return magnification;
            }
            set
            {
                magnification = value;
                Show();
            }
        }

        public void Show()
        {
            canvas.Children.Clear();

            GrandStaff grandStaffCurrent = null;
            int grandStaffIndex = 0;
            foreach (Sign sign in signs)
            {
                while (grandStaffCurrent == null || !grandStaffCurrent.Add(sign))
                {
                    grandStaffCurrent = new GrandStaff(this, 0);
                    grandStaffCurrent.Show();
                    grandStaffIndex++;
                }
            }

            canvas.UpdateLayout();
        }
    }
}
