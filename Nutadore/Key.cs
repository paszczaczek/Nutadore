using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Key
	{
		private static readonly double keyHeightRatio = 5.0;

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

		private int KeyNoInKeyboard
		{
			get
			{
				if (octave == Note.Octave.SubContra)
					return keyNoInOctave - 9;
				else 
					return ((int)octave - 1) * 12 + keyNoInOctave;
			}
		}

		public void Show(Keyboard keyboard)
		{
			double whiteWidth = keyboard.ActualWidth / Keyboard.numberOfWhiteKeys;
			double blackWidth = whiteWidth * 2 / 3;

			bool isWhite = IsWhite;
			double width = isWhite ? whiteWidth : blackWidth;
			double height = width * keyHeightRatio;

			double left = 0;
			switch(KeyNoInKeyboard)
			{
				case 0: left = 0; break;
				case 1: left = whiteWidth - blackWidth / 2; break;
				case 2: left = whiteWidth; break;
				case 3: left = whiteWidth * 2 - blackWidth / 2; break;
				case 4: left = whiteWidth * 2; break;
				case 5: left = whiteWidth * 3; break;
				case 6: left = whiteWidth * 4 - blackWidth / 2; break;
				case 7: left = whiteWidth * 4; break;
				case 8: left = whiteWidth * 5 - blackWidth / 2; break;
				case 9: left = whiteWidth * 5; break;
				case 10: left = whiteWidth * 6 - blackWidth / 2; break;
				case 11: left = whiteWidth * 6; break;
			}
			if (octave > Note.Octave.SubContra)
				left += whiteWidth * 2 + ((int)octave - 1) * whiteWidth * 7;

			Rectangle key = new Rectangle
			{
				Width = width,
				Height = height,
				Margin = new Thickness(left, 0, 0, 0),
				Fill = isWhite ? Brushes.LightGray : Brushes.Black,
				Stroke = Brushes.Black,
				StrokeThickness = 1
			};
			Canvas.SetZIndex(key, isWhite ? 0 : 1);
			keyboard.Children.Add(key);
		}
	}
}
