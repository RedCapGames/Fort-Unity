namespace Fort
{
    public static class InvitationServiceExtensions
    {
        public static bool IsAnyInvitationAvailable(this IInvitationService invitationService)
        {
            return !string.IsNullOrEmpty(invitationService.ResolveInvitationToken());
        }
    }
}