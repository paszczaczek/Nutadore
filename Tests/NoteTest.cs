using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;

namespace Tests
{
	[TestClass]
	public class NoteTest
	{
		[TestMethod]
		public void NoteEqualTest()
		{
			Note cs1 = new Note(Note.Letter.C, Note.Accidental.None, Note.Octave.OneLined);
			Note df1 = new Note(Note.Letter.D, Note.Accidental.None, Note.Octave.OneLined);

			Assert.IsTrue(cs1.Equals(cs1));
			// Test nie skończony!
		}
	}
}
