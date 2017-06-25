using Fort.Info;
using Fort.Inspector;

namespace FortBacktory.Info
{
    [Info(typeof(BacktoryInfoScriptableObject),"FortBacktory",false)]
    public class BacktoryInfo:IInfo
    {
        [PresentationTitle("X-Backtory-Authentication-Id")]
        public string AuthenticationId { get; set; }
        [PresentationTitle("X-Backtory-Authentication-Key (Client)")]
        public string AuthenticationClientKey { get; set; }
        [PresentationTitle("Cloud-Code-Id")]
        public string CloudId { get; set; }
    }
}
