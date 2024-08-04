namespace MinimalisticWPF
{
    /// <summary>
    /// 可序列化对象
    /// </summary>
    public interface ISerializableObject
    {
        /// <summary>
        /// (反)序列化格式
        /// </summary>
        SerializeModes SerializeMode { get; }

        /// <summary>
        /// 绝对路径
        /// </summary>
        string FileName { get; }
    }
}
