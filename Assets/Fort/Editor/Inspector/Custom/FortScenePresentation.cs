using Fort.Info.GameLevel;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort.CustomEditor
{
    public class FortScenePresentation: Presentation
    {
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {            
            FortScene fortScene = (FortScene)parameter.Instance;
            SceneAsset oldScene = fortScene == null?null: AssetDatabase.LoadAssetAtPath<SceneAsset>(fortScene.SceneName);
            if(fortScene == null)
                fortScene = new FortScene();
            EditorGUI.BeginChangeCheck();
            Object result = EditorGUILayout.ObjectField(parameter.Title, oldScene, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(result);
                fortScene.SceneName = newPath;
                return new PresentationResult
                {
                    Result = fortScene,
                    Change = new Change { IsDataChanged = true }
                };
            }
            return new PresentationResult
            {
                Result = fortScene,
                Change = new Change { IsDataChanged = false }
            };
        }

        #endregion
    }
}
