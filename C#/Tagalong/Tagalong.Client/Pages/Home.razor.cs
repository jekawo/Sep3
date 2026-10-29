using Microsoft.AspNetCore.SignalR.Client;
using Tagalong.Client.Services;
using Tagalong.Contracts;
using TypedSignalR.Client;

namespace Tagalong.Client.Pages;

public partial class Home
{
    private HubConnection? hubConnection;
    private ILiveHub? liveHub;
    private string name = "";
    private string? reply;
    private readonly List<string> greetings = [];
    private string? error;

    private bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Connect after the first render so prerendering on the server doesn't open a connection.
        if (!firstRender)
            return;

        var hubUrl = Configuration["WebApi:HubUrl"] ?? "https://localhost:7092/hub";
        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        hubConnection.Reconnecting += _ => InvokeAsync(StateHasChanged);
        hubConnection.Reconnected += _ => InvokeAsync(StateHasChanged);
        hubConnection.Closed += _ => InvokeAsync(StateHasChanged);

        try
        {
            liveHub = hubConnection.CreateHubProxy<ILiveHub>();
            hubConnection.Register<IHubCallbacks>(new GreetingReceiver(ReceiveGreeting));
            await hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            error = $"Could not connect to {hubUrl}: {ex.Message}";
        }

        StateHasChanged();
    }

    private async Task SayHello()
    {
        if (liveHub is null)
        {
            return;
        }

        try
        {
            error = null;
            reply = await liveHub.SayHello(name);
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
    }

    private Task ReceiveGreeting(string name, string message)
    {
        greetings.Add($"{name}: {message}");
        return InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
