using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    public class Sign
    {
        public void Paint(Canvas canvas, string signCode, double left, double top, double maginification)
        {
            Label glyph = new Label
            {
                FontFamily = new FontFamily("feta26"),
                FontSize = 42 * maginification,
                Content = signCode,
                Padding = new Thickness(0, 0, 0, 0),
                //Background = new SolidColorBrush(Colors.LightYellow),
                Margin = new Thickness(
                    left,
                    top,
                    0,
                    0)
            };
            canvas.Children.Add(glyph);
        }
    }
}
