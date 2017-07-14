using System.Windows;

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

			//Note noteStemUp1 = new Note(
			//	Note.Letter.C,
			//	Accidental.Type.None,
			//	Note.Octave.OneLined,
			//	new Duration(Duration.Name.Quarter, true));
			//noteStemUp1.finger = 1;
			//noteStemUp1.stemDirection = Note.StemDirection.Up;

			//Note noteStemDown1 = new Note(
			//	Note.Letter.A,
			//	Accidental.Type.None,
			//	Note.Octave.Small,
			//	new Duration(Duration.Name.Quarter, true));
			//noteStemDown1.finger = 2;
			//noteStemDown1.stemDirection = Note.StemDirection.Down;

			//score.Add(new Step()
			//	.AddVoice(noteStemUp1)
			//	.AddVoice(noteStemDown1)
			//	);

			//Note noteStemUp2 = new Note(
			//	Note.Letter.D,
			//	Accidental.Type.None,
			//	Note.Octave.OneLined,
			//	new Duration(Duration.Name.Eighth, true));
			//noteStemUp2.finger = 1;
			//noteStemUp2.stemDirection = Note.StemDirection.Up;

			//Note noteStemDown2 = new Note(
			//	Note.Letter.G,
			//	Accidental.Type.None,
			//	Note.Octave.Small,
			//	new Duration(Duration.Name.Eighth, true));
			//noteStemDown2.finger = 2;
			//noteStemDown2.stemDirection = Note.StemDirection.Down;

			//score.Add(new Step()
			//	.AddVoice(noteStemUp2)
			//	.AddVoice(noteStemDown2));
		}
	}
}
