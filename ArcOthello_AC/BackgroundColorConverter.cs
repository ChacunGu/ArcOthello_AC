using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ArcOthello_AC
{
    public class BackgroundColorConverter : IMultiValueConverter
    {
        public Color Player1Color { get; set; }
        public Color Player2Color { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double scoreP1 = System.Convert.ToDouble(values[0]);
            double scoreP2 = System.Convert.ToDouble(values[1]);
            double ratio = scoreP1 / (scoreP1 + scoreP2);
            if (double.IsNaN(ratio))
                return new SolidColorBrush(Colors.White);

            return new SolidColorBrush(Blend(Player1Color, Player2Color, ratio));
        }

        /// <summary>
        /// Blends the specified colors together.
        /// Taken from StackOverflow https://stackoverflow.com/questions/3722307/is-there-an-easy-way-to-blend-two-system-drawing-color-values
        /// </summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep,
        /// “on top of” <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        public Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromRgb(r, g, b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
