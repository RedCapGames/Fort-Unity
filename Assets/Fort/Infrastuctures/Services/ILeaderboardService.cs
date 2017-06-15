using UnityEngine;
using System.Collections;

namespace Fort
{
    public interface ILeaderboardService
    {
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
