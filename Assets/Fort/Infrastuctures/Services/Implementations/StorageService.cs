using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IStorageService))]
    public class StorageService : MonoBehaviour, IStorageService
    {
        Dictionary<Type, object> _cache = new Dictionary<Type, object>();
        Dictionary<TokenStorage, object> _tokenCache = new Dictionary<TokenStorage, object>();
        Queue<Deferred> _resolvedPromise = new Queue<Deferred>();
        object _defferedLockObject = new object();
        #region Implementation of IStorageService

        public void UpdateOnMemory<T>(T data)
        {
            UpdateOnMemory(data, typeof(T));
        }

        public void SaveOnMemoryData<T>()
        {
            SaveOnMemoryData(typeof(T));
        }

        public Promise SaveOnMemoryDataLatent<T>()
        {
            return SaveOnMemoryDataLatent(typeof(T));
        }

        public bool ContainsData(Type dataType)
        {
            object result;
            if (_cache.TryGetValue(dataType, out result))
                return true;
            ResolveData(dataType);
            if (_cache.TryGetValue(dataType, out result))
                return true;
            return false;
        }

        private string GetTempPath(string path)
        {
            return path + "_temp";
        }
        private object SafeLoadData(string path)
        {
            string tempFile = GetTempPath(path) + ".bin";
            string mainPath = path + ".bin";
            try
            {
                using (FileStream reader = File.OpenRead(mainPath))
                {
                    return LoadData(reader);
                }
            }
            catch (Exception)
            {
                using (FileStream reader = File.OpenRead(tempFile))
                {
                    return LoadData(reader);
                }
            }
        }
        private void SafeSaveData(object data, string path)
        {
            string tempFile = GetTempPath(path) + ".bin";
            string mainPath = path + ".bin";
            using (FileStream fileStream = File.Create(tempFile))
            {
                SaveData(fileStream, data);
            }
            try
            {
                File.Copy(tempFile, mainPath, true);
                File.Delete(tempFile);
            }
            catch
            {
            }
        }
        public object ResolveData(Type dataType)
        {
            try
            {
                object result;
                if (_cache.TryGetValue(dataType, out result))
                {
                    return result;
                }
                lock (this)
                {
                    result =
                        SafeLoadData(Path.Combine(Application.persistentDataPath,
                            string.Format("{0}", Base64Encode(dataType.AssemblyQualifiedName))));
                    _cache[dataType] = result;
                    return result;
                }
            }
            catch (Exception)
            {
                return dataType.GetDefault();
            }
        }

        private object LoadData(System.IO.Stream dataStream)
        {
            using (FortEncryptionStream fortEncryptionStream = new FortEncryptionStream(dataStream))
            {
                Serializer.Serializer serializer = new Serializer.Serializer();
                return serializer.Deserialize(fortEncryptionStream);
            }
        }
        private void SaveData(System.IO.Stream dataStream, object data)
        {
            using (FortEncryptionStream fortEncryptionStream = new FortEncryptionStream(dataStream))
            {
                Serializer.Serializer serializer = new Serializer.Serializer();
                serializer.Serialize(fortEncryptionStream, data);
            }
        }
        public void UpdateData(object data, Type dataType)
        {
            lock (this)
            {
                _cache[dataType] = data;
                string path = Path.Combine(Application.persistentDataPath, string.Format("{0}", Base64Encode(dataType.AssemblyQualifiedName)));
                SafeSaveData(data, path);
            }
        }


        public Promise UpdateDataLatent(object data, Type dataType)
        {
            object latentData = Serializer.Helper.Clone(data);
            _cache[dataType] = data;
            Deferred deferred = new Deferred();
            string path = Path.Combine(Application.persistentDataPath, string.Format("{0}", Base64Encode(dataType.AssemblyQualifiedName)));
            ThreadPool.QueueUserWorkItem(state =>
            {
                lock (this)
                {

                    SafeSaveData(latentData, path);
                    lock (_defferedLockObject)
                    {
                        _resolvedPromise.Enqueue(deferred);
                    }
                }
            });
            return deferred.Promise();
        }

        void Update()
        {
            lock (_defferedLockObject)
            {
                while (_resolvedPromise.Count > 0)
                {
                    Deferred deferred = _resolvedPromise.Dequeue();
                    deferred.Resolve();
                }
            }
        }
        public void UpdateOnMemory(object data, Type dataType)
        {
            _cache[dataType] = data;
        }

        public void SaveOnMemoryData(Type dataType)
        {
            if (_cache.ContainsKey(dataType))
                UpdateData(_cache[dataType], dataType);
        }

        public Promise SaveOnMemoryDataLatent(Type dataType)
        {
            if (_cache.ContainsKey(dataType))
                return UpdateDataLatent(_cache[dataType], dataType);
            Deferred deferred = new Deferred();
            deferred.Reject();
            return deferred.Promise();
        }

        public Promise UpdateTokenDataLatent(object data, string token, Type dataType)
        {
            object latentData = Serializer.Helper.Clone(data);
            string mainPath = string.Format("{0}_{1}", Base64Encode(dataType.AssemblyQualifiedName), token);

            _tokenCache[new TokenStorage
            {
                Token = token.ToString(),
                Type = dataType
            }] = data;
            Deferred deferred = new Deferred();
            string path = Path.Combine(Application.persistentDataPath, mainPath);
            ThreadPool.QueueUserWorkItem(state =>
            {

                lock (this)
                {
                    SafeSaveData(latentData, path);
                    lock (_defferedLockObject)
                    {
                        _resolvedPromise.Enqueue(deferred);
                    }
                }
            });
            return deferred.Promise();
        }

        public void UpdateTokenData(object data, string token, Type dataType)
        {
            string mainPath = string.Format("{0}_{1}", Base64Encode(dataType.AssemblyQualifiedName), token);
            lock (this)
            {
                string path = Path.Combine(Application.persistentDataPath, mainPath);
                SafeSaveData(data, path);
                lock (this)
                {
                    _tokenCache[new TokenStorage
                    {
                        Token = token.ToString(),
                        Type = dataType
                    }] = data;
                }
            }
        }

        public object ResolveTokenData(Type dataType, string token)
        {
            try
            {
                object result;
                TokenStorage tokenStorage = new TokenStorage
                {
                    Token = token.ToString(),
                    Type = dataType
                };
                if (_tokenCache.TryGetValue(tokenStorage, out result))
                {
                    return result;
                }
                lock (this)
                {
                    result = SafeLoadData(Path.Combine(Application.persistentDataPath,
                        string.Format("{0}_{1}", Base64Encode(dataType.AssemblyQualifiedName), token)));
                    _tokenCache[tokenStorage] = result;
                    return result;
                }
            }
            catch (Exception)
            {
                return dataType.GetDefault();
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        class TokenStorage
        {
            #region Equality members

            protected bool Equals(TokenStorage other)
            {
                return Equals(Type, other.Type) && string.Equals(Token, other.Token);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Token != null ? Token.GetHashCode() : 0);
                }
            }

            #endregion

            #region Overrides of Object

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TokenStorage)obj);
            }

            #endregion

            public Type Type { get; set; }
            public string Token { get; set; }
        }
        private class FortEncryptionStream : System.IO.Stream
        {
            private static byte Change(byte data)
            {
                try
                {
                    return FortEncryptionKey.Change(data);
                }
                catch (Exception)
                {
                    return (byte) (data ^ 0xff);
                }
                
            }

            private readonly System.IO.Stream _baseStream;

            public FortEncryptionStream(System.IO.Stream baseStream)
            {
                _baseStream = baseStream;
            }

            #region Overrides of Stream

            public override void Flush()
            {
                _baseStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int size = _baseStream.Read(buffer, offset, count);
                for (int i = 0; i < size; i++)
                {
                    buffer[offset + i] = Change(buffer[offset + i]);
                }
                return size;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                byte[] clone = (byte[])buffer.Clone();
                for (int i = 0; i < clone.Length; i++)
                {
                    clone[i] = Change(clone[i]);
                }
                _baseStream.Write(clone, offset, count);
            }

            public override bool CanRead { get { return _baseStream.CanRead; } }
            public override bool CanSeek { get { return _baseStream.CanSeek; } }
            public override bool CanWrite { get { return _baseStream.CanWrite; } }
            public override long Length { get { return _baseStream.Length; } }
            public override long Position { get { return _baseStream.Position; } set { _baseStream.Position = value; } }

            #endregion
        }


    }
}
