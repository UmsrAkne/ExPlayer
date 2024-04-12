using System;
using System.Globalization;
using System.Windows.Data;

namespace ExPlayer.Models
{
    public class TicksToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not long ticks)
            {
                return string.Empty;
            }

            var timeSpan = TimeSpan.FromTicks(ticks);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}