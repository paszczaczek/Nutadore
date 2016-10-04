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
		private static readonly int numberOfBlackKeys = 36;
		public static readonly int numberOfKeys = numberOfWhiteKeys + numberOfBlackKeys;

		public Score score;

		/// <summary>
		/// Lista wszystkich klawiszy na klawiaturze.
		/// </summary>
		public readonly List<Key> keys = new List<Key>();

		public Keyboard()
		{
			// Utworzenie klawiszy
			for (int keyNo = 0; keyNo < numberOfKeys; keyNo++)
				keys.Add(new Key(keyNo));

			// Podłączenie eventów
			base.Background = Brushes.Transparent;
			base.SizeChanged += Keyboard_SizeChanged;
		}

		public void Show()
		{
			base.Children.Clear();
			foreach (Key key in keys)
			{
				key.Show(score, this);
			}
		}

		private void Keyboard_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Show();
		}

		public void Press(Sign sign)
		{
			Note note = sign as Note;
			if (note != null)
			{
				foreach (var key in keys)
				{
					if (key.note.Equals(note))
						key.Press();
				}
			}
		}

		public void Release(Sign sign)
		{
			Note note = sign as Note;
			if (note != null)
			{
				foreach (var key in keys)
				{
					if (key.note.Equals(note))
						key.Release();
				}
			}
		}
	}
}
