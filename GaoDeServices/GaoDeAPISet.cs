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
        private const string IPFileName = "GaoDeIP";

        internal static IP IP;
        internal static string ApiKey = "None";

        /// <summary>
        /// 存储IP对象的文件路径
        /// </summary>
        public static string IPFile
        {
            get => Path.Combine(APISet.ApiMetaFolder, IPFileName);
        }

        /// <summary>
        /// 激活高德地图提供的WebService
        /// </summary>
        /// <param name="Key">从高德地图控制台申请的Key</param>
        /// <param name="IsUpdateIP">反应是否更新一次IP</param>
        public static async void Awake(string? Key = default, bool IsUpdateIP = true)
        {
            var filePath = Path.Combine(APISet.ApiMetaFolder, IPFile + ".xml");
            if (string.IsNullOrEmpty(Key))
            {
                throw new Exception("ApiKey Is Null Or Empty");
            }
            if (!IsUpdateIP && !File.Exists(filePath))
            {
                throw new Exception("No historical IP query result exists, specify IsUpdateIP as true");
            }

            IP LastIP = new IP();
            if (File.Exists(filePath))
            {
                LastIP = File.ReadAllText(filePath).XmlParse<IP>();
            }

            ApiKey = Key;
            if (IsUpdateIP)
            {
                IP = await IPService.GetIP();
                IP.HistoricalAdress = LastIP.HistoricalAdress;
                IP.HistoricalAdcode = LastIP.HistoricalAdcode;
            }
            else
            {
                if (LastIP.City == null || LastIP.Adcode == null)
                {
                    var newIP = await IPService.GetIP();
                    newIP.HistoricalAdress = LastIP.HistoricalAdress;
                    newIP.HistoricalAdcode = LastIP.HistoricalAdcode;
                    IP = newIP;
                    Save();
                    return;
                }
                IP = LastIP;
            }
            Save();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void Save()
        {
            IPFile.CreatXmlFile(APISet.ApiMetaFolder, IP);
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
