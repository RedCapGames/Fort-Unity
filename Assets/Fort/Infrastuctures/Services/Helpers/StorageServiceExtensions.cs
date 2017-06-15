namespace Fort
{
    public static class StorageServiceExtensions
    {
        public static bool ContainsData<T>(this IStorageService storageService)
        {
            return storageService.ContainsData(typeof (T));
        }

        public static T ResolveData<T>(this IStorageService storageService)
        {
            return (T) storageService.ResolveData(typeof (T));
        }

        public static void UpdateData<T>(this IStorageService storageService, T data)
        {
            storageService.UpdateData(data,typeof(T));
        }

        public static Promise UpdateDataLatent<T>(this IStorageService storageService, T data)
        {
            return storageService.UpdateDataLatent(data, typeof (T));
        }

        public static void UpdateOnMemory<T>(this IStorageService storageService, T data)
        {
            storageService.UpdateOnMemory(data,typeof(T));
        }

        public static void SaveOnMemoryData<T>(this IStorageService storageService)
        {
            storageService.SaveOnMemoryData(typeof(T));
        }

        public static Promise SaveOnMemoryDataLatent<T>(this IStorageService storageService)
        {
            return storageService.SaveOnMemoryDataLatent(typeof (T));
        }

        public static Promise UpdateTokenDataLatent<T>(this IStorageService storageService, T data, object token)
        {
            return storageService.UpdateTokenDataLatent(data, token, typeof (T));
        }

        public static void UpdateTokenData<T>(this IStorageService storageService, T data, object token)
        {
            storageService.UpdateTokenData(data,token,typeof(T));
        }

        public static T ResolveTokenData<T>(this IStorageService storageService, object token)
        {
            return (T) storageService.ResolveTokenData(typeof (T), token);
        }
    }
}