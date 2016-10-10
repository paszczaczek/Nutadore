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
		private static Brush currentBrush = Brushes.LightSeaGreen;
		private static Brush highlightBrush = Brushes.Gray;
		//private static Brush boundsBrush = Brushes.Gray;

		private List<UIElement> elements = new List<UIElement>();
		private List<UIElement> highlightElements = new List<UIElement>();
		private Rectangle highlightRectangle;
		public Rect bounds { get; protected set; } = Rect.Empty;

		private bool isCurrent;
		private bool isHighlighted;

		public virtual double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			return left;
		}

		public virtual void Hide(Score score)
		{
			foreach (var uiElement in elements)
				score.Children.Remove(uiElement);
			elements.Clear();

			highlightElements.Clear();
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
			highlightElements.Add(uiElement);

			// Zwracamy nowe położenie kursora.
			return glyphLeft + formattedText.Width;
		}

		protected void ExtendBounds(Score score, Rect extendBy)
		{
			bounds = Rect.Union(bounds, extendBy);
		}

		public void AddHighlightRectangle(Score score, Staff trebleStaff, Staff bassStaff, int zindex)
		{

			double top = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(6));
			double bottom = bassStaff.StaffPositionToY(StaffPosition.ByLegerBelow(4));
			highlightRectangle = new Rectangle
			{
				Width = bounds.Width,
				//Height = bounds.Height,
				Height = bottom - top,
				//Margin = new Thickness(bounds.Left, bounds.Top, 0, 0),
				Margin = new Thickness(bounds.Left, top, 0, 0),
				Fill = Brushes.Transparent,
				//Fill = boundsBrush,
				//Opacity = 0.1,
				Stroke = Brushes.Transparent
			};
			highlightRectangle.MouseEnter += MouseEnter;
			highlightRectangle.MouseLeave += MouseLeave;
			AddElement(score, highlightRectangle, zindex);
		}

		public virtual void MouseEnter(object sender, MouseEventArgs e)
		{
			//foreach (UIElement he in highlightElements)
			//{
			//	if (he is TextBlock)
			//		(he as TextBlock).Foreground = highlightBrush;
			//}
			//Rectangle highlightRectangle = sender as Rectangle;
			isHighlighted = true;
			SetColor();
			//highlightRectangle.Fill = highlightBrush;
			//highlightRectangle.Opacity = 0.3;
		}

		public virtual void MouseLeave(object sender, MouseEventArgs e)
		{
			//foreach (var he in highlightElements)
			//{
			//	if (he is TextBlock)
			//		(he as TextBlock).Foreground = Brushes.Black;
			//}
			//Rectangle highlightRectangle = sender as Rectangle;
			//highlightRectangle.Fill = highlightBrush;
			//(sender as Rectangle).Opacity = 0.1;
			isHighlighted = false;
			SetColor();
		}

		public void MarkAsCurrent(bool isCurrent)
		{
			this.isCurrent = isCurrent;
			SetColor();
		}

		public void MarkAsHighlighted(bool isHighlighted)
		{
			this.isHighlighted = isHighlighted;
			SetColor();
		}

		private void SetColor()
		{
			if (highlightRectangle == null)
				return;

			if (isCurrent && isHighlighted)
			{
				highlightRectangle.Fill = currentBrush;
				highlightRectangle.Stroke = currentBrush;
				highlightRectangle.Opacity = 0.3;
			}
			else if (isCurrent && !isHighlighted)
			{
				highlightRectangle.Fill = currentBrush;
				highlightRectangle.Stroke = currentBrush;
				highlightRectangle.Opacity = 0.2;

			}
			else if (!isCurrent && isHighlighted)
			{
				highlightRectangle.Fill = highlightBrush;
				highlightRectangle.Stroke = highlightBrush;
				highlightRectangle.Opacity = 0.1;
			}
			else if (!isCurrent && !isHighlighted)
			{
				highlightRectangle.Fill = Brushes.Transparent;
				highlightRectangle.Stroke = Brushes.Transparent;
			}
		}
	}
}
