using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nutadore
{
	public class ScoreEventArgs
	{
		public enum EventType
		{
			MouseEnter,
			MouseLeave,
			MouseDown,
			MouseUp
		};

		public List<Note> notes = new List<Note>();
		public EventType eventType;

		public ScoreEventArgs(List<Note> notes, EventType eventType)
		{
			this.notes = notes;
			this.eventType = eventType;
		}

		public ScoreEventArgs(Note note, EventType eventType)
		{
			this.notes.Add(note);
			this.eventType = eventType;
		}
	}
}
