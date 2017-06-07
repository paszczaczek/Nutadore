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
		[TestMethod]
		public void EliminateHeadsOverlapping()
		{
			Application app = Common.Initialize();

			// Akord z laseczką skierowaną do góry.
			Chord chordStemUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.OneLined));
			chordStemUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			// Akord z laseczką sierowaną w dół.
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

			// Akord z c i c# - powinno nastąpić odwrócenie.
			Chord chordSemitone = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined));
			chordSemitone.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			// Akord w którym nie powinno wystąpić odwrócenie nuty.
			Chord chord = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.None, Note.Octave.Small));
			chord.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			app.score.Invoke("Add",
				new Nutadore.Step()
					.AddVoice(chord)
					.AddVoice(chordSemitone));

			app.mw.ShowDialog();

			Assert.IsTrue(true);
		}

		[TestMethod]
		public void EliminateAccidentalOverlapping()
		{
			Application app = Common.Initialize();

			Chord chordSteamUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.OneLined));
			chordSteamUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			Chord chordSteamDown = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.H, Accidental.Type.Flat, Note.Octave.Small));
			chordSteamDown.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			app.score.Invoke("Add", new Nutadore.Step()
				.AddVoice(chordSteamUp)
				.AddVoice(chordSteamDown));

			app.mw.ShowDialog();

			Assert.IsTrue(true);
		}
	}
}