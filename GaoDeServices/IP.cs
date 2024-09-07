using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.GaoDeServices
{
    public struct IP
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string Adcode { get; set; }
        public Rectangle Coordinate { get; set; }

        public string GetCombined()
        {
            string result = string.Empty;

            result += $"省:{Province}\n\n";
            result += $"市:{City}\n\n";
            result += $"地区编码:{Adcode}\n\n";
            result += $"坐标:[{Coordinate.X1},{Coordinate.Y1}][{Coordinate.X2},{Coordinate.Y2}]";

            return result;
        }

        public static List<IP> Create(string xml)
        {
            List<string> provinces = xml.CaptureBetween("<province>", "</province>");
            List<string> cities = xml.CaptureBetween("<city>", "</city>");
            List<string> adcodes = xml.CaptureBetween("<adcode>", "</adcode>");
            List<string> rectangles = xml.CaptureBetween("<rectangle>", "</rectangle>");

            int count = provinces.Count;
            List<IP> result = new List<IP>();
            for (int i = 0; i < count; i++)
            {
                IP p = new IP()
                {
                    Province = provinces[i],
                    City = cities[i],
                    Adcode = adcodes[i],
                    Coordinate = new Rectangle(rectangles[i])
                };
                result.Add(p);
            }

            return result;
        }

        public struct Rectangle
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }

            public Rectangle(string rectangleString)
            {
                var parts = rectangleString.Split(';')[0].Split(',');
                X1 = double.Parse(parts[0]);
                Y1 = double.Parse(parts[1]);
                var parts2 = rectangleString.Split(';')[1].Split(',');
                X2 = double.Parse(parts2[0]);
                Y2 = double.Parse(parts2[1]);
            }
        }
    }
}
