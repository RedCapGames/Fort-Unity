using Fort.Info;

namespace Fort
{
    public interface IUserManagementService
    {
        bool IsRegistered { get; }
        string Username { get; }
        int Score { get; }
        Balance Balance { get; }
        ErrorPromise<RegisterationErrorResultStatus> Register(string username,string password);
        Promise Login(string userName, string password);
        void AddScoreAndBalance(int score,Balance balance);
        Promise FullUpdate();
        string GetSystemId();
    }

    public enum RegisterationErrorResultStatus
    {
        CannotConnectToServer,
        UsernameIsInUse
    }
}