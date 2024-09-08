using System.Net.Http;
using System.Reflection.Emit;

namespace MinimalisticWPF.GaoDeServices
{
    public static class WeatherService
    {
        /// <summary>
        /// 获取原始天气预报消息
        /// </summary>
        /// <param name="Adcode">行政地区编码</param>
        public static async Task<string> GetXmlWeather(string? Adcode = default)
        {
            var condition = string.IsNullOrEmpty(Adcode);

            if (condition || string.IsNullOrEmpty(GaoDeAPISet.IP.Adcode))
            {
                GaoDeAPISet.IP = await IPService.GetIP();
                GaoDeAPISet.IP.HistoricalAdcode = GaoDeAPISet.IP.Adcode;
            }
            if (!condition)
            {
                GaoDeAPISet.IP.HistoricalAdcode = Adcode;
            }
            GaoDeAPISet.Save();

            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(GaoDeAPISet.GetWeatherInfo("xml", condition ? GaoDeAPISet.IP.HistoricalAdcode ?? GaoDeAPISet.IP.Adcode : Adcode, "all"));
            }
        }

        /// <summary>
        /// 获取解析后的天气预报消息
        /// <para>若不填入地区名称,则自动依据当前IP获取天气</para>
        /// </summary>
        /// <param name="CityName">地区名称</param>
        /// <returns>List For Weather 今天/明天/后天/大后天</returns>
        public static async Task<List<Weather>> GetWeathers(string? CityName = default)
        {
            if (CityName == null)
            {
                GaoDeAPISet.IP.HistoricalAdress = GaoDeAPISet.IP.City;
                GaoDeAPISet.IP.HistoricalAdcode = GaoDeAPISet.IP.Adcode;
                var response = await GetXmlWeather(GaoDeAPISet.IP.Adcode);
                GaoDeAPISet.Save();
                return Weather.Create(response, GaoDeAPISet.IP.City);
            }
            else
            {
                if (CityName == GaoDeAPISet.IP.HistoricalAdress)
                {
                    var response = await GetXmlWeather(GaoDeAPISet.IP.HistoricalAdcode);
                    return Weather.Create(response, CityName);
                }
                else
                {
                    var adcode = await IPService.GetAdCode(CityName);
                    var response = await GetXmlWeather(adcode);
                    GaoDeAPISet.IP.HistoricalAdress = CityName;
                    GaoDeAPISet.IP.HistoricalAdcode = adcode;
                    GaoDeAPISet.Save();
                    return Weather.Create(response, CityName);
                }
            }
        }
    }
}
