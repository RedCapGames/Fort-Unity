using Fort.Info;

namespace FortTapligh.Info
{
    [Info(typeof(TaplighInfoScriptableObject), "FortTapligh", false)]
    public class TaplighInfo:IInfo
    {
        public string Key { get; set; }
    }
}
