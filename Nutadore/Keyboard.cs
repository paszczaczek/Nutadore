using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		//public Score score;

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

			double keyboardHeight = 0;
			foreach (Key key in keys)
			{
				double height = key.Show(/*score, */this);
				keyboardHeight = Math.Max(height, keyboardHeight);
			}

			Height = keyboardHeight;

			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.Great))
			//	key.MarkAs(Key.State.Down);
			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.Small))
			//	key.MarkAs(Key.State.Hit);
			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.OneLined))
			//	key.MarkAs(Key.State.Missed);
		}

		public void SubscribeScoreEvents(Score score)
		{
			score.Event += Score_Event;
		}

		private void Score_Event(object sender, ScoreEventArgs e)
		{
			Debug.Write(string.Format("{0}: ", e.eventType));
			foreach (Note note in e.notes)
			{
				Debug.Write(string.Format("{0} ", note.ToString()));
			}
			Debug.WriteLine("");
		}

		private void Keyboard_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Show();
		}

		public void Check(Note note)
		{
			Key key = keys.Find(k => k.note.Equals(note));
			if (key.state == Key.State.Down)
				key.MarkAs(Key.State.Hit);
			else
				key.MarkAs(Key.State.Missed);
			//score.currentStep.KeyDown(key);
		}

		public void MarkAs(Sign sign, Key.State state)
		{
			foreach (var key in keys)
			{
				if (key.note.InChord(sign))
					key.MarkAs(state);
			}
		}

		public void Reset()
		{
			foreach (var key in keys)
			{
				key.MarkAsHighlighted(false);
				key.MarkAs(Key.State.Up);
			}
		}
	}
}
