using Nutadore;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nutadore.Tests
{
    [TestClass()]
    public class MiscTests
    {
        [TestMethod()]
        public void Test1()
        {
            Note note = Note.highest.Copy();
            note.Transpose(0);
            Assert.AreEqual(note, new Note(Note.Letter.C, Note.Octave.FiveLined));
        }
    }
}