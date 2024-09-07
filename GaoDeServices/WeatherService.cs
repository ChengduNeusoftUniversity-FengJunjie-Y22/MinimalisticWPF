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
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(GaoDeAPISet.GetWeatherInfo("xml", Adcode == null ? GaoDeAPISet.LocalIP.Adcode : Adcode, "all"));
                return response;
            }
        }

        /// <summary>
        /// 获取解析后的天气预报消息
        /// </summary>
        /// <param name="CityName">地区名称</param>
        /// <returns>List For Weather 今天/明天/后天/大后天</returns>
        public static async Task<List<Weather>> GetWeathers(string? CityName = default)
        {
            if (CityName == null)
            {
                GaoDeAPISet.ReadIP();
                var response = await GetXmlWeather(GaoDeAPISet.LocalIP.Adcode);
                return Weather.Create(response);
            }
            else
            {
                var adcode = await IPService.GetAdCode(CityName);
                var response = await GetXmlWeather(adcode);
                return Weather.Create(response);
            }
        }
    }
}
