namespace Fort.ServerConnection
{
    public interface IServerConnectionProvider
    {
        IUserConnection UserConnection { get; }
        IEditorConnection EditorConnection { get; }
    }

    public interface ICallError
    {
        CallErrorType ErrorType { get; }
    }

    public enum CallErrorType
    {
        MethodConversionFailed,
        Other
    }
}