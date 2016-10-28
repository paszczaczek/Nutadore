using System;
using System.Collections.Generic;
using System.Globalization;
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
		private static readonly SolidColorBrush whiteKeyUpBrush = Brushes.Snow;
		private static readonly SolidColorBrush blackKeyUpBrush = Brushes.Black;

		// Możliwe stany:
		// Up (biały/czarny)
		// Up Guess (żółty jasny/ciemnu)
		// Down (mocno szary jasny/ciemny)
		// Down Hit (zielony jasny/ciemny)
		// Down !Hit (czerwony jasny/ciemny)
		// Dodatkowo podświetlenie rozjaśniające kolory

		private bool _highlighted;
		public bool Highlighted
		{
			get { return _highlighted; }
			set { _highlighted = value; SetColor(); }
		}

		private bool _guess;
		public bool Guess
		{
			get { return _guess; }
			set { _guess = value;  SetColor(); }
		}

		private bool? _guessed;
		public bool? Guessed
		{
			get { return _guessed; }
			set
			{
				_guessed = value;
				if (_guessed != null)
					_down = true;
				SetColor();
			}
		}

		private bool _down;
		public bool Down
		{
			get { return _down; }
			set
			{
				_down = value;
				if (_down == false)
					_guessed = null;
				SetColor();
			}
		}

		private Rectangle highlightRectangle;

		private readonly int keyNoInOctave;
		public readonly Note note;
		public readonly bool isWhite;

		public Key(int keyNo)
		{
			// Wyznaczam oktawe i numer klawisza w oktawie dla klawisza.
			const int keysInOctave = 12;
			Note.Octave octave;
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
			Note.Letter letter;
			Accidental.Type accidental = Accidental.Type.None;
			switch (keyNoInOctave)
			{
				case 0:
					letter = Note.Letter.C;
					break;
				case 1:
					accidental = Accidental.Type.Sharp;
					letter = Note.Letter.C;
					break;
				case 2:
					letter = Note.Letter.D;
					break;
				case 3:
					accidental = Accidental.Type.Sharp;
					letter = Note.Letter.D;
					break;
				case 4:
					letter = Note.Letter.E;
					break;
				case 5:
					letter = Note.Letter.F;
					break;
				case 6:
					accidental = Accidental.Type.Sharp;
					letter = Note.Letter.F;
					break;
				case 7:
					letter = Note.Letter.G;
					break;
				case 8:
					accidental = Accidental.Type.Sharp;
					letter = Note.Letter.G;
					break;
				case 9:
					letter = Note.Letter.A;
					break;
				case 10:
					accidental = Accidental.Type.Sharp;
					letter = Note.Letter.A;
					break;
				case 11:
					letter = Note.Letter.H;
					break;
				default:
					throw new ArgumentOutOfRangeException("keyNoInOctave");
			}

			// Wyznaczam nute skojarzona z klawiszem.
			note = new Note(letter, accidental, octave);

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

		public double AddToCanvas(Keyboard keyboard)
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
			left += (int)note.octave * whiteWidth * 7;
			// przesunięcie wynikające z tego że w oktawie SubContra są tylko klawisze A, A# i H
			left -= whiteWidth * 5;

			// Rysuje klawisz.
			Rectangle keyRectangle = new Rectangle
			{
				Width = width,
				Height = height,
				Margin = new Thickness(left, 0, 0, 0),
				Fill = isWhite ? whiteKeyUpBrush : blackKeyUpBrush,
				Stroke = Brushes.Black,
				StrokeThickness = isWhite ? 0.2 : 0.8
			};
			Canvas.SetZIndex(keyRectangle, isWhite ? 0 : 2);
			keyboard.Children.Add(keyRectangle);

			// Transparentny prostokąt modyfikujący kolor klawisza i reagujący na myszę.
			highlightRectangle = new Rectangle
			{
				Width = width,
				Height = height,
				Margin = new Thickness(left, 0, 0, 0),
				Fill = Brushes.Transparent,
				Stroke = Brushes.Transparent,
				Tag = keyboard
			};
			Canvas.SetZIndex(highlightRectangle, isWhite ? 1 : 3);
			keyboard.Children.Add(highlightRectangle);
			highlightRectangle.MouseEnter += MouseEnter;
			highlightRectangle.MouseLeave += MouseLeave;
			highlightRectangle.MouseDown += MouseDown;
			highlightRectangle.MouseUp += MouseUp;

			// Rysuję nazwy oktaw.
			double keyboardHeight = whiteHeight;
			if (note.letter == Note.Letter.C && isWhite)
			{
				const string familyName = "Consolas";
				double fontSize = 12;
				TextBlock octaveNameTextBlock = new TextBlock
				{
					FontFamily = new FontFamily(familyName),
					FontSize = fontSize,
					Text = note.octave.ToString(),
					Foreground = Brushes.Red,
					//Margin = new Thickness(line.X1, line.Y2, 0, 0)
					Margin = new Thickness(left + 3, height, 0, 0)
				};
				// Wyznaczamy wysokość napisu.
				FormattedText ft = new FormattedText(
					octaveNameTextBlock.Text,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(familyName),
					fontSize,
					Brushes.Black);
				keyboardHeight += ft.Height;
				keyboard.Children.Add(octaveNameTextBlock);
			}

			// Rysuję linie oddzielające klawisze i linie oddzielające oktawy.
			if (isWhite)
			{
				Line line = new Line
				{
					X1 = left,
					Y1 = 0,
					X2 = left,
					//Y2 = height,
					Y2 = keyboardHeight,
					Stroke = note.letter == Note.Letter.C && isWhite ? Brushes.Red : Brushes.Black,
					StrokeThickness = note.letter == Note.Letter.C && isWhite ? 1.0 : 0.4
				};
				Canvas.SetZIndex(line, 0);
				keyboard.Children.Add(line);
			}

			SetColor();

			return keyboardHeight;
		}

		private void MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Highlighted = true;
		}

		private void MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Highlighted = false;
		}

		private void MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Down = true;
			Guessed = Guess;
			Keyboard keyboard = (sender as Rectangle).Tag as Keyboard;
			keyboard.KeyDown(note);
		}

		private void MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Down = false;
			Guessed = null;
			Keyboard keyboard = (sender as Rectangle).Tag as Keyboard;
			keyboard.KeyUp(note);
		}

		private void SetColor()
		{
			SolidColorBrush brush = Brushes.Black.Clone();
			brush.Opacity = 1;

			if (Guess)
			{
				if (Highlighted)
				{
					brush.Color = Colors.Gold;
				}
				else
				{
					brush.Color = Colors.Yellow;
				}
			}
			else
			{
				if (Highlighted)
				{
					brush.Color = Colors.LightGray;
				}
			}

			if (Down)
			{
				if (Guessed == true)
				{
					brush.Color = Colors.LightGreen;
				}
				else if (Guessed == false)
				{
					brush.Color = Colors.Red;
				}
				else if (Guessed == null)
				{
					brush.Color = Colors.Gray;
				}
			}

			if (brush.Color == Colors.Black)
			{
				brush.Opacity = 0;
			}

			highlightRectangle.Fill = brush;
		}
	}
}
