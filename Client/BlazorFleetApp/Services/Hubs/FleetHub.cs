using BlazorFleetApp.Services.Events;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFleetApp.Services.Hubs
{
    public class FleetHub : Hub
    {
        // Single method for all events
        public async Task NotifyEvent(FleetEventType eventType)
        {

            await Clients.All.SendAsync("FleetEvent", eventType);
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task NotifyEventToGroup(string groupName, FleetEventType eventType)
        {
            await Clients.Group(groupName).SendAsync("FleetEvent", eventType);
        }

    }
}