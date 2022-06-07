using System;

namespace ONQHL7.MessageGenerator_V1
{
	public static class HL7Encoding
	{
		public static char FieldDelimiter { get; } = '|'; // \F\
		public static char ComponentDelimiter { get; } = '^'; // \S\
		public static char RepeatDelimiter { get; } = '~';  // \R\
		public static char EscapeCharacter { get; } = '\\'; // \E\
		public static char SubComponentDelimiter { get; } = '&'; // \T\
		public static string SegmentDelimiter { get; } = "\r";
		public static string PresentButNull { get; } = "\"\"";
		public static string AllDelimiters => "" + FieldDelimiter + ComponentDelimiter + RepeatDelimiter + EscapeCharacter + SubComponentDelimiter;
	}
	/// <summary>
	/// Formate the datetime to yyyymmddhhmm i.e 202010151830
	/// </summary>
	public static class Hl7Satandard
	{
		public static string FormatDate(string a_sDate)
		{
			string sDate = "";
			try
			{
				var date = Convert.ToDateTime(a_sDate)
                    .ToLocalTime();
				int len;
				string sYear = date.Year.ToString();
				if (!string.IsNullOrEmpty(sYear) && sYear.Length == 4)
					sDate = $"{sDate}{sYear}";
				else
					return "";

				string sMonth = date.Month.ToString();
				if (!string.IsNullOrEmpty(sMonth))
				{
					len = sMonth.Length;
					if (len == 1)
						sDate = $"{sDate}0{sMonth}";
					else
						sDate = $"{sDate}{sMonth}";
				}
				else
					return "";

				string sDay = date.Day.ToString();
				if (!string.IsNullOrEmpty(sDay))
				{
					len = sDay.Length;
					if (len == 1)
						sDate = $"{sDate}0{sDay}";
					else
						sDate = $"{sDate}{sDay}";
				}
				else
					return "";

				string sHour = date.Hour.ToString();
				if (!string.IsNullOrEmpty(sHour))
				{
					if (sHour == "0") {
						sDate = $"{sDate}00";
					}
					else
					{
						len = sHour.Length;
						if (len == 1)
							sDate = $"{sDate}0{sHour}";
						else
							sDate = $"{sDate}{sHour}";
					}
				}
				else
					return "";

				string sMin = date.Minute.ToString();
				if (!string.IsNullOrEmpty(sMin))
				{
					if (sMin == "0")
					{
						sDate = $"{sDate}00";
					}
					else
					{
						len = sMin.Length;
						if (len == 1)
							sDate = $"{sDate}0{sMin}";
						else
							sDate = $"{sDate}{sMin}";
					}
				}
				else
					return "";

				string sSec = date.Second.ToString();
				if (!string.IsNullOrEmpty(sSec))
				{
					if (sSec == "0")
					{
						sDate = $"{sDate}00";
					}
					else
					{
						len = sSec.Length;
						if (len == 1)
							sDate = $"{sDate}0{sSec}";
						else
							sDate = $"{sDate}{sSec}";
					}
				}
				else
					return "";
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return sDate;
		}
	}
}
