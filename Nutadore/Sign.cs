using System;
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
		protected List<UIElement> elements = new List<UIElement>();
		public Rect bounds { get; protected set; } = Rect.Empty;

		public abstract double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, Step step, double left);

		public virtual void RemoveFromScore(Score score)
		{
			foreach (var uiElement in elements)
				score.Children.Remove(uiElement);
			elements.Clear();

			//highlightElements.Clear();
			bounds = Rect.Empty;
		}

		protected void AddElement(Score score, UIElement element, int zindex = 0)
		{
			score.Children.Add(element);
			Canvas.SetZIndex(element, zindex);
			elements.Add(element);
		}

		protected double AddGlyph(Score score, double glyphLeft, double glyphTop, string glyphCode, int zindex = 0)
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
			ExtendBounds(boundsGlyph);
			//highlightElements.Add(uiElement);

			// Zwracamy nowe położenie kursora.
			return glyphLeft + formattedText.Width;
		}

		protected void ExtendBounds(Rect extendBy)
		{
			bounds = Rect.Union(bounds, extendBy);
		}
	}
}
