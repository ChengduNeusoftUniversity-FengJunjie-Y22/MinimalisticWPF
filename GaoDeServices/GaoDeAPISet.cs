using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MinimalisticWPF.GaoDeServices
{
    public static class GaoDeAPISet
    {
        private const string GaoDeDatas = "GaoDeServiceMeta";
        private const string IPFile = "GaoDeIP.xml";
        private const string KeyFile = "GaoDeKey.txt";

        public static IP LocalIP;
        /// <summary>
        /// 从.exe所在层级,读取"GaoDeServiceMeta/GaoDeIP.xml"文件,该文件是高德地图WebApi最近一次提供的IP信息
        /// </summary>
        public static async void ReadIP()
        {
            try
            {
                var filepath = Path.Combine(GaoDeDatas.CreatFolder(), IPFile);
                if (File.Exists(filepath))
                {
                    LocalIP = File.ReadAllText(filepath).XmlParse<IP>();
                }
            }
            finally
            {
                if (string.IsNullOrEmpty(LocalIP.City))
                {
                    await UpdateIP();
                }
            }
        }
        /// <summary>
        /// 手动刷新IP
        /// </summary>
        public static async Task UpdateIP()
        {
            LocalIP = await IPService.GetIP();
            IPFile.CreatXmlFile(GaoDeDatas.CreatFolder(), LocalIP);
        }

        private static string ApiKey = "None";
        /// <summary>
        /// 从.exe所在层级,读取"GaoDeServiceMeta/GaoDeKey.txt"文件,该文件是高德地图WebApi的Key
        /// </summary>
        public static bool ReadKey()
        {
            bool result;

            try
            {
                ApiKey = File.ReadAllText(Path.Combine(GaoDeDatas.CreatFolder(), KeyFile));
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private const string WeatherApiBaseUrl = $"https://restapi.amap.com/v3/weather/weatherInfo?";
        internal static string GetWeatherInfo(string OutPut, string City, string Extensions)
        {
            string result = string.Empty;

            result = WeatherApiBaseUrl + $"output={OutPut}&key={ApiKey}&city={City}&extensions={Extensions}";

            return result;
        }

        private const string AdCodeBaseUrl = "https://restapi.amap.com/v3/config/district?";
        internal static string GetAdCodeInfo(string OutPut, string City)
        {
            string result = string.Empty;

            result = AdCodeBaseUrl + $"keywords={City}&subdistrict=2&key={ApiKey}&output={OutPut}";

            return result;
        }

        private const string IPApiBaseUrl = "https://restapi.amap.com/v3/ip?";
        internal static string GetIPInfo(string OutPut)
        {
            string result = string.Empty;

            result = IPApiBaseUrl + $"output={OutPut}&key={ApiKey}";

            return result;
        }
    }
}
