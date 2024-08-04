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
    /// 【文件工具】提供系列静态的方法，用于快速完成文件读写
    /// </summary>
    public static class SerializeTool
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
        /// 将对象存放在指定的路径（ 已知绝对路径时 ）
        /// </summary>
        public static bool SaveObjectAsFile<T>(T target, string AbsolutePath) where T : class, ISerializableObject
        {
            if (IsAbsolutePathValid(AbsolutePath))
            {
                var data = SerializeObject(target);
                if (data != null)
                {
                    try
                    {
                        File.WriteAllText(AbsolutePath, data);
                        return true;
                    }
                    catch { }
                }
            }

            return false;
        }

        /// <summary>
        /// 将对象存放在指定的路径（ 需要动态获取绝对路径时 ）
        /// </summary>
        public static bool SaveObjectAsFile<T>(T target, FolderNode folder) where T : class, ISerializableObject
        {
            string path = GetPath(target, folder);
            if (IsAbsolutePathValid(path))
            {
                var data = SerializeObject(target);
                if (data != null)
                {
                    try
                    {
                        File.WriteAllText(path, data);
                        return true;
                    }
                    catch { }
                }
            }

            return false;
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
        /// 生成路径
        /// </summary>
        /// <returns>string</returns>
        public static string GetPath(ISerializableObject target, FolderNode folder)
        {
            string SuffixName = target.SerializeMode == SerializeModes.Xml ? ".xml" : ".json";
            string FloderName = folder.Path;
            return Path.Combine(FloderName, target.FileName + SuffixName);
        }
    }
}