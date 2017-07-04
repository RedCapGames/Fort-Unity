using UnityEngine;
using System.Collections;

namespace Fort
{
    /// <summary>
    /// This service is use to load leaderboard data
    /// </summary>
    public interface ILeaderboardService
    {
        /// <summary>
        /// Resolve leaderboard user data
        /// </summary>
        /// <returns>promise leaderboard user list</returns>
        ComplitionPromise<LeaderBoardUser[]> ResolveLeaderBoardUsers();
    }

    public class LeaderBoardUser
    {
        public bool IsCurrentUser { get; set; }
        public int Rank { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
    }
}
