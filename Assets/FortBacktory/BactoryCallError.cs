using System.Net;
using Fort.ServerConnection;

namespace Fort.Backtory
{
    public class BactkoryCallError: ICallError
    {
        public BactkoryCallError(HttpStatusCode responceStatus, CallErrorType errorType)
        {
            
            ResponceStatus = responceStatus;
            ErrorType = errorType;
            if (responceStatus == HttpStatusCode.Unauthorized)
                ErrorType = CallErrorType.UnAuthorize;
        }

        #region Implementation of ICallError
        
        public HttpStatusCode ResponceStatus { get; private set; }

        #endregion

        #region Implementation of ICallError

        public CallErrorType ErrorType { get; private set; }

        #endregion
    }
}