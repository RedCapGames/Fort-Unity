namespace Fort.ServerConnection
{
    public interface IEditorConnection
    {
        Promise<T, ICallError> Call<T>(string methodName, object requestBody);
    }
}
