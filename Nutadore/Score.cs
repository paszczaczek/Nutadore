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
		public List<Step> steps = new List<Step>();

		private Step _currentStep;
		public Step CurrentStep
		{
			get
			{
				return _currentStep;
			}
			set
			{
				if (_currentStep != null)
					_currentStep.IsCurrent = false;
				_currentStep = value;
				if (_currentStep != null)
				{
					_currentStep.IsCurrent = true;
					FireEvent(_currentStep.SelectNotes(), ScoreEventArgs.EventType.Selected);
				}
			}
		}

		public Score()
		{
			base.ClipToBounds = true;
			Magnification = Properties.Settings.Default.ScoreMagnification;

			base.Background = Brushes.Transparent;
			base.SizeChanged += Score_SizeChanged;
			base.PreviewMouseWheel += Score_PreviewMouseWheel;
		}

		public event EventHandler<ScoreEventArgs> EventHandler;

		public void FireEvent(List<Note> notes, ScoreEventArgs.EventType eventType)
		{
			ScoreEventArgs e = new ScoreEventArgs(notes, eventType);
			EventHandler?.Invoke(this, e);
		}

		public void FireEvent(Note note, ScoreEventArgs.EventType eventType)
		{
			ScoreEventArgs e = new ScoreEventArgs(note, eventType);
			EventHandler?.Invoke(this, e);
		}

		public void ConnectKeyboard(Keyboard keyboard)
		{
			keyboard.EventHandler += Keyboard_Event;
		}

		private void Keyboard_Event(object sender, KeyboardEventArgs e)
		{
			switch (e.eventType)
			{
				case KeyboardEventArgs.EventType.KeyDown:
					CurrentStep.KeyDown(e.note);
					break;
				case KeyboardEventArgs.EventType.KeyUp:
					CurrentStep.KeyUp(e.note);
					break;
			}
		}

		private void Score_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Show();
		}

		private void Score_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
		
		public void Add(Step step)
		{
			steps.Add(step);
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
				//if (keyboard != null)
					//keyboard.Reset();
			}
		}

		public void Show()
		{
			// Czyscimy partyturę.
			Clear();

			// Rysujemy tyle podwójnych pięciolinii, ile potrzeba
			// aby zmieściły się na nich wszystkie znaki.
			double staffGrandTop = 0;
			bool allStepsIsShown = false;
			Step fromStep = steps.FirstOrDefault();
			while (!allStepsIsShown)
			{
				// Dodajemy nowy StaffGrand.
				StaffGrand staffGrand = new StaffGrand(this, staffGrandTop);
				staffGrands.Add(staffGrand);

				// Dodajemy do niego stepy.
				Step nextStep;
				staffGrandTop = staffGrand.AddSteps(fromStep, out nextStep);
				if (nextStep == fromStep)
				{
					// Żadnej nuty nie udało się narysować lub nie zmieścił się żaden takt lub więcej nut się nie zmieściło
					// Za wąska partytura - przerywany rysowanie.
					break;
				}
				fromStep = nextStep;

				// Czy wszystkie znaki zmieściły się na nim?
				allStepsIsShown = staffGrand.lastStep == steps.Last();
			}

			// Ustawiamy pierwszą nutę jak bieżącą.
			CurrentStep = steps.FirstOrDefault();
		}

		public void Clear()
		{
			// Usuwamy bieżącą pozycję.
			CurrentStep = null;

			// usuwamy wszystkie kroki
			foreach (Step step in steps)
				step.RemoveFromScore(this);

			// usuwamy znaki przykuczowe
			scale.RemoveFromScore(this);

			// usuwamy wszystkie podwójne pięciolinie
			foreach (var staffGrand in staffGrands)
				staffGrand.RemoveFromScore();
			staffGrands.Clear();

			// usuwamy pozostałe elemetny (klucze, znaki przykluczowe, itd.)
			base.Children.Clear();
		}
	}
}
