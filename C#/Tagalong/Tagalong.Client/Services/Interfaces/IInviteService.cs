using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Services.Interfaces;

public interface IInviteService
{
    Task<List<InviteDto>> GetPendingInvitesAsync();
    Task DeclineAsync(int inviteId);
}
