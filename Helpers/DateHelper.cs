using System;

namespace OXStack.Helpers
{
    /// <summary>
    /// Sundry date functions, ancient stuff
    /// </summary>
    public class DateHelper
    {
        /// <summary>
        /// Creates a timestamp, ie: 2011-08-12 becomes: 20110812
        /// </summary>
        /// <param name="dt">Date to encode</param>
        /// <returns></returns>
        public static int ToTimeStamp(DateTime dt)
        {
            int iRet = (dt.Year * 10000);
            iRet += (dt.Month * 100);
            iRet += dt.Day;
            return iRet;
        }

        /// <summary>
        /// Returns number of days in a year, including leap years..
        /// </summary>
        /// <param name="dt">date of wich year we want the number of days</param>
        /// <returns></returns>
        public static int NumDaysInYear(DateTime dt)
        {
            if (DateTime.IsLeapYear(dt.Year))
                return 366;
            return 365;
        }

        /// <summary>
        /// Number of days left in the year, including leap years..
        /// </summary>
        /// <param name="dt">date in wich year we're going to check</param>
        /// <returns></returns>
        public static int NumDaysLeftInYear(DateTime dt)
        {
            DateTime dtEnd = new DateTime(dt.Year, 12, 31);
            TimeSpan ts = dtEnd - dt;
            return Convert.ToInt32(ts.TotalDays);
        }

        /// <summary>
        /// Fetches month in integer by short month string (eg. 'jan', 'sep', etc)
        /// </summary>
        /// <param name="sMonth"></param>
        /// <returns></returns>
        public static int MonthFromShortMonthString(string sMonth)
        {
            int iRet = 1;

            for (int i = 0; i < 12; i++)
            {
                DateTime dt = new DateTime(2011, i + 1, 1);
                if (sMonth.ToLower() == dt.ToString("MMM").ToLower())
                    return i + 1;
            }
            return iRet;
        }

        /// <summary>
        /// Converts string of dutch date (day-month-year) to DateTime
        /// </summary>
        /// <param name="sDutchDate"></param>
        /// <returns></returns>
        public static DateTime FromDutchDateString(string sDutchDate)
        {
            string[] saElems = sDutchDate.Split('-');
            DateTime dtRet = new DateTime(1970, 1, 1);
            if (saElems.Length == 3)
            {
                int iDay = Parser.Parse<int>(saElems[0], 1);
                int iMonth = Parser.Parse<int>(saElems[1], 1);
                int iYear = Parser.Parse<int>(saElems[2], 1970);

                dtRet = new DateTime(iYear, iMonth, iDay);
            }
            return dtRet;
        }

        /// <summary>
        /// Converts string of dutch datetime (day-month-year hour-minute-second) to DateTime
        /// </summary>
        /// <param name="sDutchDate"></param>
        /// <returns></returns>
        public static DateTime FromDutchDateTimeString(string sDutchDate)
        {
            string[] saElems = sDutchDate.Split(new char[] { '-', ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime dtRet = new DateTime(1970, 1, 1, 0, 0, 0);
            if (saElems.Length > 2)
            {
                int iDay = Parser.Parse<int>(saElems[0], 1);
                int iMonth = Parser.Parse<int>(saElems[1], 1);
                int iYear = Parser.Parse<int>(saElems[2], 1970);

                int iHour = 0;
                int iMinute = 0;
                int iSecond = 0;

                if (saElems.Length > 3) iHour = Parser.Parse<int>(saElems[3], 0);
                if (saElems.Length > 4) iMinute = Parser.Parse<int>(saElems[4], 0);
                if (saElems.Length > 5) iSecond = Parser.Parse<int>(saElems[5], 0);

                dtRet = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
            }
            return dtRet;
        }
    }
}
