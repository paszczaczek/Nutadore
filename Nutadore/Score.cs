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
        public Canvas canvas;
        public Scale scale = new Scale(Scale.Based.C, Scale.Type.Major);
        public List<Sign> signs = new List<Sign>();

        public Score(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.ClipToBounds = true;
        }

        public void Add(Sign sign)
        {
            signs.Add(sign);
        }

        public Note FindNextNote(Sign sign)
        {
            int idx = signs.IndexOf(sign);
            int idxNextNote = signs.FindIndex(idx, s => s is Note);

            return signs[idxNextNote] as Note;
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
            // Czyscimy partyturę.
            Clear();

            // Rysujemy podwójną pieciolinię.
            StaffGrand staffGrand = new StaffGrand();
            double top = 0;
            double bottom;
            while (!staffGrand.Show(this, top, out bottom))
            {
                // Nie wszystkie znaki zmieściły się na tej podwójnej pięciolinii.
                // Pozostałe umieścimi na kolejnej podwójnej pięciolinii.
                top = bottom;
                // Jeśli wyszliśmy poza dolną krawędź canvas, to kończymy rysowanie.
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
