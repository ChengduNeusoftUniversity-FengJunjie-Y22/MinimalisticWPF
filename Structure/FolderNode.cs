using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class FolderNode
    {
        public FolderNode() { }
        public FolderNode(string name) { Name = name; }
        public FolderNode(string name, FolderNode folderNode) { Name = name; Father = folderNode; }

        public string Name { get; set; } = string.Empty;

        public FolderNode? Father { get; set; } = null;

        public string Path
        {
            get
            {
                var pathParts = new Stack<string>();
                var currentNode = this;

                while (currentNode != null)
                {
                    pathParts.Push(currentNode.Name);
                    currentNode = currentNode.Father;
                }

                var result = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.Combine(pathParts.ToArray()));
                return result;
            }
        }
    }
}
