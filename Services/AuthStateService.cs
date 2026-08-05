using CSE325_Team4_GroupProject.Models;

namespace CSE325_Team4_GroupProject.Services;

public class AuthStateService
{
    private static User? _currentUser;
    private static event Action? _onChange;

    public User? CurrentUser => _currentUser;

    public event Action? OnChange
    {
        add => _onChange += value;
        remove => _onChange -= value;
    }

    public void SetUser(User? user)
    {
        _currentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        _currentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        _onChange?.Invoke();
    }
}