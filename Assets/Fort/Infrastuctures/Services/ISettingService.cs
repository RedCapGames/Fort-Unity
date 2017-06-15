using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fort
{
    public interface ISettingService
    {
        ComplitionPromise<ServerSettings> ResolveServerSettings();
        
        ServerSettings ResolveCachedServerSetting();
        
    }
    public enum AudioCategory
    {
        GameSound,
        Music
    }
}
