using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;

namespace Nutadore.Tests
{
	[TestClass()]
	public class StepTests
	{
		[TestMethod()]
		public void EliminateHeadsOverlapping()
		{
			Application app = Common.Initialize();

			Chord chordStemUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.OneLined));
			chordStemUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			Chord chordStemDown = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.Great));

			chordStemDown.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			app.score.Invoke("Add",
				new Nutadore.Step()
					.AddVoice(chordStemUp)
					.AddVoice(chordStemDown));
			app.mw.ShowDialog();

			Assert.IsTrue(true);
		}
	}
}