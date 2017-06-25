using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Info;

namespace FortBacktory.Info
{
    [Info(typeof(BacktoryInfoScriptableObject),"FortBacktory",false)]
    public class BacktoryInfo:IInfo
    {
        public string AuthenticationId { get; set; }
        public string AuthenticationClientKey { get; set; }
        public string CloudId { get; set; }
    }
}
