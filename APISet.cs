using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MinimalisticWPF
{
    internal static class APISet
    {
        public static string FolderName { get; set; } = "ApiMeta";
        public static string ApiMetaFolder
        {
            get => FolderName.CreatFolder();
        }
    }
}
