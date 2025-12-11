namespace BlazorFleetApp.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
public class ChatHub : Hub
{
    public async Task JoinGroup(int driverId, int dispatcherId)
    {
        var groupName = CreateGroupName(driverId, dispatcherId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(int driverId, int dispatcherId)
    {
        var groupName = CreateGroupName(driverId, dispatcherId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    private string CreateGroupName(int driverId, int dispatcherId)
    {
        return $"Driver{driverId}_Dispatcher{dispatcherId}";
    }
    public async Task SendMessageAsync(int driverId, int dispatcherId, string message , string firstName, string lastName)
    {
        var groupName = CreateGroupName(driverId, dispatcherId);
        var sender = $"{firstName} {lastName}";
        await Clients.GroupExcept(groupName, Context.ConnectionId)
            .SendAsync("ReceiveMessage", sender, message);
    }
}