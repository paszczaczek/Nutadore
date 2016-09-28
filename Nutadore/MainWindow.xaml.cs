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

			Keyboard keyboard = new Keyboard();
			List<Key> whiteKeys = keyboard.keys.FindAll(key => key.IsWhite);
			for (int k = 0; k < whiteKeys.Count - 4; k++)
			{
				Chord chord = new Chord();

				chord.Add(new Note(whiteKeys[k + 0].Letter, whiteKeys[k + 0].octave));
				chord.Add(new Note(whiteKeys[k + 2].Letter, whiteKeys[k + 2].octave));
				chord.Add(new Note(whiteKeys[k + 4].Letter, whiteKeys[k + 4].octave));

				int u = whiteKeys.Count - 1 - 4 - k;
				chord.Add(new Note(whiteKeys[u + 0].Letter, whiteKeys[u + 0].octave));
				chord.Add(new Note(whiteKeys[u + 2].Letter, whiteKeys[u + 2].octave));
				chord.Add(new Note(whiteKeys[u + 4].Letter, whiteKeys[u + 4].octave));

				score.Add(chord);
			}

			AddBar();
		}

		private void AddBar()
		{
			//score.Add(new Bar());
		}
	}
}
