using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nutadore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Score score;

        public MainWindow()
        {
            InitializeComponent();

            score = new Score(canvas);
            score.scale = new Scale(Note.Letter.C, Scale.Type.Minor);

#if false
            Chord chord1 = new Chord();
            chord1.Add(new Note(Note.Letter.A, Note.Octave.SubContra));
            chord1.Add(new Note(Note.Letter.C, Note.Octave.Contra));
            chord1.Add(new Note(Note.Letter.E, Note.Octave.Contra));
            score.Add(chord1);
            
            Chord chord2 = new Chord();
            chord2.Add(new Note(Note.Letter.H, Note.Octave.SubContra));
            chord2.Add(new Note(Note.Letter.D, Note.Octave.Contra));
            chord2.Add(new Note(Note.Letter.F, Note.Octave.Contra));
            score.Add(chord2);

            Chord chord3 = new Chord();
            chord3.Add(new Note(Note.Letter.C, Note.Octave.Contra));
            chord3.Add(new Note(Note.Letter.E, Note.Octave.Contra));
            chord3.Add(new Note(Note.Letter.G, Note.Octave.Contra));
            score.Add(chord3);

            //score.Add(new Note(Note.Letter.A, Note.Octave.SubContra));
            //score.Add(new Note(Note.Letter.H, Note.Octave.SubContra));
            score.Add(new Note(Note.Letter.D, Note.Octave.Contra));
            score.Add(new Note(Note.Letter.E, Note.Octave.Contra));
            score.Add(new Note(Note.Letter.F, Note.Octave.Contra));
            score.Add(new Note(Note.Letter.G, Note.Octave.Contra));


#else
            for (int i = 5; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.SubContra));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.Contra));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.Great));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.Small));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.OneLined));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.TwoLined));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.ThreeLined));
            score.Add(new Bar());

            for (int i = 0; i < 7; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.FourLined));
            score.Add(new Bar());

            for (int i = 0; i < 1; i++)
                score.Add(new Note(Note.Letter.C + i, Note.Octave.FiveLined));
            score.Add(new Bar());
#endif
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            score.Show();
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //base.OnPreviewMouseWheel(e);
            //if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
            //    Keyboard.IsKeyDown(Key.RightCtrl))
            {
                const double magnificationDelta = 0.2;
                if (e.Delta < 0)
                {
                    if (score.Magnification > 0.5)
                        score.Magnification -= magnificationDelta;
                }
                else
                {
                    if (score.Magnification < 5)
                        score.Magnification += magnificationDelta;
                }
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //base.OnPreviewMouseDown(e);
            //if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
            //    Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.ChangedButton == MouseButton.Middle)
                    score.Magnification = 1.0;
            }
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            score.Clear();
        }
    }
}
