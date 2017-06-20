using System;
using System.Collections.Generic;
using Fort.Inspector;

namespace Fort.Info.Language
{
    public class LanguageInfo
    {
        public LanguageInfo()
        {
            Id = Guid.NewGuid().ToString();
            LanguageDatas = new Dictionary<string, object>();
        }
        [IgnorePresentation]
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Rtl { get; set; }
        [IgnorePresentation]
        public Dictionary<string,object> LanguageDatas { get; set; } 
    }
}
