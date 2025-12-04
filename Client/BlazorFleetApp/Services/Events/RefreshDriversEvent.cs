namespace BlazorFleetApp.Services.Events;

public class RefreshDriversEvent
{
    public event Func<Task>? OnDriversChanged;

    public async Task NotifyDriversChangedAsync()
    {
        if (OnDriversChanged != null)
        {
            foreach (var handler in OnDriversChanged.GetInvocationList())
            {
                await ((Func<Task>)handler)();
            }
        }
    }
}