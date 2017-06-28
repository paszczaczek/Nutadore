using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nutadore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	static class Common
	{
		static public MainWindow Initialize() {

			MainWindow mw = new MainWindow();
			mw.Topmost = true;
			//mw.MainWindowTest();

			return mw;			
		}
	}
}
