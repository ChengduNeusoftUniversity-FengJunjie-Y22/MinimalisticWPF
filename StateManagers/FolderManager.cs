using System.IO;

namespace MinimalisticWPF
{
    /// <summary>
    /// 多层级文件夹管理
    /// </summary>
    public class FolderManager : ISerializableObject
    {
        private FolderManager() { }

        public SerializeModes SerializeMode { get; set; } = SerializeModes.Xml;
        public string AbsolutePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FolderInfo.xml");

        public List<string> Paths { get; internal set; } = new List<string>();

        public FolderManager Parse(ICollection<FolderNode> floderNodes)
        {
            FolderManager result = new FolderManager();

            foreach (FolderNode node in floderNodes)
            {
                string temp = node.Path;
                if (!Paths.Contains(temp)) { Paths.Add(temp); }
            }

            return result;
        }

        public void Creat()
        {
            foreach (string path in Paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }
}
