using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                score.canvas.Children.Remove(uiElement);

            uiElements.Clear();
        }

        public virtual bool IsShown
        {
            get { return uiElements.Count > 0; }
        }

        protected void AddElement(Score score, UIElement uiElement)
        {
            score.canvas.Children.Add(uiElement);
            uiElements.Add(uiElement);
        }

        protected double ShowFetaGlyph(Score score, double glyphLeft, double glyphTop, string glyphCode)
        {
            const string familyName = "feta26";
            double fontSize = 42 * score.Magnification;

            UIElement uiElement = new Label
            {
                FontFamily = new FontFamily(familyName),
                FontSize = fontSize,
                Content = glyphCode,
                Padding = new Thickness(0, 0, 0, 0),
                //Background = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(glyphLeft, glyphTop, 0, 0)
            };
            AddElement(score, uiElement);

            FormattedText formattedText = new FormattedText(
                glyphCode,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(familyName),
                fontSize,
                Brushes.Black);

            return glyphLeft + formattedText.Width;
        }
    }
}
