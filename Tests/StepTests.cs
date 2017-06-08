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
			MainWindow mw = Common.Initialize();

			// Akord z laseczką skierowaną do góry.
			Chord chordStemUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.Flat, Note.Octave.OneLined));
			chordStemUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			// Akord z laseczką sierowaną w dół.
			Chord chordStemDown = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.None, Note.Octave.Great))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.Flat, Note.Octave.Great));
			chordStemDown.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			mw.score.Add(
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

			mw.score.Add(
				new Nutadore.Step()
					.AddVoice(chord)
					.AddVoice(chordSemitone));

			mw.ShowDialog();

			Assert.IsTrue(true);
		}

		[TestMethod]
		public void EliminateAccidentalOverlapping()
		{
			MainWindow mw = Common.Initialize();

			Chord chordSteamUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.Flat, Note.Octave.OneLined));
			chordSteamUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			Chord chordSteamDown = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.Flat, Note.Octave.Small));
			chordSteamDown.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			mw.score.Add(new Nutadore.Step()
				.AddVoice(chordSteamUp)
				.AddVoice(chordSteamDown));

			mw.ShowDialog();

			Assert.IsTrue(true);
		}

		[TestMethod]
		public void NoteNamesAndFinges()
		{
			MainWindow mw = Common.Initialize();

			Chord chordSteamUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.Flat, Note.Octave.OneLined));
			chordSteamUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);
			int finger = 0;
			foreach (var note in chordSteamUp.notes)
				note.finger = (finger++ % 5) + 1;

			Chord chordSteamDown = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.E, Accidental.Type.Flat, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.F, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.G, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.A, Accidental.Type.Sharp, Note.Octave.Small))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.Flat, Note.Octave.Small));
			chordSteamDown.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			Note note1 = new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Great);
			Note note2 = new Note(Note.Letter.F, Accidental.Type.None, Note.Octave.Great);
			note2.finger = 1;

			mw.score.Add(new Step()
				.AddVoice(chordSteamUp)
				.AddVoice(chordSteamDown)
				.AddVoice(note1)
				.AddVoice(note2));

			foreach (var octave in Enum.GetValues(typeof(Note.Octave)).Cast<Note.Octave>())
			{
				Chord chord = new Chord();
				foreach (var letter in Enum.GetValues(typeof(Note.Letter)).Cast<Note.Letter>())
				{
					if (octave == Note.Octave.SubContra && letter < Note.Letter.A ||
						octave == Note.Octave.FiveLined && letter > Note.Letter.C)
						continue;
					chord.AddNote(new Note(letter, Accidental.Type.None, octave));
				}
				mw.score.Add(new Step().AddVoice(chord));
			}

			mw.ShowDialog();

			Assert.IsTrue(true);
		}
	}
}