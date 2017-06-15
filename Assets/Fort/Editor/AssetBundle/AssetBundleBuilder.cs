using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Fort.AssetBundle
{
    public static class AssetBundleBuilder
    {
        public static void Build()
        {
            string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformName());
            //string[] splitedPath = outputPath.Split('\\');
            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            //string currentDirectory = Directory.GetCurrentDirectory();
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);


            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
