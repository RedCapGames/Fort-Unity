using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public static class CustomAssetUtility
{
    #region  Public Methods

    public static void CreateAsset<T>() where T : ScriptableObject
    {
#if UNITY_EDITOR
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
#endif
    }

    public static void UpdateAsset<T>(T asset) where T : ScriptableObject
    {
#if UNITY_EDITOR
        EditorUtility.CopySerialized(asset, asset);
#endif
    }

    #endregion
}