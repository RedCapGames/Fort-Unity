using System;

namespace Fort
{
    /// <summary>
    /// Fort global settings
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Resolving server Settings
        /// </summary>
        /// <returns>Promise of server settings</returns>
        ComplitionPromise<ServerSettings> ResolveServerSettings();
        /// <summary>
        /// Resolving server chached settings
        /// </summary>
        /// <returns>Server settings</returns>
        ServerSettings ResolveCachedServerSetting();
        /// <summary>
        /// Get deployed version can be used in android and ios and is changable in Player settings of unity
        /// </summary>
        /// <returns>Version of deployment</returns>
        Version GetVersion();

    }
}
