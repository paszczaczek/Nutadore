using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
	public class Key
	{
		public Key(Note.Octave octave, int keyNoInOctave)
		{
			if (keyNoInOctave < 0 || keyNoInOctave > 11)
				throw new ArgumentOutOfRangeException("keyNoInOctave", "Number klawisza w oktawie musi mieścić się zakresie [0, 11]");

			this.octave = octave;
			this.keyNoInOctave = keyNoInOctave;
		}

		/// <summary>
		/// Numer klawisza w oktawie. Przyjmuje wartości od 0 do 11. 0 to C, 11 to H.
		/// </summary>
		private readonly int keyNoInOctave;

		/// <summary>
		/// Oktawa klawisza.
		/// </summary>
		public readonly Note.Octave octave;

		/// <summary>
		/// Nazwę dźwięku klawisza.
		/// </summary>
		public Note.Letter Letter
		{
			get
			{
				switch (keyNoInOctave)
				{
					case 0:
					case 1:
						return Note.Letter.C;
					case 2:
					case 3:
						return Note.Letter.D;
					case 4:
						return Note.Letter.E;
					case 5:
					case 6:
						return Note.Letter.F;
					case 7:
					case 8:
						return Note.Letter.G;
					case 9:
					case 10:
						return Note.Letter.A;
					case 11:
						return Note.Letter.H;
					default:
						throw new ArgumentOutOfRangeException("keyNoInOctave");
				}
			}
		}

		/// <summary>
		/// Określa czy klawisz ma kolor biały.
		/// </summary>
		public bool IsWhite
		{
			get
			{
				return
					keyNoInOctave == 0 ||
					keyNoInOctave == 2 ||
					keyNoInOctave == 4 ||
					keyNoInOctave == 5 ||
					keyNoInOctave == 7 ||
					keyNoInOctave == 9 ||
					keyNoInOctave == 11;
			}
		}
		
		// Określa czy klawisz mak kolor czarny.
		public bool IsBlack
		{
			get
			{
				return !IsBlack;
			}
		}
	}
}
