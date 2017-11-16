using System;
using UnityEngine;

public class Dated
{
    private static int StartYear = 2000;
    private static int Seasons = 4;
    private static int Months = 12;
    private static int Days = 360;
    private static int Hours = 24;
    private static int Minutes = 60;
    private static int Seconds = 60;
    
    public static int Minute = Seconds;
    /// <summary>
    /// Seconds in an hour
    /// </summary>
    public static int Hour = Minute * Minutes;
    public static int Day = Hour * Hours;
    /// <summary>
    /// Seconds in year.
    /// </summary>
    public static int Year = Day * Days; // Seconds in a year.
    public static int Month = Year / Months;
    /// <summary>
    /// Seconds in a season.
    /// </summary>
    public static int Season = Year / Seasons;

    public double time { get; private set; }
    public double deltaTime { get; private set; }
    public int year {
        get
        {
            return yr + StartYear;
        } }
    public int season { get; private set; }
    public int month { get; private set; }
    public int day { get; private set; }
    public int hour { get; private set; }
    public int minute { get; private set; }
    public double seconds { get; private set; }

    public int yr { get; private set; }
    
    public Dated() { }
    public Dated(int year, int month, int day, int hour, int minute, double seconds)
    {
        time = 0;
        time += (year - StartYear) * Year;
        time += (month - 1) * Month;
        time += (day - 1) * Day;
        time += hour * Hour;
        time += minute * Minute;
        time += seconds;

        UpdateDate(time);
        
    }

    public Dated(double _time)
    {
        UpdateDate(_time);
    }

    public void AddTime(double _time)
    {
        deltaTime = _time;
        time += _time;

        seconds += _time;
        if (seconds > Seconds)
        {
            minute += Mathd.FloorToInt(seconds / Seconds);
            seconds = seconds % Seconds;

            if (minute > Minutes) //Setting the Add Time parts
            {
                hour += Mathd.FloorToInt(minute / Minutes);
                minute = minute % Minutes;

                if (hour > Hours)
                {
                    day += Mathd.FloorToInt(hour / Hours);
                    hour = hour % Hours;

                    season = Mathd.FloorToInt(day / (Days / Seasons));
                    month = Mathd.FloorToInt(day / (Days / Months));

                    if (day > Days)
                    {
                        yr += Mathd.FloorToInt(day / Days);
                        day = day % Days;

                    }
                }
            }
        }




    }

    internal void UpdateDate(double _time)
    {
        time = _time;
        deltaTime = 0;

        yr = Mathd.FloorToInt(_time / Year);
        _time = _time % Year;
        season = Mathd.FloorToInt(_time / Season);
        month = Mathd.FloorToInt(_time / Month);
        day = Mathd.FloorToInt(_time / Day);
        _time = _time % Day;
        hour = Mathd.FloorToInt(_time / Hour);
        _time = _time % Hour;
        minute = Mathd.FloorToInt(_time / Minute);
        _time = _time % Minute;
        seconds = _time;
    }

    public string GetDate()
    {
        return year + " Year(s), " + (day + 1) + " Day(s)";
    }
    
    public string GetFormatedDate()
    {
        int daysInMonth = Days / Months;

        return String.Format("{0}/{1}/{2}", month + 1, (day % daysInMonth) + 1, year);
    }

    public string GetFormatedDateTime()
    {
        return String.Format("{0} - {1}:{2}:{3} {4}",
            GetSeason(),
            hour.ToString().PadLeft(2, '0'),
            minute.ToString().PadLeft(2, '0'),
            seconds.ToString("0").PadLeft(2, '0'),
            GetFormatedDate());
    }
    public string GetDateTime()
    {
        string dateTime = String.Format("{0} - {1}:{2}:{3}\nDay {4}, year {5}",
            GetSeason(),
            hour.ToString().PadLeft(2, '0'),
            minute.ToString().PadLeft(2, '0'),
            seconds.ToString("0").PadLeft(2, '0'),
            day + 1,
            year);
        return dateTime;
    }
    //public string GetDate(double _time)
    //{
    //    double year = Mathd.FloorToInt(_time / Year);
    //    _time = _time % Year;
    //    double season = Mathd.FloorToInt(_time / Season);
    //    double day = Mathd.FloorToInt(_time / Day);
    //    _time = _time % Day;
    //    double hour = Mathd.FloorToInt(_time / Hour);
    //    _time = _time % Hour;

    //    return year.ToString() + "/" + season.ToString() + "/" + day.ToString() + " " + hour.ToString() + " h";
    //}
    /// <summary>
    /// Returns a human readable version of duration of time
    /// </summary>
    /// <param name="time">Given in seconds</param>
    /// <returns></returns>
    public static string ReadTime(double time)
    {
        if (time < 90)
        {
            return time + " s";
        }
        else if (time < Dated.Hour)
        {
            return (time / Dated.Minute).ToString("0.0") + " minutes";
        }

        else if (time < Dated.Day)
        {
            return (time / Dated.Hour).ToString("0.00") + " hours";
        }
        else if (time < Dated.Year)
        {
            return (time / Dated.Day).ToString("0.0") + " days";
        }
        else
        {
            return (time / Dated.Year).ToString("0.00") + " years";
        }
    }
    private string GetSeason()
    {
        string[] seasonNames = new string[4] { "Spring", "Summer", "Fall", "Winter" };
        return ""; // seasonNames[season];
    }

    public static Dated operator -(Dated date1, Dated date2)
    {
        double diff = date1.time - date2.time;

        Dated finalDate = new Dated(diff);

        return finalDate;

    }

    public static bool operator >(Dated date1, Dated date2)
    {
        return (date1.time > date2.time) ? true : false;
    }

    public static bool operator <(Dated date1, Dated date2)
    {
        return (date1.time < date2.time) ? true : false;
    }

    public static bool operator >=(Dated date1, Dated date2)
    {
        return (date1.time >= date2.time) ? true : false;
    }
    public static bool operator <=(Dated date1, Dated date2)
    {
        return (date1.time <= date2.time) ? true : false;
    }
}