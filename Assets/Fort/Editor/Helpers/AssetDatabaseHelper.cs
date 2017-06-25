using System.IO;
using System.Linq;
using UnityEditor;

namespace Fort
{
    public static class AssetDatabaseHelper
    {
        public static void CreateFolderRecursive(string folder)
        {
            string[] folderItems = folder.Split('/');
            string path = string.Empty;
            string oldPath = path;
            foreach (string source in folderItems.Where(s => !string.IsNullOrEmpty(s)))
            {
                path += source;
                if (!Directory.Exists(path))
                    AssetDatabase.CreateFolder(oldPath, source);
                oldPath = path;
                path += "/";
            }
        }
    }
}
