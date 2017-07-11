using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Fort
{

    class InfoGenerator : EditorWindow
    {
        private string _infoName;
        private bool _editorOnly;
        [MenuItem("Fort/Custom/Info Generator")] 
        public static void ShowWindow()
        {
            GetWindow(typeof(InfoGenerator));
            
        }

        void OnGUI()
        {
            _infoName = EditorGUILayout.TextField("Info Name", _infoName);
            if (string.IsNullOrEmpty(_infoName))
            {
                GUI.enabled = false;
            }
            _editorOnly = EditorGUILayout.Toggle("Editor only", _editorOnly);
            if (GUILayout.Button("Generate Info"))
            {
                GenerateInfo(_infoName,_editorOnly);    
            }
            if (string.IsNullOrEmpty(_infoName))
            {
                GUI.enabled = true;
            }

        }

        private void GenerateInfo(string infoName,bool editorOnly)
        {
            if (!infoName.EndsWith("Info"))
                infoName += "Info";
            Debug.Log(PlayerSettings.productName);
            StringBuilder scriptableObjectBuilder = new StringBuilder();
            scriptableObjectBuilder.AppendLine(@"using Fort.Inspector;");
            scriptableObjectBuilder.AppendLine();
            scriptableObjectBuilder.AppendLine(@"namespace Fort.Info");
            scriptableObjectBuilder.AppendLine(@"{");
            scriptableObjectBuilder.Append("\t");
            scriptableObjectBuilder.AppendLine(string.Format("public class {0}ScriptableObject : FortScriptableObject<{0}>",infoName));
            scriptableObjectBuilder.Append("\t");
            scriptableObjectBuilder.AppendLine("{");
            scriptableObjectBuilder.Append("\t");
            scriptableObjectBuilder.AppendLine("}");
            scriptableObjectBuilder.AppendLine("}");

            StringBuilder editorBuilder = new StringBuilder();
            editorBuilder.AppendLine(@"using Fort.Info;");
            editorBuilder.AppendLine(@"using Fort.Inspector;");
            editorBuilder.AppendLine(@"using UnityEditor;");
            editorBuilder.AppendLine();
            editorBuilder.AppendLine(@"namespace Fort.Info");
            editorBuilder.AppendLine(@"{");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine(@"[CanEditMultipleObjects]");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine(string.Format("[UnityEditor.CustomEditor(typeof({0}ScriptableObject), true)]", infoName));
            editorBuilder.Append("\t");
            editorBuilder.AppendLine(string.Format("public class {0}Editor : FortInspector", infoName));
            editorBuilder.Append("\t");
            editorBuilder.AppendLine("{");
            editorBuilder.Append("\t");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine(string.Format("[MenuItem(\"Fort/Settings/Custom/Show {0}\")]",infoName));
            editorBuilder.Append("\t");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine("public static void ShowSetting()");
            editorBuilder.Append("\t");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine("{");
            editorBuilder.Append("\t");
            editorBuilder.Append("\t");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine(string.Format("EditorInfoResolver.ShowInfo<{0}>();", infoName));
            editorBuilder.Append("\t");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine("}");
            editorBuilder.Append("\t");
            editorBuilder.AppendLine("}");
            editorBuilder.AppendLine("}");

            StringBuilder infoBuilder = new StringBuilder();
            infoBuilder.AppendLine(@"using Fort.Info;");
            infoBuilder.AppendLine();
            infoBuilder.AppendLine(@"namespace Fort.Info");
            infoBuilder.AppendLine(@"{");
            infoBuilder.Append("\t");
            infoBuilder.AppendLine(string.Format("[Info(typeof({0}ScriptableObject), \"{1}\", {2})]", infoName,
                PlayerSettings.productName, editorOnly?"true":"false"));
            infoBuilder.Append("\t");
            infoBuilder.AppendLine(string.Format("public class {0}:IInfo", infoName));
            infoBuilder.Append("\t");
            infoBuilder.AppendLine("{");
            infoBuilder.Append("\t");
            infoBuilder.AppendLine("}");
            infoBuilder.AppendLine("}");

            AssetDatabaseHelper.CreateFolderRecursive(string.Format("Assets/{0}",infoName));
            AssetDatabaseHelper.CreateFolderRecursive(string.Format("Assets/{0}/Editor", infoName));

            string scriptableCsPath = Path.Combine(Application.dataPath,string.Format("{0}/{0}ScriptableObject.cs", infoName));
            string scriptableAssetPath = string.Format("Assets/{0}/{0}ScriptableObject.cs", infoName);
            try
            {
                File.Delete(scriptableCsPath);
            }
            catch (Exception)
            {
                // ignored
            }
            File.WriteAllText(scriptableCsPath, scriptableObjectBuilder.ToString(), Encoding.UTF8);
            //AssetDatabase.ImportAsset(scriptableAssetPath);
            

            string editorCsPath = Path.Combine(Application.dataPath, string.Format("{0}/Editor/{0}Editor.cs", infoName));
            string editorAssetPath = string.Format("Assets/{0}/Editor/{0}Editor.cs", infoName);
            try
            {
                File.Delete(editorCsPath);
            }
            catch (Exception)
            {
                // ignored
            }
            File.WriteAllText(editorCsPath, editorBuilder.ToString(), Encoding.UTF8);
            //AssetDatabase.ImportAsset(editorAssetPath);


            string infoCsPath = Path.Combine(Application.dataPath, string.Format("{0}/{0}.cs", infoName));
            string infoAssetPath = string.Format("Assets/{0}/{0}.cs", infoName);
            try
            {
                File.Delete(infoCsPath);
            }
            catch (Exception)
            {
                // ignored
            }
            File.WriteAllText(infoCsPath, infoBuilder.ToString(), Encoding.UTF8);
            //AssetDatabase.ImportAsset(infoAssetPath);

            AssetDatabase.Refresh();

        }
    }
}
