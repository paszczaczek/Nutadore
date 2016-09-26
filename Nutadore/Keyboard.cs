using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
	public class Keyboard
	{
		public static int numberOfWhiteKeys = 52;

		public static Note NoteOfKeyNumber(int keyNumber)
		{
			if (keyNumber < 1 || keyNumber > numberOfWhiteKeys)
				throw new ArgumentOutOfRangeException("keyNumber", "Poza zakresem!");
			else if (keyNumber >= 1 && keyNumber <= 2)
				return new Note(Note.Letter.A + keyNumber - 1, Note.Octave.SubContra);
			else if (keyNumber >= 3 && keyNumber <= 9)
				return new Note(Note.Letter.C + keyNumber - 3, Note.Octave.Contra);
			else if (keyNumber >= 10 && keyNumber <= 16)
				return new Note(Note.Letter.C + keyNumber - 10, Note.Octave.Great);
			else if (keyNumber >=  17 && keyNumber <= 23)
				return new Note(Note.Letter.C + keyNumber - 17, Note.Octave.Small);
			else if (keyNumber >=  24 && keyNumber <= 30)
				return new Note(Note.Letter.C + keyNumber - 24, Note.Octave.OneLined);
			else if (keyNumber >=  31 && keyNumber <= 37)
				return new Note(Note.Letter.C + keyNumber - 31, Note.Octave.TwoLined);
			else if (keyNumber >=  38 && keyNumber <= 44)
				return new Note(Note.Letter.C + keyNumber - 38, Note.Octave.ThreeLined);
			else if (keyNumber >=  45 && keyNumber <= 51)
				return new Note(Note.Letter.C + keyNumber - 45, Note.Octave.FourLined);
			else if (keyNumber >=  52)
				return new Note(Note.Letter.C, Note.Octave.FiveLined);

			return null;
		}
	}
}
