
namespace MinimalisticWPF
{
    /// <summary>
    /// 快速构建多层级的文件夹
    /// </summary>
    public class FloderManager : ISerializableObject
    {
        public FloderManager() { }
        public SerializeModes SerializeMode { get; set; } = SerializeModes.Xml;
        public string AbsolutePath { get; set; } = "Default";
    }
}
