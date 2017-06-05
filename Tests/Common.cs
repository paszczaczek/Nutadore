using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	public class Application
	{
		public MainWindow mw;
		public PrivateObject score;
	}

	static class Common
	{
		static public Application Initialize() {

			MainWindow mw = new Nutadore.MainWindow();
			mw.Topmost = true;
			mw.MainWindowTest();
			return new Application
			{
				mw = mw,
				score = new PrivateObject(mw, "score")
			};
		}
	}
}
