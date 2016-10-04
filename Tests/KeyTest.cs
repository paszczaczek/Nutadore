using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;

namespace Tests
{
	/// <summary>
	/// Summary description for KeyTest
	/// </summary>
	[TestClass]
	public class KeyTest
	{
		public KeyTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void OctaveAndLetterTest()
		{
			// SubContra A
			Key key = new Key(0);
			Assert.AreEqual(key.note.octave, Note.Octave.SubContra);
			Assert.AreEqual(key.note.letter, Note.Letter.A);

			// SubContra A#
			key = new Key(1);
			Assert.AreEqual(key.note.octave, Note.Octave.SubContra);
			Assert.AreEqual(key.note.letter, Note.Letter.A);

			// SubContra H
			key = new Key(2);
			Assert.AreEqual(key.note.octave, Note.Octave.SubContra);
			Assert.AreEqual(key.note.letter, Note.Letter.H);


			// Contra C
			key = new Key(3);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.C);

			// Contra C#
			key = new Key(4);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.C);

			// Contra D
			key = new Key(5);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.D);

			// Contra D#
			key = new Key(6);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.D);

			// Contra E
			key = new Key(7);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.E);

			// Contra F
			key = new Key(8);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.F);

			// Contra F#
			key = new Key(9);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.F);

			// Contra G
			key = new Key(10);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.G);

			// Contra G#
			key = new Key(11);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.G);

			// Contra A
			key = new Key(12);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.A);

			// Contra A#
			key = new Key(13);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.A);

			// Contra H
			key = new Key(14);
			Assert.AreEqual(key.note.octave, Note.Octave.Contra);
			Assert.AreEqual(key.note.letter, Note.Letter.H);
		}
	}
}
