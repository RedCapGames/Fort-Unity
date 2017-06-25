using System;
using System.Linq;
using Fort.Info;
using Fort.ServerConnection;
//using Backtory.Core.Public;
using Newtonsoft.Json;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IServerService))]
    public class ServerService : MonoBehaviour, IServerService
    {
        #region Implementation of IServerService

        public ComplitionPromise<T> Call<T>(string functionName, object requestBody)
        {
            ComplitionDeferred<T> deferred = new ComplitionDeferred<T>();
            InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.Call<T>(functionName,requestBody).Then(obj => deferred.Resolve(obj),
                error =>
                {
                    if (error.ErrorType == CallErrorType.UnAuthorize)
                    {
                        if (!ServiceLocator.Resolve<IUserManagementService>().IsRegistered)
                            deferred.Reject();
                        else
                        {
                            if (
                                !InfoResolver.Resolve<FortInfo>()
                                    .ServerConnectionProvider.UserConnection.IsReloginCapable())
                                deferred.Reject();
                            else
                                InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.Relogin().Then(
                                    () =>
                                    {
                                        InfoResolver.Resolve<FortInfo>()
                                            .ServerConnectionProvider.UserConnection.Call<T>(functionName, requestBody)
                                            .Then(obj => deferred.Resolve(obj), callError => deferred.Reject());
                                    }, () => deferred.Reject());
                        }
                    }
                });
            
            return deferred.Promise();
        }

        public ComplitionPromise<T> CallTokenFull<T>(string functionName, object requestBody)
        {
            ComplitionDeferred<T> deferred = new ComplitionDeferred<T>();
            Call<string>("GetToken", null).Then(token =>
             {
                 int[] tokens = GetTokens();

                 Call<T>(functionName,
                     new TokenFullData
                     {
                         Data = requestBody,
                         Tokens =
                             new Guid(token).ToByteArray()
                                 .Select(b => (int)b)
                                 .Select((i, index) => i ^ tokens[index])
                                 .ToArray()
                     }).Then(obj => deferred.Resolve(obj), () => deferred.Reject());
             }, () => deferred.Reject());
            return deferred.Promise();
        }

        private int[] GetTokens()
        {
            return FortEncryptionKey.ResolveKey().Select(b => (int)b).ToArray();
        }
        #endregion

        private class TokenFullData
        {
            [JsonProperty("tokens")]
            public int[] Tokens { get; set; }
            [JsonProperty("data")]
            public object Data { get; set; }
        }
    }

}
