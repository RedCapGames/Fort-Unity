using System;
using System.Collections.Generic;

namespace Fort
{
    public interface IInvitationService
    {
        void ShareLink();
        ComplitionPromise<InvitationData> ResolveInvitationData();
        string ResolveInvitationToken();
        Promise ApplyInvitation();
    }

    [Serializable]
    public class InvitationData
    {
        public int InvitationCount { get; set; }
        public Dictionary<string,int> InvitationAddedValues { get; set; }
        public string InvitorUserName { get; set; }
    }
}
