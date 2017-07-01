using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort.Info.Invitation
{
    public class InvitationInfo
    {
        public InvitationInfo()
        {
            InvitationPrize = new Balance();
        }
        public string ShareUrl { get; set; }
        public Balance InvitationPrize { get; set; }
    }
}
