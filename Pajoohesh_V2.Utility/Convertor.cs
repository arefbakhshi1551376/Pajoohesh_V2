using System;
using System.Globalization;

namespace Pajoohesh_V2.Utility
{
    public static class Convertor
    {
        public static string ToPersianDate(this DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            TimeSpan timeSpan = dateTime.ToLocalTime() - dateTime;
            DateTime thisTime = dateTime.AddMinutes(timeSpan.TotalMinutes);

            var year = persianCalendar.GetYear(thisTime);
            var month = persianCalendar.GetMonth(thisTime);
            var day = persianCalendar.GetDayOfMonth(thisTime);
            var hour = persianCalendar.GetHour(thisTime);
            var minute = persianCalendar.GetMinute(thisTime);

            string finalDate = $"{hour.ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')} , {day.ToString().PadLeft(2, '0')} / {month.ToString().PadLeft(2, '0')} / {year}";
            return finalDate;
        }

        public static string ToPersianDateForNaming(this DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            TimeSpan timeSpan = dateTime.ToLocalTime() - dateTime;
            DateTime thisTime = dateTime.AddMinutes(timeSpan.TotalMinutes);

            var year = persianCalendar.GetYear(thisTime);
            var month = persianCalendar.GetMonth(thisTime);
            var day = persianCalendar.GetDayOfMonth(thisTime);
            var hour = persianCalendar.GetHour(thisTime);
            var minute = persianCalendar.GetMinute(thisTime);

            string finalDate = $"{hour.ToString().PadLeft(2, '0')}{minute.ToString().PadLeft(2, '0')}{day.ToString().PadLeft(2, '0')}{month.ToString().PadLeft(2, '0')}{year}";
            return finalDate;
        }

        public static string CallPastTime(this DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            string totalTime = "";
            var dif = DateTime.UtcNow - dateTime;
            var seconds = (int)dif.TotalSeconds;
            if (seconds < 60)
            {
                totalTime = "چند لحظه قبل...";
            }
            else if (seconds > 60)
            {
                var minutes = (int)seconds / 60;
                if (minutes < 60)
                {
                    totalTime = $"{minutes} دقیقه قبل...";
                }
                else
                {
                    var hours = (int)seconds / 3600;
                    if (hours < 24)
                    {
                        totalTime = $"{hours} ساعت قبل";
                    }
                    else
                    {
                        var days = (int)seconds / 86400;
                        if (days < 30)
                        {
                            totalTime = $"{days} روز قبل...";
                        }
                        else
                        {
                            var months = (int)seconds / 2592000;
                            if (months<12)
                            {
                                totalTime = $"{months} ماه قبل...";
                            }
                            else
                            {
                                var years = (int)seconds / 31536000;
                                if (years==1)
                                {
                                    totalTime = $"{years} سال قبل...";
                                }
                                else
                                {
                                    totalTime = $"{years} years ago...";
                                }
                            }
                        }
                    }
                }
            }
            return totalTime;
        }
    }
}