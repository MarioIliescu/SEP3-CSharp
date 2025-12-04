namespace BlazorFleetApp.Services.Events;

public class RefreshDriversEvent
{
    public event Action? OnDriversChanged;

    public void NotifyDriversChanged()
    {
        OnDriversChanged?.Invoke();
    }
}