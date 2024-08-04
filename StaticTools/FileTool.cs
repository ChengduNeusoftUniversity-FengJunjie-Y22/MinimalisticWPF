using System.IO;
using System.Text.Json;
using System.Windows;
using System.Xml.Serialization;

public enum SerializeModes
{
    Xml,
    Json
}

namespace MinimalisticWPF
{
    /// <summary>
    /// 【文件工具】提供系列静态的方法，用于快速完成IO操作
    /// </summary>
    public static class FileTool
    {
        /// <summary>
        /// 【序列化】，要求必须实现ISerializableObject接口
        /// </summary>
        /// <returns>元组，Item1表示是否成功序列化，Item2表示文本消息</returns>
        public static string? SerializeObject<T>(T target) where T : class, ISerializableObject
        {
            string? result = null;

            SerializeModes mode = target.SerializeMode;
            try
            {
                switch (mode)
                {
                    case SerializeModes.Xml:
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                        using (StringWriter writer = new StringWriter())
                        {
                            xmlSerializer.Serialize(writer, target);
                            result = writer.ToString();
                        }
                        break;

                    case SerializeModes.Json:
                        string jsonString = JsonSerializer.Serialize(target, typeof(T));
                        result = jsonString;
                        break;
                }

            }
            catch { }

            return result;
        }

        /// <summary>
        /// 【反序列化】，要求必须实现ISerializableObject接口
        /// </summary>
        /// <returns>元组，Item1表示是否成功反序列化，Item2表示返回的实际对象</returns>
        public static T? DeSerializeObject<T>(string serializedData, SerializeModes mode) where T : class, ISerializableObject
        {
            T? result = null;

            try
            {
                switch (mode)
                {
                    case SerializeModes.Xml:
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                        using (StringReader reader = new StringReader(serializedData))
                        {
                            result = (T)xmlSerializer.Deserialize(reader);
                        }
                        break;

                    case SerializeModes.Json:
                        result = JsonSerializer.Deserialize<T>(serializedData);
                        break;
                }
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 以【文件】形式存储序列化对象，具体位置取决于你为对象设置的绝对路径
        /// </summary>
        /// <returns>元组，Item1表示是否存储成功，Item2表示信息</returns>
        public static string? SaveObjectAsFile<T>(T target) where T : class, ISerializableObject
        {
            if (IsAbsolutePathValid(target.AbsolutePath))
            {
                var data = SerializeObject(target);
                if (data != null)
                {
                    try
                    {
                        File.WriteAllText(target.AbsolutePath, data);
                        return target.AbsolutePath;
                    }
                    catch { }
                }
            }

            return null;
        }

        /// <summary>
        /// 以【绝对路径】读取存储的序列化文件
        /// </summary>
        /// <returns>元组，Item1表示是否读取成功，Item2表示实际对象</returns>
        public static T? ReadObjectFromFile<T>(string filePath, SerializeModes mode) where T : class, ISerializableObject
        {
            T? result = null;

            if (IsAbsolutePathValid(filePath))
            {
                try
                {
                    string data = File.ReadAllText(filePath);
                    var temp = DeSerializeObject<T>(data, mode);
                    if (temp != null)
                    {
                        result = temp;
                    }
                    return result;
                }
                catch { }
            }

            return result;
        }

        /// <summary>
        /// 检查绝对路径是否合法
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsAbsolutePathValid(string path)
        {
            bool isAbsolutePath = Path.IsPathRooted(path);
            bool driveExists = true;
            if (isAbsolutePath)
            {
                string? drive = Path.GetPathRoot(path);
                if (drive == null) { return false; }
                driveExists = DriveInfo.GetDrives()
                                       .Any(d => d.Name.Equals(drive, StringComparison.OrdinalIgnoreCase));
            }
            return isAbsolutePath && driveExists;
        }

        /// <summary>
        /// 依据【文件名】【序列化类型】【程序位置】，动态生成文件的绝对路径
        /// </summary>
        /// <returns>string</returns>
        public static string GenerateFilePath(string? floderName, string fileName, SerializeModes mode)
        {
            string SuffixName = mode == SerializeModes.Xml ? ".xml" : ".json";
            string FloderName = floderName == null ? AppDomain.CurrentDomain.BaseDirectory : floderName;
            return Path.Combine(FloderName, fileName + SuffixName);
        }
    }
}