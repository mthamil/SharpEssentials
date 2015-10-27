using System;
using System.Globalization;
using System.Windows.Data;

namespace SharpEssentials.Controls.Converters
{
    /// <summary>
    /// An <see cref="IValueConverter"/> that produces an object's runtime type.
    /// </summary>
    public class ObjectGetTypeConverter : IValueConverter
    {
        /// <see cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType() ?? typeof(object);
        }

        /// <see cref="IValueConverter.ConvertBack"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}