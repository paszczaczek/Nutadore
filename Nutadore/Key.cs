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
		public Key(int keyNo)
		{
			this.keyNo = keyNo;

			// Wyznaczam oktawe i numer klawisza w oktawie dla klawisza.
			const int keysInOctave = 12;
			if (keyNo >= 0 && keyNo <= 2)
			{
				// Subcontra A, A#, H
				octave = Note.Octave.SubContra;
				keyNoInOctave = 9 + keyNo;
			}
			else if (keyNo >= 3 && keyNo <= 86)
			{
				// Great..FourLined C..H
				octave = (Note.Octave)((keyNo - 3) / keysInOctave + 1);
				keyNoInOctave = (keyNo - 3) % keysInOctave;
			}
			else if (keyNo == 87)
			{
				// FiveLined C
				octave = Note.Octave.FiveLined;
				keyNoInOctave = 0;
			}
			else
			{
				string message = string.Format("Numer klawisza {0} spoza zakresu [0, 88]!", keyNo);
				throw new ArgumentOutOfRangeException("keyNo", message);
			}

			// Wyznaczam literę nuty dla klawisza.
			switch (keyNoInOctave)
			{
				case 0:
					letter = Note.Letter.C;
					break;
				case 1:
					// TODO krzyżyk
					letter = Note.Letter.C;
					break;
				case 2:
					letter = Note.Letter.D;
					break;
				case 3:
					// TODO krzyżyk
					letter = Note.Letter.D;
					break;
				case 4:
					letter = Note.Letter.E;
					break;
				case 5:
					letter = Note.Letter.F;
					break;
				case 6:
					// TODO krzyżyk
					letter = Note.Letter.F;
					break;
				case 7:
					letter = Note.Letter.G;
					break;
				case 8:
					// TODO krzyżyk
					letter = Note.Letter.G;
					break;
				case 9:
					letter = Note.Letter.A;
					break;
				case 10:
					// TODO krzyżyk
					letter = Note.Letter.A;
					break;
				case 11:
					letter = Note.Letter.H;
					break;
				default:
					throw new ArgumentOutOfRangeException("keyNoInOctave");
			}

			// Wyznaczam kolor klawisza.
			isWhite =
				keyNoInOctave == 0 ||
				keyNoInOctave == 2 ||
				keyNoInOctave == 4 ||
				keyNoInOctave == 5 ||
				keyNoInOctave == 7 ||
				keyNoInOctave == 9 ||
				keyNoInOctave == 11;
		}

		private readonly int keyNo;
		private readonly int keyNoInOctave;

		public readonly Note.Letter letter;
		public readonly Note.Octave octave;
		public readonly bool isWhite;

		public void Show(Keyboard keyboard)
		{
			// szerokość klawiszy białych i czarnych
			double whiteWidth = keyboard.ActualWidth / Keyboard.numberOfWhiteKeys;
			double blackWidth = whiteWidth * 0.6;

			// wysokość klawiszy białych i czarnych
			double whiteHeight = whiteWidth * 4.0;
			double blackHeight = whiteHeight * 0.6;

			// serokość i długość klawisza
			double width = isWhite ? whiteWidth : blackWidth;
			double height = isWhite ? whiteHeight : blackHeight;

			// położenie klawisza wynikające z położenia w oktawie
			double left = 0;
			switch(keyNoInOctave)
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
			// przesunięcie wynikające z oktawy
			left += (int)octave * whiteWidth * 7;
			// przesunięcie wynikające z tego że w oktawie SubContra są tylko klawisze A, A# i H
			left -= whiteWidth * 5;

			// Rysuje klawisz.
			Rectangle rect = new Rectangle
			{
				Width = width,
				Height = height,
				Margin = new Thickness(left, 0, 0, 0),
				Fill = isWhite ? Brushes.LightGray : Brushes.Black,
				//Stroke = Brushes.Black,
				//StrokeThickness = 1
			};
			Canvas.SetZIndex(rect, isWhite ? 0 : 1);
			keyboard.Children.Add(rect);

			Line line = new Line
			{
				X1 = left,
				Y1 = 0,
				X2 = left,
				Y2 = height,
				Stroke = Brushes.Black,
				StrokeThickness = letter == Note.Letter.C && isWhite ? 1.0 : 0.4
			};
			Canvas.SetZIndex(line, 2);
			keyboard.Children.Add(line);
		}
	}
}
