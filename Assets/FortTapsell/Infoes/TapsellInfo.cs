using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Info;

namespace FortTapsell.Info
{
    [Info(typeof(TapsellInfoScriptableObject),"FortTapsell",false)]
    public class TapsellInfo:IInfo
    {
        public string Key { get; set; }
        public static TapsellInfo Instance { get { return InfoResolver.Resolve<TapsellInfo>(); } }
    }
}
