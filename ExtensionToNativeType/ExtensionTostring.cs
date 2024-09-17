using System.Windows.Media;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;

namespace MinimalisticWPF
{
    public static class ExtensionTostring
    {
        /// <summary>
        /// 简易检查密码强度
        /// </summary>
        public static int CheckPasswordStrength(this string password, int MinLength = 8)
        {
            if (string.IsNullOrEmpty(password))
            {
                return 0;
            }

            int length = password.Length;
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            bool hasDigit = Regex.IsMatch(password, @"\d");
            bool hasSpecialChar = Regex.IsMatch(password, @"[\W_]");

            if (length < MinLength)
            {
                return 0;
            }

            int score = 0;
            if (hasUpperCase) score++;
            if (hasLowerCase) score++;
            if (hasDigit) score++;
            if (hasSpecialChar) score++;

            return score;
        }

        /// <summary>
        /// Levenshtein距离(长文本匹配)
        /// </summary>
        public static int LevenshteinDistance(this string source, string target)
        {
            int n = source.Length;
            int m = target.Length;
            var d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        /// <summary>
        /// Jaro-Winkler距离(短文本匹配)
        /// </summary>
        public static double JaroWinklerDistance(this string source, string target)
        {
            if (source == target) return 1.0;

            int s1Len = source.Length;
            int s2Len = target.Length;

            if (s1Len == 0 || s2Len == 0) return 0.0;

            int matchDistance = Math.Max(s1Len, s2Len) / 2 - 1;
            bool[] s1Matches = new bool[s1Len];
            bool[] s2Matches = new bool[s2Len];

            int matches = 0;
            for (int i = 0; i < s1Len; i++)
            {
                int start = Math.Max(0, i - matchDistance);
                int end = Math.Min(i + matchDistance + 1, s2Len);
                for (int j = start; j < end; j++)
                {
                    if (s2Matches[j]) continue;
                    if (source[i] != target[j]) continue;
                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0) return 0.0;

            int t = 0;
            int k = 0;
            for (int i = 0; i < s1Len; i++)
            {
                if (!s1Matches[i]) continue;
                while (!s2Matches[k]) k++;
                if (source[i] != target[k]) t++;
                k++;
            }
            t /= 2;

            double jt = (matches / (double)s1Len + matches / (double)s2Len + (matches - t) / (double)matches) / 3.0;
            double jw = jt + (0.1 * Math.Min(s1Len, s2Len) * (1 - jt));
            return jw;
        }

        /// <summary>
        /// 最佳匹配
        /// </summary>
        /// <param name="target">匹配源</param>
        /// <param name="similarity">Levenshtein距离(int) OR JaroWinkler距离(double)</param>
        /// <returns>int? 匹配结果在集合中的索引</returns>
        public static int? BestMatch(this string source, ICollection<string> target, object similarity)
        {
            int? result = null;

            if (similarity is int LD)
            {
                int Mini = LD;
                List<string> data = new List<string>(target);
                for (int i = 0; i < data.Count; i++)
                {
                    int sim = source.LevenshteinDistance(data[i]);
                    if (sim <= Mini && sim > 0)
                    {
                        Mini = sim;
                        result = i;
                    }
                    else if (sim == 0)
                    {
                        result = i;
                        break;
                    }
                }
            }
            if (similarity is double JD)
            {
                double Max = JD;
                List<string> data = new List<string>(target);
                for (int i = 0; i < data.Count; i++)
                {
                    double sim = source.JaroWinklerDistance(data[i]);
                    if (sim >= Max && sim < 1)
                    {
                        Max = sim;
                        result = i;
                    }
                    else if (sim == 1)
                    {
                        result = i;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 转为颜色
        /// </summary>
        public static Brush ToBrush(this string source)
        {
            if (!source.StartsWith("#") || source.Length != 7) { return Brushes.Transparent; }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(source));
        }

        /// <summary>
        /// 转为int
        /// </summary>
        public static int? ToInt(this string source)
        {
            int? result = null;
            bool success = int.TryParse(source, out int temp);
            if (success)
            {
                result = temp;
            }
            return result;
        }

        /// <summary>
        /// 转为double
        /// </summary>
        public static double ToDouble(this string source)
        {
            double? result = null;
            bool success = double.TryParse(source, out double temp);
            if (success)
            {
                result = temp;
            }
            return result == null ? double.NaN : (double)result;
        }

        /// <summary>
        /// 转为float
        /// </summary>
        public static float? ToFloat(this string source)
        {
            float? result = null;
            bool success = float.TryParse(source, out float temp);
            if (success)
            {
                result = temp;
            }
            return result;
        }

        /// <summary>
        /// 转为bool
        /// </summary>
        public static bool? ToBool(this string source)
        {
            if (source.Length < 4 || source.Length > 5)
            {
                return null;
            }
            if ((source[0] == 'T' || source[0] == 't') &&
                (source[1] == 'R' || source[1] == 'r') &&
                (source[2] == 'U' || source[2] == 'u') &&
                (source[3] == 'E' || source[3] == 'e'))
            {
                return true;
            }
            if ((source[0] == 'F' || source[0] == 'f') &&
                (source[1] == 'A' || source[1] == 'a') &&
                (source[2] == 'L' || source[2] == 'l') &&
                (source[3] == 'S' || source[3] == 's') &&
                (source[4] == 'E' || source[4] == 'e'))
            {
                return false;
            }
            return null;
        }

        /// <summary>
        /// 对字符串采用AES加密
        /// </summary>
        /// <param name="plaintext">原始字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>string 加密字符串</returns>
        public static string AES(this string source, string key)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException("The key size is not compliant. Use a 16, 24, or 32 key size");
            }

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
                            swEncrypt.Write(source);
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
        public static string AESParse(this string source, string key)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException("The key size is not compliant. Use a 16, 24, or 32 key size");
            }

            string[] parts = source.Split('|');
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
        /// 依据左右标识符捕获文本段
        /// </summary>
        /// <param name="left">左标识</param>
        /// <param name="right">右标识</param>
        /// <returns>List</returns>
        public static List<string> CaptureBetween(this string source, string left, string right)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Target string cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
            {
                throw new ArgumentException("The length of the capture identifier cannot be zero.");
            }

            List<string> result = new List<string>();

            string escapedLeft = Regex.Escape(left);
            string escapedRight = Regex.Escape(right);

            string pattern = @$"{escapedLeft}(.*?){escapedRight}";
            Regex regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(source);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    result.Add(match.Groups[1].Value);
                }
            }

            return result;
        }

