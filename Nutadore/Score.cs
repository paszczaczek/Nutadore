using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	public class Score : Canvas
	{
		public Scale scale = new Scale(Note.Letter.C, Scale.Type.Major);
		private List<StaffGrand> staffGrands = new List<StaffGrand>();
		public List<Sign> signs = new List<Sign>();

		public Score()
		{
			ClipToBounds = true;
			Magnification = Properties.Settings.Default.ScoreMagnification;

			base.Background = Brushes.Transparent;
			base.SizeChanged += _SizeChanged;
			base.PreviewMouseWheel += _PreviewMouseWheel;
		}

		private void _SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Show();
		}

		private void _PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			//base.OnPreviewMouseWheel(e);
			//if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
			//    Keyboard.IsKeyDown(Key.RightCtrl))
			{
				const double magnificationDelta = 0.2;
				bool changed = false;

				if (e.Delta < 0)
				{
					if (Magnification > 0.5)
					{
						Magnification -= magnificationDelta;
						changed = true;
					}
				}
				else
				{
					if (Magnification < 5)
					{
						Magnification += magnificationDelta;
						changed = true;
					}
				}

				if (changed)
				{
					Properties.Settings.Default.ScoreMagnification = Magnification;
					Properties.Settings.Default.Save();
				}
			}
		}

		public void Add(Sign sign)
		{
			signs.Add(sign);
		}

		public Note FindNextNote(Sign sign)
		{
			int idx = signs.IndexOf(sign);
			int idxNextNote = signs.FindIndex(idx, s => s is Note);

			return signs[idxNextNote] as Note;
		}

		private double magnification = 1.0;
		public double Magnification {
			get
			{
				return magnification;
			}
			set
			{
				magnification = value;
				Show();
			}
		}

		public void Show()
		{
			// Czyscimy partyturę.
			Clear();

			// Rysujemy tyle podwójnych pięciolinii, ile potrzeba
			// aby zmieściły się na nich wszystkie znaki.
			double staffGrandTop = 0;
			bool allSignsIsShown = false;
			Sign fromSign = signs.FirstOrDefault();
			while (!allSignsIsShown)
			{
				// Rysujemy nowy StaffGrand.
				StaffGrand staffGrand = new StaffGrand(this, staffGrandTop);
				staffGrands.Add(staffGrand);

				// Wyświetlamy na nim znaki.
				Sign nextSign;
				staffGrandTop = staffGrand.Show(fromSign, out nextSign);
				if (nextSign == fromSign)
				{
					// Żadnej nuty nie udało się narysować (nie zmieścił się żaden takt).
					// Za wąska partytura - przerywany rysowanie.
					break;
				}
				fromSign = nextSign;

				// Czy wszystkie znaki zmieściły się na nim?
				allSignsIsShown = staffGrand.lastSign == signs.Last();
			}
		}

		public void Clear()
		{
			// usuwamy wszystkie nuty
			signs.ForEach(sign => sign.Hide(this));

			// usuwamy wszystkie podwójne pięciolinie
			foreach (var staffGrand in staffGrands)
				staffGrand.Hide();
			staffGrands.Clear();

			// usuwamy pozostałe elemetny (klucze, znaki przykluczowe, itd.)
			Children.Clear();
		}
	}
}
