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
		public Rect bounds { get; protected set; } = Rect.Empty;

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
			bounds = Rect.Empty;
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
			ExtendBounds(score, boundsGlyph);
			boundsElements.Add(uiElement);

			// Zwracamy nowe położenie kursora.
			return glyphLeft + formattedText.Width;
		}

		protected void ExtendBounds(Score score, Rect extendBy)
		{
			bounds = Rect.Union(bounds, extendBy);
		}

		public void AddFocusRectangle(Score score, int zindex)
		{
			Rectangle boundsRectangle = new Rectangle
			{
				Width = bounds.Width,
				Height = bounds.Height,
				Margin = new Thickness(bounds.Left, bounds.Top, 0, 0),
				Fill = Brushes.Transparent/*,
				Stroke = zindex == 101 ? Brushes.Green : Brushes.Blue*/
			};
			boundsRectangle.MouseEnter += Bounds_MouseEnter;
			boundsRectangle.MouseLeave += Bounds_MouseLeave;
			AddElement(score, boundsRectangle, zindex);
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
