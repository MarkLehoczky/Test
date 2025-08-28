using Microsoft.UI.Xaml.Data;
using System;
using System.IO;

namespace VidHub.WinUI.Converters
{
    internal partial class MissingPreviewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is string file && File.Exists(file) ? file : "../Assets/NoImage.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
