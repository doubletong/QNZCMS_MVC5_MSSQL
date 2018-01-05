using System;
using System.Collections.Generic;

namespace TZGCMS.Infrastructure.Helper
{
    public static class DateTimeHelper
    {
        public static List<int> GetYearList(int starYear)
        {
            List<int> yearList = new List<int>();
            int year = DateTime.Now.Year;
            for (int i = starYear; i <= year; i++)
            {
                yearList.Add(i);
            }
            return yearList;
        }
        public static List<int> GetMonthList()
        {
            List<int> monthList = new List<int>();           
            for (int i = 1; i <= 12; i++)
            {
                monthList.Add(i);
            }
            return monthList;
        }
        public static List<int> GetDayList()
        {
            List<int> dayList = new List<int>();
            int year = DateTime.Now.Year;
            for (int i = 1; i <= 31; i++)
            {
                dayList.Add(i);
            }
            return dayList;
        }
    }
}
