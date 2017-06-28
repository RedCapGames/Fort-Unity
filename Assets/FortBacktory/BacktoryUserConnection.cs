using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Assets.FortBacktory;
using Fort.Dispatcher;
using Fort.Info;
using Fort.ServerConnection;
using FortBacktory.Info;
using Newtonsoft.Json;

namespace Fort.Backtory
{
    public class BacktoryUserConnection : IUserConnection
    {
        public BacktoryUserConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        }

        #region Implementation of IUserConnection

        public ErrorPromise<RegisterationErrorResultStatus> Register(string username, string password)
        {
            IDispatcher dispatcher = GameDispatcher.Dispatcher;
            ErrorDeferred< RegisterationErrorResultStatus > deferred = new ErrorDeferred<RegisterationErrorResultStatus>();
            string authenticationId = InfoResolver.Resolve<BacktoryInfo>().AuthenticationId;
            string url = "https://api.backtory.com/auth/users";
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    webRequest.KeepAlive = true;
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/json; charset=utf-8";
                    webRequest.Headers.Add("X-Backtory-Authentication-Id", authenticationId);

                    string body = JsonConvert.SerializeObject(new { username, password });
                    byte[] resoponse = Encoding.UTF8.GetBytes(body);
                    webRequest.ContentLength = resoponse.Length;
                    using (System.IO.Stream requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(resoponse, 0, resoponse.Length);
                    }
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (System.IO.Stream responseStream = webResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                reader.ReadToEnd();
                            }

                        }
                    }
                    dispatcher.Dispach(() =>
                    {
                        deferred.Resolve();
                    });
                }
                catch (WebException we)
                {
                    dispatcher.Dispach(() =>
                    {
                        HttpWebResponse httpWebResponse = we.Response as HttpWebResponse;
                        if (httpWebResponse != null)
                        {
                            if (httpWebResponse.StatusCode == HttpStatusCode.Conflict)
                            {
                                deferred.Reject(RegisterationErrorResultStatus.UsernameIsInUse);
                                return;
                            }
                        }
                        deferred.Reject(RegisterationErrorResultStatus.CannotConnectToServer);
                    });
                }
            });

            return deferred.Promise();
        }

        public static string MultiPartCall(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            webRequest.KeepAlive = true;
            webRequest.Method = "POST";
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            PostData postData = new PostData();
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                postData.Params.Add(new PostDataParam(pair.Key, pair.Value, PostDataParamType.Field));
            }
            string s = postData.GetPostData();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            foreach (KeyValuePair<string, string> pair in headers)
            {
                webRequest.Headers.Add(pair.Key, pair.Value);
            }
            webRequest.ContentType = "multipart/form-data; boundary=----WebKitFormBoundaryS3pOJgmMVoMmQZ9Y";
            webRequest.ContentLength = bytes.Length;
            using (System.IO.Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (System.IO.Stream responseStream = webResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }

                }
            }
        }
        public Promise Login(string username, string password)
        {
            IDispatcher dispatcher = GameDispatcher.Dispatcher;
            Deferred deferred = new Deferred();

            string authenticationId = InfoResolver.Resolve<BacktoryInfo>().AuthenticationId;
            string authenticationClientKey = InfoResolver.Resolve<BacktoryInfo>().AuthenticationClientKey;
            string url = "https://api.backtory.com/auth/login";
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    string result = MultiPartCall(url, new Dictionary<string, string>
                    {
                        {"username", username},
                        {"password", password}
                    }, new Dictionary<string, string>
                    {
                        {"X-Backtory-Authentication-Id", authenticationId},
                        {"X-Backtory-Authentication-Key", authenticationClientKey}
                    });
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);
                    dispatcher.Dispach(() =>
                    {
                        if (string.IsNullOrEmpty(loginResponse.AccessToken))
                            deferred.Reject();
                        else
                        {
                            BacktoryAccessData backtoryAccessData = ServiceLocator.Resolve<IStorageService>().ResolveData<BacktoryAccessData>() ?? new BacktoryAccessData();
                            backtoryAccessData.AccessToken = loginResponse.AccessToken;
                            backtoryAccessData.RefreshToken = loginResponse.RefreshToken;
                            backtoryAccessData.TokenType = loginResponse.TokenType;
                            ServiceLocator.Resolve<IStorageService>().UpdateData(backtoryAccessData);
                            deferred.Resolve();
                        }
                    });
                }
                catch (Exception)
                {
                    dispatcher.Dispach(() =>
                    {
                        deferred.Reject();
                    });
                }

            });
            return deferred.Promise();
        }

        public Promise Relogin()
        {
            IDispatcher dispatcher = GameDispatcher.Dispatcher;
            Deferred deferred = new Deferred();
            if (!IsReloginCapable())
                throw new Exception("Relogin is not capable.No saved refresh token found");
            string authenticationId = InfoResolver.Resolve<BacktoryInfo>().AuthenticationId;
            string authenticationClientKey = InfoResolver.Resolve<BacktoryInfo>().AuthenticationClientKey;
            string url = "https://api.backtory.com/auth/login";
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    BacktoryAccessData backtoryAccessData = ServiceLocator.Resolve<IStorageService>().ResolveData<BacktoryAccessData>();
                    string result = MultiPartCall(url, new Dictionary<string, string>
                    {
                        {"refresh_token", backtoryAccessData.RefreshToken},
                        
                    }, new Dictionary<string, string>
                    {
                        {"X-Backtory-Authentication-Id", authenticationId},
                        {"X-Backtory-Authentication-Key", authenticationClientKey},
                        {"X-Backtory-Authentication-Refresh", "1"}
                    });
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);
                    dispatcher.Dispach(() =>
                    {
                        if (string.IsNullOrEmpty(loginResponse.AccessToken))
                            deferred.Reject();
                        else
                        {
                            backtoryAccessData = ServiceLocator.Resolve<IStorageService>().ResolveData<BacktoryAccessData>();
                            backtoryAccessData.AccessToken = loginResponse.AccessToken;
                            backtoryAccessData.RefreshToken = loginResponse.RefreshToken;
                            backtoryAccessData.TokenType = loginResponse.TokenType;
                            ServiceLocator.Resolve<IStorageService>().UpdateData(backtoryAccessData);
                            deferred.Resolve();
                        }
                    });
                }
                catch (Exception)
                {
                    dispatcher.Dispach(() =>
                    {
                        deferred.Reject();
                    });
                }

            });
            return deferred.Promise();
        }

        public bool IsReloginCapable()
        {
            return ServiceLocator.Resolve<IStorageService>().ContainsData<BacktoryAccessData>();
        }

        public Promise<T, ICallError> Call<T>(string methodName, object requestBody)
        {
            IDispatcher dispatcher = GameDispatcher.Dispatcher;
            Deferred<T, ICallError> deferred = new Deferred<T, ICallError>();            
            string url = string.Format("https://api.backtory.com/cloud-code/{0}/{1}", InfoResolver.Resolve<BacktoryInfo>().CloudId,methodName);
            if(!string.IsNullOrEmpty(BacktoryCloudUrl.Url))
                url = new Uri(new Uri(BacktoryCloudUrl.Url),new Uri(string.Format("/{0}",methodName))).ToString();
            string authorization = string.Empty;
            if (ServiceLocator.Resolve<IStorageService>().ContainsData<BacktoryAccessData>())
            {
                BacktoryAccessData backtoryAccessData =
                    ServiceLocator.Resolve<IStorageService>().ResolveData<BacktoryAccessData>();
                authorization = string.Format("{0} {1}", backtoryAccessData.TokenType, backtoryAccessData.AccessToken);
            }
            string body = JsonConvert.SerializeObject(requestBody);
            if (body == "null")
                body = "{}";
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    webRequest.KeepAlive = true;
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/json; charset=utf-8";
                    if (!string.IsNullOrEmpty(authorization))
                        webRequest.Headers.Add("Authorization", authorization);

                    
                    byte[] resoponse = Encoding.UTF8.GetBytes(body);
                    webRequest.ContentLength = resoponse.Length;
                    using (System.IO.Stream requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(resoponse, 0, resoponse.Length);
                    }
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (System.IO.Stream responseStream = webResponse.GetResponseStream())
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
                                        deferred.Reject(new BactkoryCallError(HttpStatusCode.Continue, CallErrorType.MethodConversionFailed));
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
                            deferred.Reject(new BactkoryCallError(httpWebResponse.StatusCode, CallErrorType.Other));
                        }
                        deferred.Reject(new BactkoryCallError(HttpStatusCode.Continue,CallErrorType.Other ));
                    });
                }
                catch (Exception e)
                {
                    deferred.Reject(new BactkoryCallError(HttpStatusCode.Continue, CallErrorType.Other));
                }
            });

            return deferred.Promise();
        }

        #endregion

        private class LoginResponse
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


        class BacktoryAccessData
        {
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public string RefreshToken { get; set; }
        }
        class PostData
        {
            private static string boundary = "----WebKitFormBoundaryS3pOJgmMVoMmQZ9Y";

            private List<PostDataParam> m_Params;

            public List<PostDataParam> Params
            {
                get { return m_Params; }
                set { m_Params = value; }
            }

            public PostData()
            {
                m_Params = new List<PostDataParam>();
            }
            public string GetPostData()
            {
                StringBuilder sb = new StringBuilder();
                foreach (PostDataParam p in m_Params)
                {
                    sb.AppendLine("--" + boundary);

                    if (p.Type == PostDataParamType.File)
                    {
                        sb.AppendLine(string.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", p.Name, p.FileName));
                        sb.AppendLine("Content-Type: application/octet-stream");
                        sb.AppendLine();
                        sb.AppendLine(p.Value);
                    }
                    else
                    {
                        sb.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", p.Name));
                        sb.AppendLine();
                        sb.AppendLine(p.Value);
                    }
                }

                sb.AppendLine("--" + boundary + "--");

                return sb.ToString();
            }
        }

        enum PostDataParamType
        {
            Field,
            File
        }

        class PostDataParam
        {
            public PostDataParam(string name, string value, PostDataParamType type)
            {
                Name = name;
                Value = value;
                Type = type;
            }

            public PostDataParam(string name, string filename, string value, PostDataParamType type)
            {
                Name = name;
                Value = value;
                FileName = filename;
                Type = type;
            }

            public string Name;
            public string FileName;
            public string Value;
            public PostDataParamType Type;
        }
    }
}