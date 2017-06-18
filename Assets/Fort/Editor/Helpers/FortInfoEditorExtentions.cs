using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Assets.Fort.Editor.Helpers
{
    public static class FortInfoEditorExtentions
    {
        public static void Save(this FortInfo fortInfo)
        {
            FortInfoScriptable fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
            fortInfoScriptable.Save(fortInfoScriptable);
            EditorUtility.SetDirty(fortInfoScriptable);

        }
    }
}
