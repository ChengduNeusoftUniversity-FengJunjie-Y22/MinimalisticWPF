using System.IO;

namespace MinimalisticWPF
{
    /// <summary>
    /// 为分级文件夹的管理提供便利
    /// </summary>
    public static class FolderManager
    {
        public static void Creat(params FolderNode[] floderNodes)
        {
            foreach (FolderNode path in floderNodes)
            {
                string target = path.Path;
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
            }
        }

        public static void Delete(params FolderNode[] floderNodes)
        {
            foreach (FolderNode path in floderNodes)
            {
                string target = path.Path;
                if (Directory.Exists(target))
                {
                    Directory.Delete(target, true);
                }
            }
        }
    }
}
