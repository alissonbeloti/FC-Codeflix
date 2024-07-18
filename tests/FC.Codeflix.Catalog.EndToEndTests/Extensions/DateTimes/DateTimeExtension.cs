using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTimes;
internal static class DateTimeExtension
{
    public static DateTime TrimMilliseconds(this DateTime dateTime)
    {
        return new DateTime(
            dateTime.Year, 
            dateTime.Month, 
            dateTime.Day, 
            dateTime.Hour, 
            dateTime.Minute, 
            dateTime.Second, 
            0,
            dateTime.Kind);
    }
}
