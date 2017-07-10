using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Info;

namespace FortTapligh.Info
{
    [Info(typeof(TaplighInfoScriptableObject), "FortTapligh", false)]
    public class TaplighInfo:IInfo
    {
        public string Key { get; set; }
    }
}
