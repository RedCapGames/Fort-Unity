namespace Fort.Info.Language
{
    public class LanguageEditorInfo
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
