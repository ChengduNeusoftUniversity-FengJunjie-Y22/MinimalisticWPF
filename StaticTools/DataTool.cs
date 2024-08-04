using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// 【数据工具】提供系列静态方法，用于处理常见的数据分析、转换、捕获……
    /// </summary>
    public static class DataTool
    {
        /// <summary>
        /// 将 string 转为 int
        /// </summary>
        /// <param name="target">待被转换的 string</param>
        /// <returns>若转换失败，则返回 null</returns>
        public static int? StringToInt(string target)
        {
            int? result = null;
            bool success = int.TryParse(target, out int temp);
            if (success)
            {
                result = temp;
            }
            return result;
        }

        /// <summary>
        /// 将 string 转为 bool
        /// </summary>
        /// <param name="target">待被转换的 string</param>
        /// <returns>若转换失败，则返回 null</returns>
        public static bool? StringToBool(string target)
        {
            if (target.Length < 4 || target.Length > 5)
            {
                return null;
            }
            if ((target[0] == 'T' || target[0] == 't') &&
                (target[1] == 'R' || target[1] == 'r') &&
                (target[2] == 'U' || target[2] == 'u') &&
                (target[3] == 'E' || target[3] == 'e'))
            {
                return true;
            }
            if ((target[0] == 'F' || target[0] == 'f') &&
                (target[1] == 'A' || target[1] == 'a') &&
                (target[2] == 'L' || target[2] == 'l') &&
                (target[3] == 'S' || target[3] == 's') &&
                (target[4] == 'E' || target[4] == 'e'))
            {
                return false;
            }
            return null;
        }

        /// <summary>
        /// 将 # string 转为指定类型的颜色
        /// </summary>
        public static T? StringToColor<T>(string colorString) where T : class
        {
            if (!colorString.StartsWith("#") || colorString.Length != 7) { return null; }
            if (typeof(T) == typeof(Brush))
            {
                Color color = (Color)ColorConverter.ConvertFromString(colorString);
                return (T)(object)new SolidColorBrush(color);
            }
            else if (typeof(T) == typeof(SolidColorBrush))
            {
                Color color = (Color)ColorConverter.ConvertFromString(colorString);
                return (T)(object)new SolidColorBrush(color);
            }
            return null;
        }

        /// <summary>
        /// 保留小数点
        /// </summary>
        public static double KeepDecimal(double value, int decimalPlaces)
        {
            double multiplier = Math.Pow(10, decimalPlaces);

            double result = Math.Truncate(value * multiplier) / multiplier;

            return result;
        }

        /// <summary>
        /// 对字符串采用AES加密
        /// </summary>
        /// <param name="plaintext">原始字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>string 加密字符串</returns>
        public static string AES(string plaintext, string key)
        {
            byte[] encryptedBytes;
            byte[] IV;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plaintext);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(IV) + "|" + Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// 对AES加密字符串解密
        /// </summary>
        /// <param name="ciphertext">加密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>string 解密字符串</returns>
        public static string AESParse(string ciphertext, string key)
        {
            string[] parts = ciphertext.Split('|');
            byte[] IV = Convert.FromBase64String(parts[0]);
            byte[] cipherBytes = Convert.FromBase64String(parts[1]);
            string plaintext = string.Empty;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// 捕获所有位于 《 》 内的字符串
        /// </summary>
        /// <param name="target">待被分解的字符串</param>
        /// <returns>捕获集合 List</returns>
        public static List<string> CaptureByQuotationMarks(string target)
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\《(.*?)\》");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }

        /// <summary>
        /// 捕获所有位于 尖括号 内的字符串
        /// </summary>
        /// <param name="target">待被分解的字符串</param>
        /// <returns>捕获集合 List</returns>
        public static List<string> CaptureByAngleBrackets(string target)
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\<(.*?)\>");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }

        /// <summary>
        /// 捕获所有位于 { } 内的字符串
        /// </summary>
        /// <param name="target">待被分解的字符串</param>
        /// <returns>捕获集合 List</returns>
        public static List<string> CaptureByCurlyBraces(string target)
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\{(.*?)\}");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }

        /// <summary>
        /// 捕获所有位于 [ ] 内的字符串
        /// </summary>
        /// <param name="target">待被分解的字符串</param>
        /// <returns>捕获集合 List</returns>
        public static List<string> CaptureBySquareBrackets(string target)
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\[(.*?)\]");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }

        /// <summary>
        /// 捕获所有位于 ( ) 内的字符串
        /// </summary>
        /// <param name="target">待被分解的字符串</param>
        /// <returns>捕获集合 List</returns>
        public static List<string> CaptureByParenthesis(string target)
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\((.*?)\)");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }
    }
}
