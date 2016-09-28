using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
	/// <summary>
	/// Klawiatura.
	/// </summary>
	public class Keyboard : Canvas
	{
		public static readonly int numberOfWhiteKeys = 52;
		//private static readonly int numberOfBlackKeys = 36;

		/// <summary>
		/// Lista wszystkich klawiszy na klawiaturze.
		/// </summary>
		public readonly List<Key> keys = new List<Key>();

		public Keyboard()
		{
			// Subcontra A, A#, H
			keys.Add(new Key(Note.Octave.SubContra, 9));
			keys.Add(new Key(Note.Octave.SubContra, 10));
			keys.Add(new Key(Note.Octave.SubContra, 11));

			// Contra .. FourLined
			for (Note.Octave octave = Note.Octave.Contra; 
				octave <= Note.Octave.FourLined; 
				octave++)
			{
				for (int keyNoInOctave = 0; keyNoInOctave < 12; keyNoInOctave++)
				{
					int keyNoInKeyboard = 3 + ((int)octave - 1) * 12 + keyNoInOctave;
					keys.Add(new Key(octave, keyNoInOctave));
				}
			}

			// FileLined C
			keys.Add(new Key(Note.Octave.FiveLined, 0));

			// Podłączenie eventów
			base.Background = Brushes.Transparent;
			base.SizeChanged += Keyboard_SizeChanged;
		}

		public void Show()
		{
			base.Children.Clear();
			foreach (Key key in keys)
			{
				key.Show(this);
			}
			//keys[0].Show(this);
			//keys[1].Show(this);
			//keys[2].Show(this);

			//keys[3].Show(this);
			//keys[4].Show(this);
			//keys[5].Show(this);
			//keys[6].Show(this);
			//keys[7].Show(this);
			//keys[8].Show(this);
			//keys[9].Show(this);
			//keys[10].Show(this);
			//keys[11].Show(this);
			//keys[12].Show(this);
			//keys[13].Show(this);
			//keys[14].Show(this);

			//keys[15].Show(this);
		}

		private void Keyboard_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Show();
		}


	}
}
