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

		protected List<UIElement> elements = new List<UIElement>();
		private List<UIElement> highlightElements = new List<UIElement>();
		private Rectangle highlightRectangle;
		public Rect bounds { get; protected set; } = Rect.Empty;

		private bool isCurrent;
		private bool isHighlighted;

		public virtual double AddToScore(Score score, Staff trebleStaff, Staff bassStaff, double left)
		{
			return left;
		}

		public virtual void RemoveFromScore(Score score)
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

		public Rectangle AddHighlightRectangle(Score score, Staff trebleStaff, Staff bassStaff, int zindex)
		{
			/* TODO wywalic
			double top = trebleStaff.StaffPositionToY(StaffPosition.ByLegerAbove(6));
			double bottom = bassStaff.StaffPositionToY(StaffPosition.ByLegerBelow(4));
			highlightRectangle = new Rectangle
			{
				Width = bounds.Width,
				Height = bottom - top,
				Margin = new Thickness(bounds.Left, top, 0, 0),
				Fill = Brushes.Transparent,
				Stroke = Brushes.Transparent,
				Tag = score // potrzebne w event handlerze
			};
			highlightRectangle.MouseEnter += HighlightRectangle_MouseEnter;
			highlightRectangle.MouseLeave += HightlightRectangle_MouseLeave;
			highlightRectangle.MouseDown += HighlightRectangle_MouseDown;
			AddElement(score, highlightRectangle, zindex);
			return highlightRectangle;
			*/
			return null;
		}

		public virtual void HighlightRectangle_MouseEnter(object sender, MouseEventArgs e)
		{
			MarkAsHighlighted(true);
		}

		public virtual void HightlightRectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			MarkAsHighlighted(false);
		}

		private void HighlightRectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// Zaznacz znak jako bieżący.
			Score score = (sender as Rectangle).Tag as Score;
			//score.currentStep = this;
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

		public virtual void KeyDown(Key key)
		{
		}

		public virtual void KeyUp(Key key)
		{
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
