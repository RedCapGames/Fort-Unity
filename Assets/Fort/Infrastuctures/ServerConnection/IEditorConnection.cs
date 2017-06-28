using System;

namespace Fort.ServerConnection
{
    public interface IEditorConnection
    {
        Promise<T, ICallError> Call<T>(string methodName, object requestBody);
        ComplitionPromise<string[]> SendFilesToStorage(StorageFile[] storageFiles, Action<float> progress);
    }

    public class StorageFile
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public System.IO.Stream Stream { get; set; }
    }
}
