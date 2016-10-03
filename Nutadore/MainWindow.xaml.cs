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
		public MainWindow()
		{
			InitializeComponent();

			//AddAllTriads();
			AddAllNotes();
			AddTriad();
			score.Add(new Note(Note.Letter.C, Note.Octave.OneLined));
		}

		private void AddTriad()
		{
			Chord chord = new Chord();

			chord.Add(new Note(Note.Letter.C, Note.Octave.OneLined));
			chord.Add(new Note(Note.Letter.E, Note.Octave.OneLined));
			chord.Add(new Note(Note.Letter.G, Note.Octave.OneLined));

			score.Add(chord);
		}

		private void AddAllNotes()
		{
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.isWhite);
			for (int k = 0; k < whiteKeys.Count - 4; k++)
			{
				score.Add(new Note(whiteKeys[k + 0].letter, whiteKeys[k + 0].octave));
			}
		}

		private void AddAllTriads()
		{
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.isWhite);
			for (int k = 0; k < whiteKeys.Count - 4; k++)
			{
				Chord chord = new Chord();

				chord.Add(new Note(whiteKeys[k + 0].letter, whiteKeys[k + 0].octave));
				chord.Add(new Note(whiteKeys[k + 2].letter, whiteKeys[k + 2].octave));
				chord.Add(new Note(whiteKeys[k + 4].letter, whiteKeys[k + 4].octave));

				int u = whiteKeys.Count - 1 - 4 - k;
				chord.Add(new Note(whiteKeys[u + 0].letter, whiteKeys[u + 0].octave));
				chord.Add(new Note(whiteKeys[u + 2].letter, whiteKeys[u + 2].octave));
				chord.Add(new Note(whiteKeys[u + 4].letter, whiteKeys[u + 4].octave));

				score.Add(chord);
			}

			AddBar();
		}

		private void AddBar()
		{
			score.Add(new Bar());
		}
	}
}
