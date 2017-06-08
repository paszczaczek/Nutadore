using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;

namespace Tests
{
	[TestClass]
	public class LilyPondTests
	{
		[TestMethod]
		public void BachAirInDMajor()
		{
			MainWindow mw = Common.Initialize();
			LilyPond.Parse(@"Misc\bach-air-in-d-major.ly", mw.score);
			mw.ShowDialog();
			Assert.IsTrue(true);
		}
	}
}
