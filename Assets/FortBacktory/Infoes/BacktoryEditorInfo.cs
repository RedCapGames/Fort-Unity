using Fort.Info;
using Fort.Inspector;

namespace FortBacktory.Info
{
    [Info(typeof(BacktoryEditorInfoScriptableObject),"FortBacktory",true)]
    public class BacktoryEditorInfo:IInfo
    {
        [PresentationTitle("X-Backtory-Authentication-Key (Master)")]
        public string AuthenticationMasterKey { get; set; }
    }
}
