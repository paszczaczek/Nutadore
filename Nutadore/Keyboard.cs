using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
	/// <summary>
	/// Klawiatura.
	/// </summary>
	public class Keyboard
	{
		//private static readonly int numberOfWhiteKeys = 52;
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
		}
	}
}
