using System;
using System.Collections.Generic;

namespace Fort
{
    /// <summary>
    /// This service is used to manage invitation. 
    /// </summary>
    public interface IInvitationService
    {
        /// <summary>
        /// Share Game link and invitation token 
        /// </summary>
        void ShareLink();
        /// <summary>
        /// Resolve Invititation data from server
        /// </summary>
        /// <returns>Invitaion Data Promise</returns>
        ComplitionPromise<InvitationData> ResolveInvitationData();
        /// <summary>
        /// Resolving invitation token from application url or cache
        /// </summary>
        /// <returns>invitation token</returns>
        string ResolveInvitationToken();
        /// <summary>
        /// Apply invitation from cached Invitation token
        /// </summary>
        /// <returns>Promise of invitation applying</returns>
        Promise ApplyInvitation();
    }
    /// <summary>
    /// Data for invitation
    /// </summary>
    [Serializable]
    public class InvitationData
    {
        /// <summary>
        /// Invitation applied count
        /// </summary>
        public int InvitationCount { get; set; }
        /// <summary>
        /// Applied invitation added values
        /// </summary>
        public Dictionary<string,int> InvitationAddedValues { get; set; }
        /// <summary>
        /// The username of invitor
        /// </summary>
        public string InvitorUserName { get; set; }
    }
}
