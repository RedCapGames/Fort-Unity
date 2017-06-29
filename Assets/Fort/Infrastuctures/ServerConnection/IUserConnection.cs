using System;
using System.Net;

namespace Fort.ServerConnection
{
    public interface IUserConnection
    {
        ErrorPromise<RegisterationErrorResultStatus> Register(string username, string password);
        Promise Login(string username, string password);
        Promise Relogin();
        bool IsReloginCapable();
        Promise<T, ICallError> Call<T>(string methodName, object requestBody);
        Uri GetStorageAddress();
        ComplitionPromise<string> LoadFromStorage(Uri uri,Action<DownloadProgress> progress);
        bool IsCached(Uri uri);
        string LoadFromCache(Uri uri);
    }
    public enum RegisterationErrorResultStatus
    {
        CannotConnectToServer,
        UsernameIsInUse
    }

    public class DownloadProgress
    {
        public DownloadProgress(long totalSize, long progress)
        {
            TotalSize = totalSize;
            Progress = progress;
        }

        public long TotalSize { get;private set; }
        public long Progress { get; private set; }

        public float NormalPosition
        {
            get
            {
                if (TotalSize == 0)
                    return 0;
                return (float)Progress/TotalSize;
            }
        }
    }
}