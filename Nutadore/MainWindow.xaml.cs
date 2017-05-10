using mshtml;
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

			score.ConnectKeyboard(keyboard);
			keyboard.ConnectScore(score);

			//BachAirInDMajor();
			//AddTestAccidentalsFingers();
			LyParserTest();
			//AddSteps();
			//AddAllTriads();
			//AddAllNotes();
			//AddTriad();
			//score.Add(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined));
		}

		private void LyParserTest()
		{
			List<Sign>[] pm = LyParser.ParallelMusic(@"Misc\bach-air-in-d-major.ly");
			foreach (Sign sign in pm[0])
			{
				Step step = new Step();
				step.AddVoice(sign);
				score.Add(step);
			}
		}

		private void AddTestAccidentalsFingers()
		{
			Chord chord = new Chord();
			List<Note> notesUp = new List<Note> {
				new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.TwoLined),
				new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.TwoLined),
				new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.TwoLined),
				new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.TwoLined),
				new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.TwoLined),
				new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.TwoLined),
				new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.TwoLined),

				new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.ThreeLined),
				new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.ThreeLined),
				new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.ThreeLined),
				new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.ThreeLined),
				new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.ThreeLined),
				new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.ThreeLined),
				new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.ThreeLined)
			};
			int finger = 0;
			foreach (Note note in notesUp)
			{
				note.stemDirection = Note.StemDirection.Up;
				note.finger = finger++ % 5;
				chord.Add(note);
			}

			List<Note> notesDown = new List<Note> { 
				new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Great),
				new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.Great),
				new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.Great),
				new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.Great),
				new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Great),
				new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.Great),
				new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.Great),

				new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small),
				new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.Small),
				new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.Small),
				new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.Small),
				new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Small),
				new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.Small),
				new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.Small)
			};
			finger = 1;
			foreach (Note note in notesDown)
			{
				note.stemDirection = Note.StemDirection.Down;
				note.finger = finger++ % 5;
				//if (note.finger == 0)
				//	note.finger = null;
			}

			Step step = new Step();
			step.AddVoice(chord);
			notesDown.ForEach(note => step.AddVoice(note));
			score.Add(step);
		}

		private void BachAirInDMajor()
		{
			score.scale = new Scale(Note.Letter.D, Scale.Type.Major);

			score
				// measure 1
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.OneLined, Staff.Type.Bass)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.OneLined, Staff.Type.Bass)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small)))
				// measure 2
				.Add(new Step()
					.AddVoice(new Bar()))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.Great)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.Great)))
				// measure 3
				.Add(new Step()
					.AddVoice(new Bar()))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.Great)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.TwoLined)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.TwoLined)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.OneLined))
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.TwoLined)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Great)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.TwoLined)))
				// measure 4
				.Add(new Step()
					.AddVoice(new Bar()))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.OneLined))
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.Great)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.OneLined))
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.Small)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.OneLined)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.Great)))
				// measure 5
				.Add(new Step()
					.AddVoice(new Bar()))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.TwoLined))
					.AddVoice(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.Great)))
				.Add(new Step()
					.AddVoice(new Note(Note.Letter.C, Accidental.Type.Natural, Note.Octave.TwoLined)))
				// tu skonczylem, kasownik rysuje sie jako #
				;
		}

		private void AddSteps()
		{
			Chord chord1 = new Chord();
			chord1.Add(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined));
			chord1.Add(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.OneLined));
			chord1.Add(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.OneLined));

			Note note1 = new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.TwoLined);

			Chord chord2 = new Chord();
			chord2.Add(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.Great));
			chord2.Add(new Note(Note.Letter.F, Accidental.Type.None, Note.Octave.Great));
			chord2.Add(new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.Great));

			Note note2 = new Note(Note.Letter.F, Accidental.Type.None, Note.Octave.Small);

			Step step = new Step();
			step.AddVoice(chord1);
			step.AddVoice(note1);
			step.AddVoice(chord2);
			step.AddVoice(note2);

			score.Add(step);
		}

		//private void AddTriad()
		//{
		//	Chord chord = new Chord();

		//	chord.Add(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined));
		//	//chord.Add(new Note(Note.Letter.D, Note.Octave.OneLined));
		//	chord.Add(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.OneLined));
		//	//chord.Add(new Note(Note.Letter.F, Note.Octave.OneLined));
		//	chord.Add(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.OneLined));

		//	score.Add(chord);
		//}

		private void AddAllNotes()
		{
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.isWhite);
			for (int k = 0; k < whiteKeys.Count; k++)
			{
				Note note = new Note(
					whiteKeys[k].note.letter,
					whiteKeys[k].note.accidental.type,
					whiteKeys[k].note.octave);

				Note note2 = new Note(
					whiteKeys[k].note.letter,
					Accidental.Type.Sharp,
					whiteKeys[k].note.octave);

				Step step = new Step();
				step.AddVoice(note);
				step.AddVoice(note2);

				score.Add(step);
			}
		}

		private void AddAllTriads()
		{
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.isWhite);
			for (int k = 0; k < whiteKeys.Count - 4; k++)
			{
				Chord chord1 = new Chord();

				chord1.Add(new Note(whiteKeys[k + 0].note.letter, Accidental.Type.None, whiteKeys[k + 0].note.octave));
				chord1.Add(new Note(whiteKeys[k + 2].note.letter, Accidental.Type.None, whiteKeys[k + 2].note.octave));
				chord1.Add(new Note(whiteKeys[k + 4].note.letter, Accidental.Type.None, whiteKeys[k + 4].note.octave));

				int u = whiteKeys.Count - 1 - 4 - k;
				Chord chord2 = new Chord();
				chord2.Add(new Note(whiteKeys[u + 0].note.letter, Accidental.Type.None, whiteKeys[u + 0].note.octave));
				chord2.Add(new Note(whiteKeys[u + 2].note.letter, Accidental.Type.None, whiteKeys[u + 2].note.octave));
				chord2.Add(new Note(whiteKeys[u + 4].note.letter, Accidental.Type.None, whiteKeys[u + 4].note.octave));

				Step step = new Step();
				step.AddVoice(chord1);
				step.AddVoice(chord2);

				score.Add(step);
			}

			AddBar();
		}

		private void AddBar()
		{
			Step step = new Step();
			step.AddVoice(new Bar());

			score.Add(step);
		}

		//private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
		//{
		//	//e.Cancel = true;
		//	mshtml.IHTMLDocument2 htmlDoc = webBrowser.Document as mshtml.IHTMLDocument2;
		//	if (htmlDoc != null)
		//	{
		//		IHTMLElementCollection all = htmlDoc.all;
		//		var t = all.item(0).tagName;
		//	}
		//}
	}
}
