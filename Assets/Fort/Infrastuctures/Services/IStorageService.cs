using System;

namespace Fort
{
    public interface IStorageService
    {
        bool ContainsData(Type dataType);
        object ResolveData(Type dataType);
        void UpdateData(object data, Type dataType);
        Promise UpdateDataLatent(object data, Type dataType);
        void UpdateOnMemory(object data, Type dataType);
        void SaveOnMemoryData(Type dataType);
        Promise SaveOnMemoryDataLatent(Type dataType);
        Promise UpdateTokenDataLatent(object data, object token, Type dataType);
        void UpdateTokenData(object data, object token, Type dataType);
        object ResolveTokenData(Type dataType, object token);
    }
}
