using System;

namespace Nutadore
{
	public class KeyboardEventArgs : EventArgs
	{
		public enum EventType
		{
			KeyDown,
			KeyUp
		}

		public Note note;
		public EventType eventType;

		public KeyboardEventArgs(Note note, EventType eventType)
		{
			this.note = note;
			this.eventType = eventType;
		}
	}
}