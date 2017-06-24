using System;
using Fort.ServerConnection;

namespace Fort.Backtory
{
    public class BacktoryServerConnectionProvider : IServerConnectionProvider
    {
        public BacktoryServerConnectionProvider()
        {            
            UserConnection = new BacktoryUserConnection();
            EditorConnection =
                (IEditorConnection)
                    Activator.CreateInstance(TypeHelper.EditorType("Fort.Backtory.BacktoryEditorConnection"));
        }
        #region Implementation of IServerConnectionProvider

        public IUserConnection UserConnection { get; private set; }
        public IEditorConnection EditorConnection { get; private set; }

        #endregion
    }
}
