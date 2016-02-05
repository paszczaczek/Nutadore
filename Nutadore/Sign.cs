using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    abstract public class Sign
    {
        //public StaffPosition staffPosition = StaffPosition.ByLine(1);
        //public Staff.Type staffType;

        protected UIElement uiElement;

        virtual public double Show(Score score, double left, double top)
        {
            return left;
        }

        virtual public void Hide(Score score)
        {
            if (score != null && uiElement != null)
            {
                score.canvas.Children.Remove(uiElement);
                uiElement = null;
            }
        }

        virtual public bool IsShown
        {
            get { return uiElement != null; }
        }

        protected double ShowFetaGlyph(Score score, double glyphLeft, double glyphTop, string glyphCode)
        {

            const string familyName = "feta26";
            double fontSize = 42 * score.Magnification;

            uiElement = new Label
            {
                FontFamily = new FontFamily(familyName),
                FontSize = fontSize,
                Content = glyphCode,
                Padding = new Thickness(0, 0, 0, 0),
                //Background = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(glyphLeft, glyphTop, 0, 0)
            };
            score.canvas.Children.Add(uiElement);

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
