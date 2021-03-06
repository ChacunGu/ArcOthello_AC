﻿using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ArcOthello_AC
{
    /// <summary>
    /// Converter used to convert the team value (an enum) to an image
    /// </summary>
    public class TeamToImageConverter : IValueConverter
    {
        public BitmapImage WhiteImage { get; set; }
        public BitmapImage BlackImage { get; set; }
        public BitmapImage WhiteImagePreview { get; set; }
        public BitmapImage BlackImagePreview { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // If the converter is used incorrectly, and the value is not a Team
            if (!(value is Team))
            {
                return null;
            }

            Team t = (Team) value;
            // Return the image accordingly
            // The tag is used to know if the image is a valid piece and not a preview
            switch(t)
            {
                case Team.Black:
                    return new Image
                    {
                        Source = BlackImage,
                        Tag = "valid"
                    };
                case Team.White:
                    return new Image
                    {
                        Source = WhiteImage,
                        Tag = "valid"
                    };
                case Team.BlackPreview:
                    return new Image
                    {
                        Source = BlackImagePreview,
                        Tag = null
                    };
                case Team.WhitePreview:
                    return new Image
                    {
                        Source = WhiteImagePreview,
                        Tag = null
                    };
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
