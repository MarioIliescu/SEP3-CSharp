using BlazorFleetApp.Services.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorFleetApp.Services.Hubs;

public sealed class FleetHubClient : IAsyncDisposable
{
    private readonly NavigationManager _nav;
    private HubConnection? _hub;
    private bool _started;

    public event Func<FleetEventType, Task>? OnFleetEvent;

    public FleetHubClient(NavigationManager nav)
    {
        _nav = nav;
    }

    public async Task StartAsync()
    {
        // Do not start again if already connected
        if (_started && _hub?.State == HubConnectionState.Connected)
            return;

        if (_hub == null)
        {
            _hub = new HubConnectionBuilder()
                .WithUrl(_nav.ToAbsoluteUri("/fleethub"))
                .WithAutomaticReconnect()
                .Build();

            _hub.On<FleetEventType>("FleetEvent", evt =>
                OnFleetEvent?.Invoke(evt) ?? Task.CompletedTask
            );

            _hub.Reconnected += async _ => { await Task.CompletedTask; };
        }

        if (_hub.State != HubConnectionState.Connected)
            await _hub.StartAsync();

        _started = true;
    }

    public async Task JoinGroupAsync(string group)
    {
        if (_hub?.State == HubConnectionState.Connected)
            await _hub.InvokeAsync("JoinGroup", group);
    }

    public async Task LeaveGroupAsync(string group)
    {
        if (_hub?.State == HubConnectionState.Connected)
            await _hub.InvokeAsync("LeaveGroup", group);
    }

    public async Task NotifyGroupAsync(string group, FleetEventType evt)
    {
        if (_hub?.State == HubConnectionState.Connected)
            await _hub.InvokeAsync("NotifyEventToGroup", group, evt);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
        {
            try
            {
                await _hub.StopAsync();
            }
            catch
            {
                // ignore
            }
            await _hub.DisposeAsync();
            _hub = null;
        }

        _started = false;
    }
}
