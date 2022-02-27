namespace RecipeScheduler.Utilites
{
    public static class DateTimeUtils
    {
        private const string utcDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff'Z'";
        private const string alternateutcDateTimeFormat = "yyyy'-'dd'-'MM'T'HH':'mm':'ss'.'fffffff'Z'";

        public static DateTime ParseInputDate(string s)
        {
            DateTime result;
            if (!DateTime.TryParseExact(s, utcDateTimeFormat, null, DateTimeStyles.None, out result))
            {
                result = DateTime.ParseExact(s, alternateutcDateTimeFormat, CultureInfo.InvariantCulture);
            }
            return result;
        }

        public static string ToUTCString(DateTime dt)
        {
            return dt.ToString(utcDateTimeFormat);
        }
    }
}
