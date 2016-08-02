using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Panteam.MetroChart
{
    class YearColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {   
            var valueString = value.ToString();
            switch (System.Convert.ToInt32(valueString.Split
                (new char[1] { '.' })[0]))
            {
                case 1:
                    {
                        return "#FF313176";                        
                    }
                case 2:
                    {
                        return "#FF315476";
                    }
                case 3:
                    {
                        return "#FF28C797";
                    }
                case 4:
                    {
                        return "#FF28C748";
                    }
                case 5:
                    {
                        return "#FF58C728";
                    }
                case 6:
                    {
                        return "#FFCD5418";
                    }
                case 7:
                    {
                        return "#FFC72D28";
                    }
                case 8:
                    {
                        return "#FFC74328";
                    }
                case 9:
                    {
                        return "#FFF77C3E";
                    }
                case 10:
                    {
                        return "#FFFDBF16";
                    }
                case 11:
                    {
                        return "#FFC28F19";
                    }
                case 12:
                    {
                        return "#FF19A0C2";
                    }
            }

            return "#FFFFFFFF";

            //var valueString = value.ToString();
            //if ((int)(valueString[valueString.Length-1]) % 2 == 0)
            //    return "#FFED7D31";
            //else return "#FFFFC000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }    
}
