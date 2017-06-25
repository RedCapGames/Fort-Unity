using Fort.Inspector;
using UnityEditor;

namespace Fort.Info
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(FortInfoScriptable), true)]

    public class FortInfoEditor : FortInspector
    {
    }
}
