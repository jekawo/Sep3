using System.Net.Http.Json;
using Tagalong.Client.Services.Interfaces;
using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Services;

public class HttpInviteService : IInviteService
{
    private readonly HttpClient client;

    public HttpInviteService(HttpClient client)
    {
        this.client = client;
    }
    public async Task<List<InviteDto>> GetPendingInvitesAsync()
    {
        List<InviteDto>? invites = await client.GetFromJsonAsync<List<InviteDto>>("/invites/pending");

        return invites ?? new List<InviteDto>();
    }

    public async Task DeclineAsync(int inviteId)
    {
        HttpResponseMessage response =
            await client.PostAsync($"/invites/{inviteId}/decline", null);

        response.EnsureSuccessStatusCode();
    }
}
