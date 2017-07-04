using System;

namespace Fort
{
    /// <summary>
    /// Service that is used to save classes
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Is any data saved for this type
        /// </summary>
        /// <param name="dataType">Type of class</param>
        /// <returns>Is any data saved for this type</returns>
        bool ContainsData(Type dataType);
        /// <summary>
        /// Resolving saved data.(null will return if no data saved for this class type)
        /// </summary>
        /// <param name="dataType">Type of class</param>
        /// <returns>Returning saved data.(null will return if no data saved for this class type)</returns>
        object ResolveData(Type dataType);
        /// <summary>
        /// Updating data
        /// </summary>
        /// <param name="data">Data to update</param>
        /// <param name="dataType">Type of class</param>
        void UpdateData(object data, Type dataType);
        /// <summary>
        /// Updating data in another thread. Data will be cloned and a copy of it will be saved
        /// </summary>
        /// <param name="data">Data to update</param>
        /// <param name="dataType">Type of class</param>
        /// <returns>Promise of updating</returns>
        Promise UpdateDataLatent(object data, Type dataType);
        /// <summary>
        /// Update data only on cache of storage
        /// </summary>
        /// <param name="data">Data to update</param>
        /// <param name="dataType">Type of class</param>
        void UpdateOnMemory(object data, Type dataType);
        /// <summary>
        /// Save cached data to storage
        /// </summary>
        /// <param name="dataType">Type of class</param>
        void SaveOnMemoryData(Type dataType);
        /// <summary>
        /// Save cached data to storage in another thread. Data will be cloned and a copy of it will be saved
        /// </summary>
        /// <param name="dataType">Type of class</param>
        /// <returns>Promise of updating</returns>
        Promise SaveOnMemoryDataLatent(Type dataType);

        /// <summary>
        /// Save data with a token in another thread. Data will be cloned and a copy of it will be saved
        /// </summary>
        /// <param name="data">Data to update</param>
        /// <param name="token">Token for save</param>
        /// <param name="dataType">Type of class</param>
        /// <returns>Promise of updating</returns>
        Promise UpdateTokenDataLatent(object data, string token, Type dataType);
        /// <summary>
        /// Save data with a token
        /// </summary>
        /// <param name="data">Data to update</param>
        /// <param name="token">Token for save</param>
        /// <param name="dataType">Type of class</param>
        void UpdateTokenData(object data, string token, Type dataType);
        /// <summary>
        /// Resolving data that contain a token
        /// </summary>
        /// <param name="dataType">Type of class</param>
        /// <param name="token">Token for load</param>
        /// <returns>Returning saved data.(null will return if no data saved for this class type)</returns>
        object ResolveTokenData(Type dataType, string token);
    }
}
