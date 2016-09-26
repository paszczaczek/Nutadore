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

			//AddTriad(Note.Letter.G, Note.Octave.FourLined);
			//FromLowestToHighestAdd((letter, octave) => new Note(letter, octave));
			//FromLowestToHighestAdd((letter, octave) => Triad(letter, octave));
			for (int keyNumber = 1; keyNumber <= Keyboard.numberOfWhiteKeys - 4; keyNumber++)
			{
				Chord chord = new Chord();
				chord.Add(Keyboard.NoteOfKeyNumber(keyNumber)); 
				chord.Add(Keyboard.NoteOfKeyNumber(keyNumber + 2));
				chord.Add(Keyboard.NoteOfKeyNumber(keyNumber + 4));
				chord.Add(Keyboard.NoteOfKeyNumber(Keyboard.numberOfWhiteKeys + 1 - keyNumber));
				chord.Add(Keyboard.NoteOfKeyNumber(Keyboard.numberOfWhiteKeys + 1 - keyNumber - 2));
				chord.Add(Keyboard.NoteOfKeyNumber(Keyboard.numberOfWhiteKeys + 1 - keyNumber - 4));

				score.Add(chord);
			}
			AddBar();
			//AddBar();
			//FromLowestToHighestAdd((letter, octave) => new Note(letter, octave));

			//score.Add(new Note(Note.Letter.C, Note.Octave.ThreeLined));
			//score.Add(new Bar());

			//chord1.Add(new Note(Note.Letter.C, Note.Octave.Contra));
			//chord1.Add(new Note(Note.Letter.E, Note.Octave.Contra));
			//chord1.Add(new Note(Note.Letter.G, Note.Octave.Contra));
			//chord1.Add(new Note(Note.Letter.H, Note.Octave.Contra));

			//chord1.Add(new Note(Note.Letter.D, Note.Octave.Great));
			//chord1.Add(new Note(Note.Letter.F, Note.Octave.Great));
			//chord1.Add(new Note(Note.Letter.A, Note.Octave.Great));

			//chord1.Add(new Note(Note.Letter.C, Note.Octave.Small));
			//chord1.Add(new Note(Note.Letter.E, Note.Octave.Small));
			//chord1.Add(new Note(Note.Letter.G, Note.Octave.Small));
			//chord1.Add(new Note(Note.Letter.H, Note.Octave.Small));

			//chord1.Add(new Note(Note.Letter.D, Note.Octave.OneLined));
			//chord1.Add(new Note(Note.Letter.F, Note.Octave.OneLined));
			//chord1.Add(new Note(Note.Letter.A, Note.Octave.OneLined));

			//chord1.Add(new Note(Note.Letter.C, Note.Octave.TwoLined));
			//chord1.Add(new Note(Note.Letter.E, Note.Octave.TwoLined));
			//chord1.Add(new Note(Note.Letter.G, Note.Octave.TwoLined));
			//chord1.Add(new Note(Note.Letter.H, Note.Octave.TwoLined));

			//chord1.Add(new Note(Note.Letter.D, Note.Octave.ThreeLined));
			//chord1.Add(new Note(Note.Letter.F, Note.Octave.ThreeLined));
			//chord1.Add(new Note(Note.Letter.A, Note.Octave.ThreeLined));

			// TODO sprawdzic jak sie zachowa rysowanie ottavy przy akrodzie
			// z reszta znakow w gore...
			//chord1.Add(new Note(Note.Letter.C, Note.Octave.FourLined));
			//chord1.Add(new Note(Note.Letter.E, Note.Octave.FourLined));
			//chord1.Add(new Note(Note.Letter.G, Note.Octave.FourLined));
			//chord1.Add(new Note(Note.Letter.H, Note.Octave.FourLined));

			//score.Add(chord1);

			//Chord chord2 = new Chord();
			//chord2.Add(new Note(Note.Letter.H, Note.Octave.SubContra));
			//chord2.Add(new Note(Note.Letter.D, Note.Octave.Contra));
			//chord2.Add(new Note(Note.Letter.F, Note.Octave.Contra));
			//score.Add(chord2);

			//Chord chord3 = new Chord();
			//chord3.Add(new Note(Note.Letter.C, Note.Octave.Contra));
			//chord3.Add(new Note(Note.Letter.E, Note.Octave.Contra));
			//chord3.Add(new Note(Note.Letter.G, Note.Octave.Contra));
			//score.Add(chord3);

			//score.Add(new Note(Note.Letter.A, Note.Octave.SubContra));
			////score.Add(new Note(Note.Letter.H, Note.Octave.SubContra));
			//score.Add(new Note(Note.Letter.D, Note.Octave.Contra));
			//score.Add(new Note(Note.Letter.E, Note.Octave.Contra));
			//score.Add(new Note(Note.Letter.F, Note.Octave.Contra));
			//score.Add(new Note(Note.Letter.G, Note.Octave.Contra));
			//#else
		}

		private void AddBar()
		{
			score.Add(new Bar());
		}

		private Sign Triad(Note.Letter letter, Note.Octave octave)
		{
			Chord chord = new Chord();
			Note rootNote = new Note(letter, octave);

			if (rootNote.octave == Note.Octave.FiveLined && rootNote.letter == Note.Letter.C ||
				rootNote.octave == Note.Octave.FourLined && rootNote.letter > Note.Letter.F)
				return null;

			chord.Add(rootNote.Copy().Transpose(0));
			chord.Add(rootNote.Copy().Transpose(2));
			chord.Add(rootNote.Copy().Transpose(4));

			return chord;
		}

		private void FromLowestToHighestAdd(Func<Note.Letter, Note.Octave, Sign> makeSign, bool showBars = true)
		{
			Sign sign;
			for (int i = 5; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.SubContra);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.Contra);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.Great);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.Small);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.OneLined);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.TwoLined);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.ThreeLined);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			for (int i = 0; i < 7; i++)
			{
				sign = makeSign(Note.Letter.C + i, Note.Octave.FourLined);
				if (sign != null)
					score.Add(sign);
			}
			if (showBars)
				score.Add(new Bar());

			sign = makeSign(Note.Letter.C, Note.Octave.FiveLined);
			if (sign != null)
				score.Add(sign);
			if (showBars)
				score.Add(new Bar());
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
				bool changed = false;

				if (e.Delta < 0)
				{
					if (score.Magnification > 0.5)
					{
						score.Magnification -= magnificationDelta;
						changed = true;
					}
				}
				else
				{
					if (score.Magnification < 5)
					{
						score.Magnification += magnificationDelta;
						changed = true;
					}
				}

				if (changed)
				{
					Properties.Settings.Default.ScoreMagnification = score.Magnification;
					Properties.Settings.Default.Save();
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
				{
					score.Magnification = 1.0;
					Properties.Settings.Default.ScoreMagnification = score.Magnification;
					Properties.Settings.Default.Save();
				}
			}
		}

		private void testButton_Click(object sender, RoutedEventArgs e)
		{
			score.Clear();
		}
	}
}
