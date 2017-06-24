namespace Fort
{
    public interface IServerService
    {
        ComplitionPromise<T> Call<T>(string functionName, object requestBody);
        ComplitionPromise<T> CallTokenFull<T>(string functionName, object requestBody);
    }

    public static class ServerServiceExtensions
    {
        public static ComplitionPromise<T> Call<T>(this IServerService serverService, string functionName)
        {
            return serverService.Call<T>(functionName, null);
        }
        public static ComplitionPromise<T> CallTokenFull<T>(this IServerService serverService, string functionName)
        {
            return serverService.CallTokenFull<T>(functionName, null);
        }

    }
}
