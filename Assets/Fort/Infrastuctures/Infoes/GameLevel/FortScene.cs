using Fort.Inspector;

namespace Fort.Info.GameLevel
{

    [Inspector(Presentation = "Fort.CustomEditor.FortScenePresentation")]

    public class FortScene
    {
        public string SceneName { get; set; }

        public static bool IsNullOrEmpty(FortScene fortScene)
        {
            return fortScene == null || string.IsNullOrEmpty(fortScene.SceneName);
        }
    }
}
