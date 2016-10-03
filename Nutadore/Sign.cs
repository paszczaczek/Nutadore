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
		private List<UIElement> elements = new List<UIElement>();
		private List<UIElement> boundsElements = new List<UIElement>();
		private Rectangle boundsRectangle;
		public Rect Bounds
		{
			get
			{
				return new Rect(
					boundsRectangle.Margin.Left,
					boundsRectangle.Margin.Top,
					boundsRectangle.Width,
					boundsRectangle.Height);
			}
		}
		public bool focusable = false;

		public virtual double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			return left;
		}

		public virtual void Hide(Score score)
		{
			foreach (var uiElement in elements)
				score.Children.Remove(uiElement);
			elements.Clear();

			boundsElements.Clear();
			boundsRectangle = null;
		}

		public virtual bool IsShown
		{
			get { return elements.Count > 0; }
		}

		protected void AddElement(Score score, UIElement uiElement, int zindex = 0)
		{
			score.Children.Add(uiElement);
			Canvas.SetZIndex(uiElement, zindex);
			elements.Add(uiElement);
		}

		protected double AddFetaGlyph(Score score, double glyphLeft, double glyphTop, string glyphCode, int zindex = 0)
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

			// Wyznaczamy granice w ktorych mieszczą się czarne piksele symbolu.
			Rect boundsGlyph = new Rect(
				glyphLeft,
				glyphTop + formattedText.Height + formattedText.OverhangAfter - formattedText.Extent,
				formattedText.Width,
				formattedText.Extent);
			ExtendBounds(score, boundsGlyph, 100);
			boundsElements.Add(uiElement);

			// Zwracamy nowe położenie kursora.
			return glyphLeft + formattedText.Width;
		}

		protected void ExtendBounds(Score score, Rect extendBy, int zindex = 0)
		{
			// Czy już został utworzona ramka bounds?
			if (boundsRectangle == null)
			{
				// Nie, jescze nie. Tworzymy ją i podczepiamy obsługę zdarzeń.
				//boundsRectangle = new Rectangle
				boundsRectangle = new Rectangle
				{
					Width = extendBy.Width,
					Height = extendBy.Height,
					Margin = new Thickness(extendBy.Left, extendBy.Top, 0, 0),
					Fill = Brushes.Transparent
					//Stroke = zindex == 101 ? Brushes.Green : Brushes.Blue
				};
				if (focusable)
				{
					boundsRectangle.MouseEnter += Bounds_MouseEnter;
					boundsRectangle.MouseLeave += Bounds_MouseLeave;
				}
				AddElement(score, boundsRectangle, zindex);
			}
			else
			{
				// Tak, już była utworzona ramka bounds. Powiąkszamy ją o extenBy
				//Rect boundsRect = new Rect(bounds.Margin.Left, bounds.Margin.Top, bounds.Width, bounds.Height);
				Rect boundExtended = Rect.Union(Bounds, extendBy);
				boundsRectangle.Width = boundExtended.Width;
				boundsRectangle.Height = boundExtended.Height;
				Thickness margin = boundsRectangle.Margin;
				margin.Left = boundExtended.Left;
				margin.Top = boundExtended.Top;
				boundsRectangle.Margin = margin;
			}
		}

		public virtual void Bounds_MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (var be in boundsElements)
			{
				if (be is TextBlock)
					(be as TextBlock).Foreground = Brushes.Black;
			}
		}

		public virtual void Bounds_MouseEnter(object sender, MouseEventArgs e)
		{
			foreach (UIElement be in boundsElements)
			{
				if (be is TextBlock)
					(be as TextBlock).Foreground = Brushes.DarkGray;
			}
		}
	}
}
