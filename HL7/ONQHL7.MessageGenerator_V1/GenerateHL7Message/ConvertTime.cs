using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
    public class ConvertTime
    {
        public static DateTimeOffset ConvertTimetoAEDT(DateTime date)
        {
            var countryTimeZone = "Australia/Sydney";
            if (!string.IsNullOrEmpty(countryTimeZone))
            {
                var zone = DateTimeZoneProviders.Tzdb[countryTimeZone];
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(date, DateTimeKind.Utc));
                var localDateTime = instant.InZone(zone).ToDateTimeOffset();
                return localDateTime;
            }
            else
            {
                return new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc)).ToOffset(TimeSpan.Parse(countryTimeZone.Replace("+", "")));
            }
        }
    }
}
