namespace Fort.Info.Language
{

    [Info(typeof(LanguageScriptableObject),"Fort",true)]
    public class LanguageEditorInfo:IInfo
    {
        public LanguageEditorInfo()
        {
            Languages = new[]
            {
                new LanguageInfo
                {
                    Name = "Eng"

                }
            };
        }
        public LanguageInfo[] Languages { get; set; }
    }
}
