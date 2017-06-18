using System.Linq;
using Fort.Info;
using Fort.Info.GameLevel;
using UnityEditor;
using UnityEngine;

namespace Fort.Inspector
{
    class SceneNamePostProccess: AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(movedAssets[i]);
                if (sceneAsset != null)
                {
                    FortScene[] fortScenes =
                        TypeFinder.FindType(InfoResolver.FortInfo, typeof (FortScene)).Cast<FortScene>().ToArray();
                    foreach (FortScene fortScene in fortScenes.Where(scene => scene != null))
                    {
                        if (fortScene.SceneName == movedFromAssetPaths[i])
                        {
                            fortScene.SceneName = movedAssets[i];
                        }
                    }
                    FortInfoScriptable fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
                    fortInfoScriptable.Save(InfoResolver.FortInfo);
                    EditorUtility.SetDirty(fortInfoScriptable);
                }
            }
        }
    }
}
