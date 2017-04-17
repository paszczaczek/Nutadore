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
		private static readonly int numberOfKeys = numberOfWhiteKeys + numberOfBlackKeys;

		/// <summary>
		/// Lista wszystkich klawiszy na klawiaturze.
		/// </summary>
		public readonly List<Key> keys = new List<Key>();

		public event EventHandler<KeyboardEventArgs> EventHandler;

		public void FireEvent(Note note, KeyboardEventArgs.EventType eventType)
		{
			KeyboardEventArgs e = new KeyboardEventArgs(note, eventType);
			EventHandler?.Invoke(this, e);
		}

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
				double height = key.AddToCanvas(/*score, */this);
				keyboardHeight = Math.Max(height, keyboardHeight);
			}

			Height = keyboardHeight;

			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.Contra))
			//	key.Guess = true;
			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.Great))
			//	key.Hit = true;
			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.Small))
			//	key.Hit = false;
			//foreach (var key in keys.FindAll(key => key.note.octave == Note.Octave.OneLined))
			//	key.Down = true;
		}

		public void ConnectScore(Score score)
		{
			score.EventHandler += Score_Event;
		}

		private void Score_Event(object sender, ScoreEventArgs e)
		{
			Debug.Write(string.Format("{0}: ", e.eventType));
			foreach (Note note in e.notes)
			{
				Debug.Write(string.Format("{0} ", note.ToString()));
			}
			Debug.WriteLine("");

			List<Key> keys = FindKeys(e.notes);
			switch (e.eventType)
			{
				case ScoreEventArgs.EventType.HighlightedOn:
					keys.ForEach(key => key.Highlighted = true);
					break;
				case ScoreEventArgs.EventType.HighlightedOff:
					keys.ForEach(key => key.Highlighted = false);
					break;
				case ScoreEventArgs.EventType.Selected:
					Reset();
					keys.ForEach(key => key.Guess = true);
					break;
				//case ScoreEventArgs.EventType.MouseUp:
					//break;
			}
		}
		
		public new void KeyDown(Note note)
		{
			FireEvent(note, KeyboardEventArgs.EventType.KeyDown);
		}
		
		public new void KeyUp(Note note)
		{
			FireEvent(note, KeyboardEventArgs.EventType.KeyUp);
		}

		private List<Key> FindKeys(List<Note> notes)
		{
			return notes
				.Select(note => FindKey(note))
				.ToList();
		}

		private Key FindKey(Note note)
		{
			return keys.Find(key => key.note.Equals(note));
		}

		private void Keyboard_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Show();
		}

		public void Reset()
		{
			foreach (var key in keys)
			{
				key.Guess = false;
			}
		}
	}
}
