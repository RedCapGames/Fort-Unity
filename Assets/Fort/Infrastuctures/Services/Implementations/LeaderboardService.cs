using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fort
{
    [Service(ServiceType = typeof(ILeaderboardService))]
    public class LeaderboardService : MonoBehaviour, ILeaderboardService
    {
        #region Implementation of ILeaderboardService

        public ComplitionPromise<LeaderBoardUser[]> ResolveLeaderBoardUsers()
        {
            if (!ServiceLocator.Resolve<IUserManagementService>().IsRegistered)
                throw new Exception("Leaderboard can only be resolve for registered users");
            ComplitionDeferred<LeaderBoardUser[]> deferred = new ComplitionDeferred<LeaderBoardUser[]>();
            ServiceLocator.Resolve<IUserManagementService>().FullUpdate().Then(() =>
            {
                ServiceLocator.Resolve<IServerService>().Call<LeaderboardData>("GetLeaderboard").Then(data =>
                {
                    List<LeaderBoardUser> leaderBoardUsers = new List<LeaderBoardUser>();
                    string username = ServiceLocator.Resolve<IUserManagementService>().Username;
                    for (int i = 0; i < data.TopPlayers.Length; i++)
                    {
                        leaderBoardUsers.Add(new LeaderBoardUser
                        {
                            UserName = data.TopPlayers[i].UserName,
                            Score = data.TopPlayers[i].Score,
                            Rank = i,
                            IsCurrentUser = string.Equals(data.TopPlayers[i].UserName, username)
                        });
                    }

                    int userRankStartRank = data.Rank -
                        data.UserRank.ToList().FindIndex(user => string.Equals(user.UserName, username));
                    for (int i = 0; i < data.UserRank.Length; i++)
                    {
                        if (!leaderBoardUsers.Any(user => string.Equals(user.UserName, data.UserRank[i].UserName)))
                        {
                            leaderBoardUsers.Add(new LeaderBoardUser
                            {
                                UserName = data.UserRank[i].UserName,
                                Score = data.UserRank[i].Score,
                                Rank = userRankStartRank+i,
                                IsCurrentUser = string.Equals(data.UserRank[i].UserName, username)
                            });
                        }
                    }
                    leaderBoardUsers.Sort((user, boardUser) => user.Score.CompareTo(boardUser.Rank));
                    deferred.Resolve(leaderBoardUsers.ToArray());
                }, () => deferred.Reject());

            }, () => deferred.Reject());
            return deferred.Promise();
        }

        #endregion
        private class LeaderboardData
        {
            public ServerLeaderboardUser[] TopPlayers { get; set; }
            public int Rank { get; set; }
            public ServerLeaderboardUser[] UserRank { get; set; }
        }
        private class ServerLeaderboardUser
        {
            public string UserName { get; set; }
            public int Score { get; set; }
        }
    }


}