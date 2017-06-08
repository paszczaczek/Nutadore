using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nutadore
{
	/// <summary> 
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			score.ConnectKeyboard(keyboard);
			keyboard.ConnectScore(score);

			//LyParserTest2();

		}

		public void MainWindowTest()
		{
			InitializeComponent();

			score.ConnectKeyboard(keyboard);
			keyboard.ConnectScore(score);
		}

		private void LyParserTest2()
		{
			LilyPond.Parse(@"Misc\bach-air-in-d-major.ly", score);
		}
	}
}
