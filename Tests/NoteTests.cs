using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;

namespace Tests
{
	[TestClass]
	public class NoteTests
	{
		[TestMethod]
		public void NoteScale()
		{
			MainWindow mw = Common.Initialize();

			mw.score.Add(
				new Step().AddVoice(
					new Note(Note.Letter.A, Accidental.Type.None, Note.Octave.OneLined)));

			mw.ShowDialog();
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void HeadOffsetTest()
		{
			MainWindow mw = Common.Initialize();

			Chord chordSteamUp = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.D, Accidental.Type.None, Note.Octave.OneLined));
			chordSteamUp.notes.ForEach(note => note.stemDirection = Note.StemDirection.Up);

			Chord chordSteamDown = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small)
				{
					duration = new Duration(Duration.Name.Half)
				})
				.AddNote(new Note(Note.Letter.D, Accidental.Type.Flat, Note.Octave.Small));
			chordSteamDown.notes.ForEach(note => note.stemDirection = Note.StemDirection.Down);

			mw.score.showNotesName = true;
			mw.score.Add(new Nutadore.Step()
				.AddVoice(chordSteamUp)
				.AddVoice(chordSteamDown));

			mw.ShowDialog();

			Assert.IsTrue(true);
		}
	}
}
