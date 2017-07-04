using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Fort
{
    public class CustomMenuItem
    {
        [MenuItem("Fort/Custom/Purge Persistent Data")]
        static public void PurgePersistentData()
        {
            foreach (string file in Directory.GetFiles(Application.persistentDataPath))
            {
                File.Delete(file);
            }
        }
    }
}
