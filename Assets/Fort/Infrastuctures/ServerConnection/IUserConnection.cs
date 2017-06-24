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
    }
    public enum RegisterationErrorResultStatus
    {
        CannotConnectToServer,
        UsernameIsInUse
    }
}