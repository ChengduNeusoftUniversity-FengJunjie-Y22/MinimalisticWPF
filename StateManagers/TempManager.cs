
namespace MinimalisticWPF
{
    /// <summary>
    /// 【临时信息管理器】提供系列方法以管理程序的临时数据，例如设置信息
    /// </summary>
    public class TempManager : ISerializableObject
    {
        internal static TempManager Manager = new TempManager();

        internal TempManager() { }

        public SerializeModes SerializeMode { get; } = SerializeModes.Xml;
        public string AbsolutePath { get; } = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.xml");

        /// <summary>
        /// 【核心数据】假定你使用某个实现ISerializableObject的类型作为临时数据的存储结构，那么你可以将此类型的实例对象放入这里
        /// </summary>
        public List<ISerializableObject> Data { get; set; } = new List<ISerializableObject>();

        /// <summary>
        /// 创建新的管理器
        /// </summary>
        public static void CreateNewManager()
        {
            Manager = new TempManager();
        }

        /// <summary>
        /// 存储临时信息
        /// </summary>
        public void Save()
        {
            FileTool.SaveObjectAsFile(this);
        }

        /// <summary>
        /// 读取临时信息
        /// </summary>
        public void Load()
        {
            FileTool.ReadObjectFromFile<TempManager>(AbsolutePath, SerializeMode);
        }
    }
}