        /// <summary>
        /// 依据有序特征集合，捕获文本段
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="features">特征字符串集合</param>
        /// <returns>捕获的文本段列表</returns>
        public static List<string> CaptureLike(this string source, params string[] features)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Source string cannot be null or empty.");
            }

            if (features == null || features.Length < 2)
            {
                throw new ArgumentException("At least two features are required.");
            }

            List<string> result = new List<string>();

            // 构造正则表达式
            string pattern = BuildPattern(features);
            Regex regex = new Regex(pattern);

            // 匹配源字符串
            MatchCollection matches = regex.Matches(source);

            foreach (Match match in matches)
            {
                // 将每个匹配的结果添加到结果列表中
                result.Add(match.Value);
            }

            return result;
        }

        /// <summary>
        /// 从.exe所在位置开始,创建一个文件夹
        /// </summary>
        /// <returns>成功创建文件夹的路径</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CreatFolder(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Cannot create a folder with an empty name");
            }

            string result = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, source);

            if (!IsPathValid(result))
            {
                throw new ArgumentException("The generated folder is not formatted correctly");
            }

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }

        /// <summary>
        /// 从.exe所在位置开始,创建多级嵌套关系的文件夹
        /// </summary>
        /// <param name="nodes">有序父节点</param>
        /// <returns>成功创建文件夹的路径</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CreatFolder(this string source, params string[] nodes)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Cannot create a folder with an empty name");
            }

            string result = string.Empty;
            for (int i = 0; i < nodes.Length; i++)
            {
                result = Path.Combine(i == 0 ? AppDomain.CurrentDomain.BaseDirectory : result, nodes[i]);
            }
            result = Path.Combine(result, source);

            if (!IsPathValid(result))
            {
                throw new ArgumentException("The generated folder is not formatted correctly");
            }

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }

        /// <summary>
        /// 作为文件名,存储指定对象为XML文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="targetObject">待存储对象</param>
        /// <returns>存储路径</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static string CreatXmlFile<T>(this string source, string folderPath, T targetObject)
        {
            if (!IsPathValid(folderPath) || !Directory.Exists(folderPath))
            {
                throw new ArgumentException($"Unavailable folder path {folderPath}");
            }

            string fileName = source + ".xml";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, targetObject);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating XML file '{filePath}': {ex.Message}");
            }
        }

        /// <summary>
        /// 作为文件名,存储指定对象为JSON文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="targetObject">待存储对象</param>
        /// <returns>存储路径</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static string CreatJsonFile<T>(this string source, string folderPath, T targetObject)
        {
            if (!IsPathValid(folderPath) || !Directory.Exists(folderPath))
            {
                throw new ArgumentException("Unavailable folder path");
            }

            string fileName = source + ".json";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(targetObject, options);

                File.WriteAllText(filePath, jsonString);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating JSON file '{filePath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Xml数据解析为指定类型的对象
        /// </summary>
        /// <param name="source">XML文件路径</param>
        /// <returns>T? 解析结果</returns>
        /// <exception cref="ArgumentException">无效的文件路径</exception>
        /// <exception cref="Exception">解析错误</exception>
        public static T? XmlParse<T>(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Empty Content !");
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(source))
                {
                    return (T?)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing XML file '{source}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Json数据解析为指定类型的对象
        /// </summary>
        /// <returns>T? 解析结果</returns>
        public static T? JsonParse<T>(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Invalid file path");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing JSON file '{source}': {ex.Message}");
            }
        }

        /// <summary>
        /// 检查绝对路径是否合法
        /// </summary>
        /// <returns>bool 检查结果</returns>
        public static bool IsPathValid(this string source)
        {
            bool isAbsolutePath = Path.IsPathRooted(source);
            bool driveExists = true;
            if (isAbsolutePath)
            {
                string? drive = Path.GetPathRoot(source);
                if (drive == null) { return false; }
                driveExists = DriveInfo.GetDrives().Any(d => d.Name.Equals(drive, StringComparison.OrdinalIgnoreCase));
            }
            return isAbsolutePath && driveExists;
        }

        internal static string BuildPattern(string[] features)
        {
            List<string> patternParts = new List<string>();

            for (int i = 0; i < features.Length - 1; i++)
            {
                patternParts.Add(Regex.Escape(features[i]) + "(.*?)" + Regex.Escape(features[i + 1]));
            }

            // 使用捕获组来获取完整的匹配部分
            return string.Join("|", patternParts);
        }
    }
}
