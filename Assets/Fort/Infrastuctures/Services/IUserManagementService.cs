using Fort.Info;
using Fort.ServerConnection;

namespace Fort
{
    public interface IUserManagementService
    {
        bool IsRegistered { get; }
        string Username { get; }
        int Score { get; }
        Balance Balance { get; }
        ErrorPromise<RegisterationErrorResultStatus> Register(string username,string password);
        Promise Login(string username, string password);
        void AddScoreAndBalance(int score,Balance balance);
        Promise FullUpdate();
        string GetSystemId();
    }


}