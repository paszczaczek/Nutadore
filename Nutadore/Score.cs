using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Score
    {
        static private readonly double distanceBetweenStaffGrands = 150;

        public Canvas canvas;
        public Scale scale = new Scale(Scale.Based.C, Scale.Type.Major);
        public  List<Sign> signs = new List<Sign>();

        public Score(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.ClipToBounds = true;
        }

        public void Add(Sign sign)
        {
            signs.Add(sign);
        }

        private double magnification = 1.0;
        public double Magnification {
            get
            {
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
            // czyscimy partyturę
            Clear();

            // rusujemy podwójną pieciolinię 
            StaffGrand staffGrand = new StaffGrand();
            double top = 0;
            while (!staffGrand.Show(this, top))
            {
                // nie zmieściło się, dokończymy na kolejnej
                top += distanceBetweenStaffGrands;
                // jeśli brakuje miejsca w pionie, to przerwyamy
                if (top > canvas.ActualHeight)
                    break;
            }
        }

        public void Clear()
        {
            // usuwamy wszystkie nuty
            signs.ForEach(sign => sign.Clear());

            // usuwamy pozostałe elemetny (klucze, znaki przykluczowe, itd.)
            canvas.Children.Clear();
        }
    }
}
