using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Assets.Fort.Editor.Dispatcher;
using Fort.Dispatcher;
using Fort.Info;
using Fort.ServerConnection;
using Fort.Stream;
using FortBacktory.Info;
using Newtonsoft.Json;
using UnityEngine;

namespace Fort.Backtory
{
    public class FortBacktoryEditorConnection:IEditorConnection
    {
		
        public FortBacktoryEditorConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        }
        private ComplitionPromise<BactoryMasterLoginResponse> MasterLogin(string authenticationId, string authenticationMasterKey)
        {
            ComplitionDeferred<BactoryMasterLoginResponse> deferred = new ComplitionDeferred<BactoryMasterLoginResponse>();

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
                    using (System.IO.Stream responseStream = webResponse.GetResponseStream())
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
            string url = string.Format("https://api.backtory.com/cloud-code/{0}/{1}", InfoResolver.Resolve<BacktoryInfo>().CloudId, methodName);
            if (!string.IsNullOrEmpty(BacktoryCloudUrl.Url))
                url = new Uri(new Uri(BacktoryCloudUrl.Url), new Uri(string.Format("/{0}", methodName),UriKind.Relative)).ToString();
            string authenticationId = InfoResolver.Resolve<BacktoryInfo>().AuthenticationId;
            string authenticationMasterKey = EditorInfoResolver.Resolve<BacktoryEditorInfo>().AuthenticationMasterKey;
            string body = JsonConvert.SerializeObject(requestBody);
            if (body == "null")
                body = "{}";
            ThreadPool.QueueUserWorkItem(state =>
            {
                MasterLogin(authenticationId, authenticationMasterKey).Then(response =>
                {
                    string authorization = string.Format("{0} {1}", response.TokenType, response.AccessToken);
                    try
                    {
                        string iUrl = url;
                        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(new Uri(iUrl));
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
                                            deferred.Reject(new BactkoryCallError(HttpStatusCode.Continue,
                                                CallErrorType.MethodConversionFailed));
                                        }

                                    });
                                }
                            }
                        }
                    }
                    catch (WebException we)
                    {
                        Debug.LogException(we);
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
                    catch (Exception)
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

        public ComplitionPromise<string[]> SendFilesToStorage(StorageFile[] storageFiles, Action<float> progress)
        {
            ComplitionDeferred<string[]> deferred = new ComplitionDeferred<string[]>();
            if (storageFiles.Length == 0)
            {
                deferred.Resolve(new string[0]);
                return deferred.Promise();
            }
            string boundary = "----WebKitFormBoundaryS3pOJgmMVoMmQZ9Y";
            MultiPartFormDataStream dataStream = new MultiPartFormDataStream(boundary, storageFiles.Select((file, i) =>
            {

                MultiPartParameter[] multiPartParameters = new MultiPartParameter[4];
                multiPartParameters[0] = new StreamMultiPartParameter(boundary, string.Format("fileItems[{0}].fileToUpload", i), file.FileName, file.Stream);
                multiPartParameters[1] = new StringMultiPartParameter(boundary, string.Format("fileItems[{0}].path", i), file.Path);
                multiPartParameters[2] = new StringMultiPartParameter(boundary, string.Format("fileItems[{0}].replacing", i), "true");
                multiPartParameters[3] = new StringMultiPartParameter(boundary, string.Format("fileItems[{0}].extract", i), "false");
                return multiPartParameters;
            }).SelectMany(parameters => parameters).ToArray());
/*            using (FileStream test = File.Create(@"d:\1.log"))
            {
                dataStream.CopyTo(test);

            }
            return null;*/
            string authenticationId = InfoResolver.Resolve<BacktoryInfo>().AuthenticationId;
            string authenticationMasterKey = EditorInfoResolver.Resolve<BacktoryEditorInfo>().AuthenticationMasterKey;
            string storageId = EditorInfoResolver.Resolve<BacktoryEditorInfo>().StorageId;
            string url = "https://storage.backtory.com/files";
            IDispatcher dispatcher = EditorDispatcher.Dispatcher;
            ThreadPool.QueueUserWorkItem(state =>
            {
                MasterLogin(authenticationId, authenticationMasterKey).Then(response =>
                {
                    try
                    {
                        string authorization = string.Format("{0} {1}", response.TokenType, response.AccessToken);
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                        webRequest.KeepAlive = true;
                        webRequest.Method = "POST";

                        webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                        webRequest.Headers.Add("x-backtory-storage-id", storageId);
                        webRequest.Headers.Add("Authorization", authorization);
                        webRequest.ContentLength = dataStream.Length;
                        using (System.IO.Stream requestStream = webRequest.GetRequestStream())
                        {

                            int bytesRead;
                            byte[] buffer = new byte[2048];
                            long wroteSize = 0;
                            while ((bytesRead = dataStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                requestStream.Write(buffer, 0, bytesRead);
                                wroteSize += bytesRead;
                                float normalPosition = (float)wroteSize / dataStream.Length;
                                dispatcher.Dispach(() =>
                                {
                                    progress(normalPosition);
                                });
                            }
                            using (WebResponse webResponse = webRequest.GetResponse())
                            {
                                using (System.IO.Stream responseStream = webResponse.GetResponseStream())
                                {
                                    using (StreamReader reader = new StreamReader(responseStream))
                                    {
                                        string readToEnd = reader.ReadToEnd();
                                        BacktoryStorageResoponce backtoryStorageResoponce = JsonConvert.DeserializeObject<BacktoryStorageResoponce>(readToEnd);
                                        foreach (StorageFile storageFile in storageFiles)
                                        {
                                            storageFile.Stream.Close();
                                        }
                                        dispatcher.Dispach(() => deferred.Resolve(backtoryStorageResoponce.SavedFilesUrls));

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        foreach (StorageFile storageFile in storageFiles)
                        {
                            storageFile.Stream.Close();
                        }
                        Debug.LogException(e);
                        dispatcher.Dispach(() => deferred.Reject());
                    }
                }, () =>
                {
                    foreach (StorageFile storageFile in storageFiles)
                    {
                        storageFile.Stream.Close();
                    }
                    dispatcher.Dispach(() => deferred.Reject());
                });
            });
            return deferred.Promise();
        }

        /*public void SendFiles()
        {
            string authenticationId = InfoResolver.Resolve<BacktoryInfo>().AuthenticationId;
            string authenticationMasterKey = EditorInfoResolver.Resolve<BacktoryEditorInfo>().AuthenticationMasterKey;
            string storageId = EditorInfoResolver.Resolve<BacktoryEditorInfo>().StorageId;
            string url = "https://storage.backtory.com/files";
            MasterLogin(authenticationId,authenticationMasterKey).Then(response =>
            {
                string authorization = string.Format("{0} {1}", response.TokenType, response.AccessToken);
                string boundary = "----WebKitFormBoundaryS3pOJgmMVoMmQZ9Y";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                webRequest.KeepAlive = true;
                webRequest.Method = "POST";
                
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webRequest.Headers.Add("x-backtory-storage-id", storageId);
                webRequest.Headers.Add("Authorization", authorization);
                using (FileStream stream = File.OpenRead(@"d:\StateNames.txt"))
                {
                    MultiPartFormDataStream dataStream =
                        new MultiPartFormDataStream(boundary,new StreamMultiPartParameter(boundary, "fileItems[0].fileToUpload", "StateNames.txt", stream)
                        ,new StringMultiPartParameter(boundary, "fileItems[0].path", "/")
                        , new StringMultiPartParameter(boundary, "fileItems[0].replacing", "true")
                        , new StringMultiPartParameter(boundary, "fileItems[0].extract", "false"));
/*                    using (FileStream test = File.Create(@"d:\1.log"))
                    {
                        dataStream.CopyTo(test);

                    }#1#
                    webRequest.ContentLength = dataStream.Length;
                    using (System.IO.Stream requestStream = webRequest.GetRequestStream())
                    {
                        dataStream.CopyTo(requestStream);
                    }
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (System.IO.Stream responseStream = webResponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                string readToEnd = reader.ReadToEnd();
                            }
                        }
                    }
                }
            });
        }*/
        #endregion

/*        [MenuItem("Fort/SendFiles")]
        public static void SendFilesToServer()
        {
            FortBacktoryEditorConnection fortBacktoryEditorConnection = new FortBacktoryEditorConnection();
            fortBacktoryEditorConnection.SendFiles();
        }*/

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

        private class BacktoryStorageResoponce
        {
            [JsonProperty("savedFilesUrls")]
            public string[] SavedFilesUrls { get; set; }
        }
    }

    public class BacktoryEditorCallError : ICallError
    {
        public BacktoryEditorCallError(CallErrorType errorType, bool masterLoginFailed, HttpStatusCode responceStatus)
        {
            ErrorType = errorType;
            MasterLoginFailed = masterLoginFailed;
            ResponceStatus = responceStatus;
            if (responceStatus == HttpStatusCode.Unauthorized)
                ErrorType = CallErrorType.UnAuthorize;
        }

        #region Implementation of ICallError

        public CallErrorType ErrorType { get; private set; }

        #endregion

        public bool MasterLoginFailed { get; private set; }
        public HttpStatusCode ResponceStatus { get; private set; }
    }


}
