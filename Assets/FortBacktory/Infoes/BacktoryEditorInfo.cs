using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Info;

namespace FortBacktory.Info
{
    [Info(typeof(BacktoryEditorInfoScriptableObject),"FortBacktory",true)]
    public class BacktoryEditorInfo:IInfo
    {
        public string AuthenticationMasterKey { get; set; }
    }
}
