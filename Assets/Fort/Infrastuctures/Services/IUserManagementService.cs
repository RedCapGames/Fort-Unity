using Fort.Info;
using Fort.ServerConnection;

namespace Fort
{
    /// <summary>
    /// Service to manage user
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Is user registered
        /// </summary>
        bool IsRegistered { get; }
        /// <summary>
        /// Get username
        /// </summary>
        string Username { get; }
        /// <summary>
        /// Get user score
        /// </summary>
        int Score { get; }
        /// <summary>
        /// Get user balance
        /// </summary>
        Balance Balance { get; }
        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="username">The username of user</param>
        /// <param name="password">The password of user</param>
        /// <returns></returns>
        ErrorPromise<RegisterationErrorResultStatus> Register(string username,string password);
        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="username">The username of user</param>
        /// <param name="password">The password of user</param>
        /// <returns></returns>
        Promise Login(string username, string password);
        /// <summary>
        /// Adding score and balance.Data will update to server in full update
        /// </summary>
        /// <param name="score">Added score</param>
        /// <param name="balance">Added balance</param>
        void AddScoreAndBalance(int score,Balance balance);
        /// <summary>
        /// Full update user score, balance, claimed achievement and purchased item
        /// </summary>
        /// <returns>Promise of update</returns>
        Promise FullUpdate();
        /// <summary>
        /// return a unique id of system
        /// </summary>
        /// <returns>Unique id of system</returns>
        string GetSystemId();
    }


}