using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;

namespace Tests
{
	[TestClass]
	public class NoteTests
	{
		[TestMethod]
		public void NoteDurationTest()
		{
			MainWindow mw = Common.Initialize();

			foreach (Duration.Name durationName in Enum.GetValues(typeof(Duration.Name)))
			{
				Note note = new Note(
					Note.Letter.F,
					Accidental.Type.None,
					Note.Octave.TwoLined,
					new Duration(durationName));

				Note noteDotted = new Note(
					Note.Letter.B,
					Accidental.Type.None,
					Note.Octave.Small,
					new Duration(durationName, true),
					Staff.Type.Treble);

				Note noteDotted2 = new Note(
					Note.Letter.A,
					Accidental.Type.None,
					Note.Octave.Small,
					new Duration(durationName, true),
					Staff.Type.Bass);

				mw.score.Add(new Step()
					.AddVoice(note)
					.AddVoice(noteDotted)
					.AddVoice(noteDotted2));
			}

			Chord chord = new Chord()
				.AddNote(new Note(Note.Letter.C, Accidental.Type.None, Note.Octave.OneLined))
				.AddNote(new Note(Note.Letter.B, Accidental.Type.None, Note.Octave.TwoLined));
			chord.duration = new Duration(Duration.Name.Eighth, true);
			mw.score.Add(new Step()
				.AddVoice(chord));


			//// ćwierćnuty
			//Note quoterUp = new Note(
			//	Note.Letter.C,
			//	Accidental.Type.None,
			//	Note.Octave.OneLined,
			//	new Duration(Duration.Name.Quarter, true));
			//quoterUp.finger = 1;
			//quoterUp.stemDirection = Note.StemDirection.Up;

			//Note quoterDown = new Note(
			//	Note.Letter.A,
			//	Accidental.Type.None,
			//	Note.Octave.Small,
			//	new Duration(Duration.Name.Quarter, true));
			//quoterDown.finger = 2;
			//quoterDown.stemDirection = Note.StemDirection.Down;

			//mw.score.Add(new Step()
			//	.AddVoice(quoterUp)
			//	.AddVoice(quoterDown));

			//// ósemki
			//Note eighthUp = new Note(
			//	Note.Letter.D,
			//	Accidental.Type.None,
			//	Note.Octave.OneLined,
			//	new Duration(Duration.Name.Eighth, true));
			//eighthUp.finger = 1;
			//eighthUp.stemDirection = Note.StemDirection.Up;

			//Note eighthDown = new Note(
			//	Note.Letter.G,
			//	Accidental.Type.None,
			//	Note.Octave.Small,
			//	new Duration(Duration.Name.Eighth, true));
			//eighthDown.finger = 2;
			//eighthDown.stemDirection = Note.StemDirection.Down;

			//mw.score.Add(new Step()
			//	.AddVoice(eighthUp)
			//	.AddVoice(eighthDown));

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
				.AddNote(new Note(Note.Letter.C, Accidental.Type.Sharp, Note.Octave.Small))
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
