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

			score.keyboard = keyboard;
			keyboard.score = score;

			AddAllTriads();
			AddAllNotes();
			AddTriad();
			score.Add(new Note(Note.Letter.C, Note.Accidental.None, Note.Octave.OneLined));
		}

		private void AddTriad()
		{
			Chord chord = new Chord();

			chord.Add(new Note(Note.Letter.C, Note.Accidental.None, Note.Octave.OneLined));
			//chord.Add(new Note(Note.Letter.D, Note.Octave.OneLined));
			chord.Add(new Note(Note.Letter.E, Note.Accidental.None, Note.Octave.OneLined));
			//chord.Add(new Note(Note.Letter.F, Note.Octave.OneLined));
			chord.Add(new Note(Note.Letter.G, Note.Accidental.None, Note.Octave.OneLined));

			score.Add(chord);
		}

		private void AddAllNotes()
		{
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.isWhite);
			for (int k = 0; k < whiteKeys.Count; k++)
			{
				score.Add(new Note(
					whiteKeys[k].note.letter,
					whiteKeys[k].note.accidental,
					whiteKeys[k].note.octave));
			}
		}

		private void AddAllTriads()
		{
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.isWhite);
			for (int k = 0; k < whiteKeys.Count - 4; k++)
			{
				Chord chord = new Chord();

				chord.Add(new Note(whiteKeys[k + 0].note.letter, Note.Accidental.None, whiteKeys[k + 0].note.octave));
				chord.Add(new Note(whiteKeys[k + 2].note.letter, Note.Accidental.None, whiteKeys[k + 2].note.octave));
				chord.Add(new Note(whiteKeys[k + 4].note.letter, Note.Accidental.None, whiteKeys[k + 4].note.octave));

				int u = whiteKeys.Count - 1 - 4 - k;
				chord.Add(new Note(whiteKeys[u + 0].note.letter, Note.Accidental.None, whiteKeys[u + 0].note.octave));
				chord.Add(new Note(whiteKeys[u + 2].note.letter, Note.Accidental.None, whiteKeys[u + 2].note.octave));
				chord.Add(new Note(whiteKeys[u + 4].note.letter, Note.Accidental.None, whiteKeys[u + 4].note.octave));

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
