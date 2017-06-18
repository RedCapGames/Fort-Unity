using Fort.Inspector;

namespace Fort.Info.GameLevel
{
#if UNITY_EDITOR
    [Inspector(Presentation = "Fort.CustomEditor.FortScenePresentation")]
#endif
    public class FortScene
    {
        public string SceneName { get; set; }

        public static bool IsNullOrEmpty(FortScene fortScene)
        {
            return fortScene == null || string.IsNullOrEmpty(fortScene.SceneName);
        }
    }
}
