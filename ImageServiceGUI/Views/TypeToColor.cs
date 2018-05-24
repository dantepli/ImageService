using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ImageService.Infrastructure.Enums;

namespace ImageServiceGUI.Views
{
    public class TypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((MessageTypeEnum) value == MessageTypeEnum.INFO)
            {
                return new SolidColorBrush(Colors.Green);
            } else if((MessageTypeEnum) value == MessageTypeEnum.WARNING)
            {
                return new SolidColorBrush(Colors.Yellow);
            } else
            {
                return new SolidColorBrush(Colors.Red);
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
