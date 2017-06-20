using System.IO;
using UnityEngine;

using UnityEditor;


public static class CustomAssetUtility
{
    #region  Public Methods

    public static void CreateAsset<T>() where T : ScriptableObject
    {

        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof (T) + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

    }

    public static void UpdateAsset<T>(T asset) where T : ScriptableObject
    {
        EditorUtility.CopySerialized(asset, asset);
    }

    #endregion
}