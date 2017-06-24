using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Assets.Fort.Editor.Dispatcher;
using Fort.Dispatcher;
using Fort.ServerConnection;
using Newtonsoft.Json;
using UnityEditor;

namespace Fort.Backtory
{
    public class FortBacktoryEditorConnection:IEditorConnection
    {
		
        public FortBacktoryEditorConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        }
        private ComplitionPromise<BactoryMasterLoginResponse> MasterLogin()
        {
            ComplitionDeferred<BactoryMasterLoginResponse> deferred = new ComplitionDeferred<BactoryMasterLoginResponse>();
            string authenticationId = "5911f0bee4b051bb1e6e2ecb";
            string authenticationMasterKey = "a28ecd5fdc4f423ea9fd56f0";
            string url = "http://api.backtory.com/auth/login";
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                webRequest.KeepAlive = true;
                webRequest.Method = "POST";
                webRequest.Headers.Add("X-Backtory-Authentication-Id", authenticationId);
                webRequest.Headers.Add("X-Backtory-Authentication-Key", authenticationMasterKey);
                webRequest.ContentLength = 0;
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            BactoryMasterLoginResponse bactoryMasterLoginResponse = JsonConvert.DeserializeObject<BactoryMasterLoginResponse>(reader.ReadToEnd());
                            deferred.Resolve(bactoryMasterLoginResponse);
                        }
                    }
                }

            }
            catch (Exception)
            {
                deferred.Reject();
            }

            return deferred.Promise();
        }

        #region Implementation of IEditorConnection

        public Promise<T, ICallError> Call<T>(string methodName, object requestBody)
        {
            
            Deferred<T,ICallError> deferred = new Deferred<T, ICallError>();
            IDispatcher dispatcher = EditorDispatcher.Dispatcher;            
            string url = string.Format("https://api.backtory.com/cloud-code/{0}/{1}", "5911f0bfe4b051bb1e6e2ecf", methodName);
            ThreadPool.QueueUserWorkItem(state =>
            {
                MasterLogin().Then(response =>
                {
                    string authorization = string.Format("{0} {1}", response.TokenType, response.AccessToken);
                    try
                    {
                        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(new Uri(url));
                        webRequest.KeepAlive = true;
                        webRequest.Method = "POST";
                        webRequest.ContentType = "application/json; charset=utf-8";
                        if (!string.IsNullOrEmpty(authorization))
                            webRequest.Headers.Add("Authorization", authorization);

                        string body = JsonConvert.SerializeObject(requestBody);
                        byte[] resoponse = Encoding.UTF8.GetBytes(body);
                        webRequest.ContentLength = resoponse.Length;
                        using (Stream requestStream = webRequest.GetRequestStream())
                        {
                            requestStream.Write(resoponse, 0, resoponse.Length);
                        }
                        using (WebResponse webResponse = webRequest.GetResponse())
                        {
                            using (Stream responseStream = webResponse.GetResponseStream())
                            {
                                using (StreamReader reader = new StreamReader(responseStream))
                                {
                                    string readToEnd = reader.ReadToEnd();
                                    dispatcher.Dispach(() =>
                                    {

                                        try
                                        {
                                            T result = JsonConvert.DeserializeObject<T>(readToEnd);
                                            deferred.Resolve(result);
                                        }
                                        catch (Exception)
                                        {
                                            deferred.Reject(new BactkoryCallError(HttpStatusCode.Continue,
                                                CallErrorType.MethodConversionFailed));
                                        }

                                    });



                                }
                            }
                        }
                        dispatcher.Dispach(() =>
                        {
                            deferred.Resolve(default(T));
                        });
                    }
                    catch (WebException we)
                    {
                        dispatcher.Dispach(() =>
                        {
                            HttpWebResponse httpWebResponse = we.Response as HttpWebResponse;
                            if (httpWebResponse != null)
                            {
                                deferred.Reject(new BacktoryEditorCallError(CallErrorType.Other, false,
                                    httpWebResponse.StatusCode));
                            }
                            deferred.Reject(new BacktoryEditorCallError(CallErrorType.Other, false,
                                HttpStatusCode.Continue));
                        });
                    }
                    catch (Exception e)
                    {
                        deferred.Reject(new BacktoryEditorCallError(CallErrorType.Other, false, HttpStatusCode.Continue));
                    }
                }, () =>
                {
                    dispatcher.Dispach(() =>
                    {
                        deferred.Reject(new BacktoryEditorCallError(CallErrorType.Other, true, HttpStatusCode.Continue));
                    });
                });
            });
            return deferred.Promise();
        }

        #endregion

        private class BactoryMasterLoginResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonProperty("scope")]
            public string Scope { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
        }
    }

    public class BacktoryEditorCallError : ICallError
    {
        public BacktoryEditorCallError(CallErrorType errorType, bool masterLoginFailed, HttpStatusCode responceStatus)
        {
            ErrorType = errorType;
            MasterLoginFailed = masterLoginFailed;
            ResponceStatus = responceStatus;
        }

        #region Implementation of ICallError

        public CallErrorType ErrorType { get; private set; }

        #endregion

        public bool MasterLoginFailed { get; private set; }
        public HttpStatusCode ResponceStatus { get; private set; }
    }


}
