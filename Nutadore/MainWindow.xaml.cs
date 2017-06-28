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

			Note noteStemUp = new Note(
				Note.Letter.E,
				Accidental.Type.None,
				Note.Octave.OneLined,
				new Duration(Duration.Name.Eighth, true));
			noteStemUp.stemDirection = Note.StemDirection.Up;

			Note noteStemDown = new Note(
				Note.Letter.A,
				Accidental.Type.None,
				Note.Octave.Small,
				new Duration(Duration.Name.Eighth, true));
			noteStemDown.stemDirection = Note.StemDirection.Down;

			score.Add(new Step()
				.AddVoice(noteStemUp)
				.AddVoice(noteStemDown));
		}
	}
}
