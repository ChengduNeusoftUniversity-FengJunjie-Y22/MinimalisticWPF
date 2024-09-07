using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.GaoDeServices
{
    public struct Weather
    {
        public string Date { get; set; }
        public string Week { get; set; }
        public string DayTemp { get; set; }
        public string NightTemp { get; set; }
        public string DayWeather { get; set; }
        public string NightWeather { get; set; }
        public string DayWind { get; set; }
        public string NightWind { get; set; }
        public string DayPower { get; set; }
        public string NightPower { get; set; }

        public string GetCombined()
        {
            string result = string.Empty;

            result += $"日期:{Date}\n";
            result += $"星期:{Week}\n\n";
            result += $"白天:\n气温:{DayTemp}℃\n";
            result += $"天气:{DayWeather}\n";
            result += $"风向:{DayWind}\n";
            result += $"风力:{DayPower}\n\n";
            result += $"夜间:\n气温:{NightTemp}℃\n";
            result += $"天气:{NightWeather}\n";
            result += $"风向:{NightWind}\n";
            result += $"风力:{NightPower}\n";

            return result;
        }

        internal static List<Weather> Create(string xml)
        {
            List<string> dates = xml.CaptureBetween("<date>", "</date>");
            List<string> weeks = xml.CaptureBetween("<week>", "</week>");
            List<string> dayWeathers = xml.CaptureBetween("<dayweather>", "</dayweather>");
            List<string> nightWeathers = xml.CaptureBetween("<nightweather>", "</nightweather>");
            List<string> dayTemps = xml.CaptureBetween("<daytemp>", "</daytemp>");
            List<string> nightTemps = xml.CaptureBetween("<nighttemp>", "</nighttemp>");
            List<string> dayWinds = xml.CaptureBetween("<daywind>", "</daywind>");
            List<string> nightWinds = xml.CaptureBetween("<nightwind>", "</nightwind>");
            List<string> dayPowers = xml.CaptureBetween("<daypower>", "</daypower>");
            List<string> nightPowers = xml.CaptureBetween("<nightpower>", "</nightpower>");

            int count = dates.Count;
            List<Weather> weatherForecasts = new List<Weather>();
            for (int i = 0; i < count; i++)
            {
                Weather weather = new Weather
                {
                    Date = dates[i],
                    Week = weeks[i],
                    DayWeather = dayWeathers[i],
                    NightWeather = nightWeathers[i],
                    DayTemp = dayTemps[i],
                    NightTemp = nightTemps[i],
                    DayWind = dayWinds[i],
                    NightWind = nightWinds[i],
                    DayPower = dayPowers[i],
                    NightPower = nightPowers[i]
                };
                weatherForecasts.Add(weather);
            }

            return weatherForecasts;
        }
    }
}
