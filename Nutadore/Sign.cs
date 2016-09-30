using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
	abstract public class Sign
	{
		protected List<UIElement> uiElements = new List<UIElement>();


		public virtual double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			return left;
		}

		public virtual void Hide(Score score)
		{
			foreach (var uiElement in uiElements)
				score.Children.Remove(uiElement);

			uiElements.Clear();
			boundaryBox = Rect.Empty;
		}

		public virtual bool IsShown
		{
			get { return uiElements.Count > 0; }
		}

		protected void AddElement(Score score, UIElement uiElement, int zindex = 0, AddToBoundaryBox addToBoundaryBox = AddToBoundaryBox.No)
		{
			Canvas.SetZIndex(uiElement, zindex);
			score.Children.Add(uiElement);
			uiElements.Add(uiElement);
		}

		protected enum AddToBoundaryBox { Yes, No };
		public Rect boundaryBox {
			get;
			protected set;
		} = Rect.Empty;

		protected /*Rect*/double AddFetaGlyph(Score score, double glyphLeft, double glyphTop, string glyphCode, int zindex = 0, AddToBoundaryBox addToBoundaryBox = AddToBoundaryBox.No)
		{
			const string familyName = "feta26";
			double fontSize = 42 * score.Magnification;

			// Rysujemy symbol.
			UIElement uiElement = new TextBlock
			{
				FontFamily = new FontFamily(familyName),
				FontSize = fontSize,
				Text = glyphCode,
				Padding = new Thickness(0, 0, 0, 0),
				Margin = new Thickness(glyphLeft, glyphTop, 0, 0)
			};
			AddElement(score, uiElement, zindex);

			// Wyznaczamy wymiary symbolu.
			FormattedText formattedText = new FormattedText(
				glyphCode,
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface(familyName),
				fontSize,
				Brushes.Black);

			// Wyznaczamy granice w ktorych mieści się symbol (czarne piksele)
			// i dodajemy do granich pozostałych elementów.
			if (addToBoundaryBox == AddToBoundaryBox.Yes)
			{
				Rect glyphBoundaryBox = new Rect(
					glyphLeft,
					glyphTop + formattedText.Height + formattedText.OverhangAfter - formattedText.Extent,
					formattedText.Width,
					formattedText.Extent);
				if (boundaryBox.IsEmpty)
					boundaryBox = glyphBoundaryBox;
				else
					boundaryBox = Rect.Union(boundaryBox, glyphBoundaryBox);
			}

			//return boundaryRect;
			return glyphLeft + formattedText.Width;
		}

		protected Rectangle AddBoundaryBox(Score score, int zindex = 0)
		{
			// Dodajemy przezroczysty prostokąt w którym mieści się znak i podłączamy pod niego 
			// zdarzenie MouseEnter i MouseLeave.
			Rectangle boundaryRect = new Rectangle
			{
				Width = boundaryBox.Width,
				Height = boundaryBox.Height,
				Margin = new Thickness(boundaryBox.Left, boundaryBox.Top, 0, 0),
				Fill = Brushes.Transparent,
				Stroke = Brushes.Blue
			};
			AddElement(score, boundaryRect, zindex);

			return boundaryRect;
		}
	}
}
