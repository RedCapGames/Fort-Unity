using System;
using Fort.Inspector;
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
                    Activator.CreateInstance(TypeHelper.EditorType("Fort.Backtory.FortBacktoryEditorConnection"));
        }
        #region Implementation of IServerConnectionProvider

        [IgnorePresentation]
        public IUserConnection UserConnection { get; private set; }
        [IgnorePresentation]
        public IEditorConnection EditorConnection { get; private set; }

        #endregion
    }
}
