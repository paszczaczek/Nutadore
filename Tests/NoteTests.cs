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
	}
}
