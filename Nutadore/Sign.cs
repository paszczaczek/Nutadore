using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    abstract public class Sign
    {
        public Position staffLine;

        abstract public double Paint(Canvas canvas, double left, double top, double maginification);

        public double Paint(Canvas canvas, string text, double left, double top, double maginification)
        {
            const string familyName = "feta26";
            double fontSize = 42 * maginification;

            Label sign = new Label
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
            canvas.Children.Add(sign);

            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(familyName),
                fontSize,
                Brushes.Black);

            return left + formattedText.Width;
        }
    }
}
