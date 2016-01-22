using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    abstract public class Sign
    {
        public StaffPosition staffPosition;

        protected Score score;
        protected UIElement signLabel;

        abstract public double Show(double left, double top);

        public double Show(string text, double left, double top)
        {
            const string familyName = "feta26";
            double fontSize = 42 * score.Magnification;

            signLabel = new Label
            {
                FontFamily = new FontFamily(familyName),
                FontSize = fontSize,
                Content = text,
                Padding = new Thickness(0, 0, 0, 0),
                //Background = new SolidColorBrush(Colors.LightYellow),
                Margin = new Thickness(
                    left,
                    top,
                    0,
                    0)
            };
            score.canvas.Children.Add(signLabel);

            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(familyName),
                fontSize,
                Brushes.Black);

            return left + formattedText.Width;
        }

        protected void Hide()
        {
            score.canvas.Children.Remove(signLabel);
        }
    }
}
