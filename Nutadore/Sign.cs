using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nutadore
{
    abstract public class Sign
    {
        public StaffPosition staffPosition;
        public Staff.Type staffType;

        protected string code;
        protected Brush brush = Brushes.Black;
        protected UIElement uiElement;
        protected Score score;

        virtual public double Show(Score score, double left, double top)
        {
            this.score = score;

            const string familyName = "feta26";
            double fontSize = 42 * score.Magnification;

            uiElement = new Label
            {
                FontFamily = new FontFamily(familyName),
                FontSize = fontSize,
                Content = code,
                Foreground = brush,
                Padding = new Thickness(0, 0, 0, 0),
                //Background = new SolidColorBrush(Colors.LightYellow),
                Margin = new Thickness(
                    left,
                    top,
                    0,
                    0)
            };
            score.canvas.Children.Add(uiElement);

            FormattedText formattedText = new FormattedText(
                code,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(familyName),
                fontSize,
                Brushes.Black);

            return left + formattedText.Width;
        }

        public void Clear()
        {
            if (score != null && uiElement != null)
            {
                score.canvas.Children.Remove(uiElement);
                uiElement = null;
            }
        }

        public bool Shown
        {
            get { return uiElement != null; }
        }
    }
}
