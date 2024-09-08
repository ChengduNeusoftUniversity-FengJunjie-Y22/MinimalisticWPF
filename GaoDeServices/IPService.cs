using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.GaoDeServices
{
    public static class IPService
    {
        /// <summary>
        /// 获取原始IP消息
        /// </summary>
        public static async Task<string> GetXmlIP()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(GaoDeAPISet.GetIPInfo("xml"));
                return response;
            }
        }

        /// <summary>
        /// 获取解析后的IP信息
        /// </summary>
        public static async Task<IP> GetIP()
        {
            var ipxml = await GetXmlIP();
            var ips = IP.Create(ipxml);
            IP result = ips.Count > 0 ? ips[0] : default;
            return result;
        }

        /// <summary>
        /// 查询地区的行政编码
        /// </summary>
        /// <param name="CityName">地区名称</param>
        public static async Task<string> GetAdCode(string CityName)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(GaoDeAPISet.GetAdCodeInfo("xml", CityName));
                var result = response.CaptureBetween("<adcode>", "</adcode>").FirstOrDefault() ?? string.Empty;
                return result;
            }
        }
    }
}
